namespace CoreMeridian.Workers;

internal sealed class EventsLogWorker : BackgroundService
{
    private readonly ServerConnection _serverConnection;
    private readonly IPendingMeridianOrders _pendingOrders;
    private readonly ILogger<MeridianOrder> _logger;
    private readonly string _logFilePath;
    private static long _lastFileSize;

    public EventsLogWorker(
        ServerConnection serverConnection,
        IPendingMeridianOrders pendingOrders,
        ILogger<MeridianOrder> logger)
    {
        this._serverConnection = serverConnection;
        this._pendingOrders = pendingOrders;
        this._logger = logger;
        this._logFilePath = Path.Combine(serverConnection.logsPath, LogFilePath.World2FormatLog);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        FileMethods.EnsureFileExists(_logFilePath);
        _lastFileSize = FileMethods.GetFileSize(_logFilePath);        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {nameof(EventsLogWorker)}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                long currentFileSize = FileMethods.GetFileSize(_logFilePath);

                if (currentFileSize > _lastFileSize)
                {
                    var logLines = FileMethods.ReadTail(_logFilePath, UpdateLastFileSize(currentFileSize));

                    var logoffRecords = logLines
                        .Where(line => line.Contains(LogIdentifier.Logoff))
                        .Select(LogoffRecord.ParseFromLogString);

                    if (logoffRecords.Any())
                        _pendingOrders.UpdateLogoffStatus(logoffRecords);
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