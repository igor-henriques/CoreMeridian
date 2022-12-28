namespace CoreMeridian.Workers;

internal sealed class ChatLogWorker : BackgroundService
{
    private readonly IServerService _serverService;
    private readonly IPendingMeridianOrders _pendingOrders;
    private readonly IChatMessageFactory _messageFactory;
    private readonly ILogger<ChatLogWorker> _logger;
    private readonly string _logFilePath;
    private static long _lastFileSize;

    public ChatLogWorker(
        IServerService serverService,
        IPendingMeridianOrders pendingOrders,
        IChatMessageFactory messageFactory,
        ServerConnection serverConnection,
        ILogger<ChatLogWorker> logger)
    {
        this._serverService = serverService;
        this._pendingOrders = pendingOrders;
        this._messageFactory = messageFactory;
        this._logFilePath = Path.Combine(serverConnection.logsPath, LogFilePath.World2Chat);        
        this._logger = logger;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        FileMethods.EnsureFileExists(_logFilePath);
        _lastFileSize = FileMethods.GetFileSize(_logFilePath);        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {nameof(ChatLogWorker)}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                long currentFileSize = FileMethods.GetFileSize(_logFilePath);
                
                if (currentFileSize > _lastFileSize)
                {
                    var logLines = FileMethods.ReadTail(_logFilePath, UpdateLastFileSize(currentFileSize));
                    
                    var meridianoRequestsMessage = logLines
                        .Select(line => _messageFactory.CreateChatMessageFromLogString(line, Encoding.Unicode))                        
                        .Where(message => message?.Text?.Contains(ChatTrigger.MeridianoTrigger) ?? false);
                    
                    foreach (var meridianoRequestMessage in meridianoRequestsMessage)
                    {
                        var issuerPlayer = _serverService.GetRoleData(meridianoRequestMessage.RoleID);

                        if (issuerPlayer is null)
                        {
                            _ = _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Seu registro não foi encontrado. Entre em contato com a administração.");
                            _logger.Write(string.Format("Player {0} not found.", meridianoRequestMessage.RoleID), LogLevel.Critical);
                            continue;
                        }

                        if (issuerPlayer.GRoleStatus.MeridianData.Equals(GameConstants.MeridianData))
                        {
                            _ = _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Você já possui meridiano completo.");
                            _logger.Write($"Role {meridianoRequestMessage.RoleID} already has meridian");
                            continue;
                        }

                        if (_pendingOrders.Orders.Any(order => order.IssuerRoleId.Equals(meridianoRequestMessage.RoleID)))
                        {
                            _ = _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Você já possui um pedido de meridiano pendente. Deslogue e logue novamente para aplicar as alterações.");
                            _logger.Write($"Role {meridianoRequestMessage.RoleID} already has a pending order");
                            continue;
                        }

                        if (!issuerPlayer.PlayerHasRequiredItem(MeridianItem.Id))
                        {
                            _ = _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, $"Você não possui o item de meridiano. Caso tenha conseguido o item recentemente, relogue o seu personagem e digite {ChatTrigger.MeridianoTrigger} novamente.");
                            _logger.Write($"Role {meridianoRequestMessage.RoleID} don't have the required item");
                            continue;
                        }

                        _pendingOrders.Orders.Add(new MeridianOrder
                        {
                            IsDelivered = false,
                            IsLoggedOff = false,
                            Role = issuerPlayer,
                            IssuerRoleId = issuerPlayer.GRoleBase.Id
                        });

                        _ = _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Deslogue o personagem para confirmar o meridiano.");
                        _logger.Write($"Meridian order for player {issuerPlayer.GRoleBase.Name} was registered. Waiting for logoff.");
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.Write(e.ToString(), LogLevel.Error);
                await Task.Delay(10_000, stoppingToken);
            }
        }
    }

    private static long UpdateLastFileSize(long fileSize)
    {
        long result = fileSize - _lastFileSize;
        _lastFileSize = fileSize;
        return result;
    }
}
