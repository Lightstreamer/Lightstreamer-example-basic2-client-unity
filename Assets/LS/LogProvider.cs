using com.lightstreamer.log;

internal class LogProvider : ILoggerProvider
{
    ILogger ILoggerProvider.GetLogger(string category)
    {
        return new DbgLogger();
    }
}