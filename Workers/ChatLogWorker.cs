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
        this._logFilePath = Path.Combine(serverConnection.logsPath, LogFileName.World2Chat);
        this._logger = logger;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {nameof(ChatLogWorker)}");

        while (stoppingToken.IsCancellationRequested)
        {
            try
            {
                long currentFileSize = GetFileSize(_logFilePath);

                if (currentFileSize > _lastFileSize)
                {
                    var logLines = ReadTail(_logFilePath, UpdateLastFileSize(currentFileSize));

                    var meridianoRequestsMessage = logLines
                        .Where(line => line.Contains(ChatTrigger.MeridianoTrigger))
                        .Select(_messageFactory.CreateChatMessageFromLogString);

                    foreach (var meridianoRequestMessage in meridianoRequestsMessage)
                    {
                        var issuerPlayer = _serverService.GetRoleData(meridianoRequestMessage.RoleID);

                        if (issuerPlayer is null)
                        {
                            _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Seu registro não foi encontrado. Entre em contato com a administração.");
                            _logger.Write(string.Format("Player {0} not found.", meridianoRequestMessage.RoleID), LogLevel.Critical);
                            continue;
                        }

                        if (issuerPlayer.GRoleStatus.MeridianData.Equals(GameConstants.MeridianData))
                        {
                            _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Você já possui meridiano completo.");
                            continue;
                        }

                        if (!issuerPlayer.PlayerHasRequiredItem(MeridianItem.Id))
                        {
                            _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, $"Você não possui o item de meridiano. Caso tenha conseguido o item recentemente, relogue o seu personagem e digite {ChatTrigger.MeridianoTrigger} novamente.");
                            continue;
                        }

                        _pendingOrders.Orders.Add(new MeridianOrder
                        {
                            IsLoggedOff = false,
                            Role = issuerPlayer
                        });

                        _serverService.SendPrivateMessage(meridianoRequestMessage.RoleID, "Deslogue o personagem para confirmar o meridiano.");
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

    private IEnumerable<string> ReadTail(string filename, long offset)
    {
        using FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        fileStream.Seek(offset * -1, SeekOrigin.End);

        var array = new byte[offset];
        fileStream.Read(array, 0, (int)offset);

        return Encoding.Default.GetString(array)
                               .Split("\n")
                               .Where(line => !string.IsNullOrEmpty(line.Trim()));
    }

    public static long GetFileSize(string fileName)
    {
        return new FileInfo(fileName).Length;
    }

    private static long UpdateLastFileSize(long fileSize)
    {
        long result = fileSize - _lastFileSize;
        _lastFileSize = fileSize;
        return result;
    }
}
