using System;
using UnityEngine;
using com.lightstreamer.log;
using ILogger = com.lightstreamer.log.ILogger;

internal class DbgLogger : ILogger
{
    bool ILogger.IsDebugEnabled => true;

    bool ILogger.IsInfoEnabled => true;

    bool ILogger.IsWarnEnabled => true;

    bool ILogger.IsErrorEnabled => true;

    bool ILogger.IsFatalEnabled => true;

    bool ILogger.IsTraceEnabled => true;
    void ILogger.Debug(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }

    void ILogger.Trace(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }

    void ILogger.Error(string line, Exception exception)
    {
        Debug.LogError(line + exception.Message);
    }

    void ILogger.Fatal(string line, Exception exception)
    {
        Debug.LogError(line + exception.Message);
    }

    void ILogger.Info(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }

    void ILogger.Warn(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }
}