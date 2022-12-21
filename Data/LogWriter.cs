namespace CoreMeridian.Data;

public static class LogWriter
{
    public static void Write<T>(this ILogger<T> logger, string logMessage, LogLevel logLevel = LogLevel.Information)
    {
        try
        {
            logger.Log(logLevel, logMessage);

            using StreamWriter txtWriter = File.AppendText("./log.txt");

            txtWriter.Write("\r\nLog Entry:");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            txtWriter.WriteLine("{0}", logMessage);
            txtWriter.WriteLine("-------------------------------------------------------");
        }
        catch (Exception) { }
    }
}
