using Lightstreamer.DotNet.Logging.Log;

internal class LogProvider : ILoggerProvider
{
    ILogger ILoggerProvider.GetLogger(string category)
    {
        return new DbgLogger();
    }
}