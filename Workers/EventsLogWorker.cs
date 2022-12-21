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
        this._logFilePath = Path.Combine(serverConnection.logsPath, LogFileName.World2Log);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {nameof(EventsLogWorker)}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                long currentFileSize = GetFileSize(_logFilePath);

                if (currentFileSize > _lastFileSize)
                {
                    var logLines = ReadTail(_logFilePath, UpdateLastFileSize(currentFileSize));

                    var logoffRecords = logLines
                        .Where(line => line.Contains(JapaneseLogIdentifiers.LogoffLogId))
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

    private IEnumerable<string> ReadTail(string filename, long offset)
    {
        using FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        fileStream.Seek(offset * -1, SeekOrigin.End);

        var array = new byte[offset];
        fileStream.Read(array, 0, (int)offset);

        return EncodingConvert.GB2312ToUtf8(array)
            .Split('\n')
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