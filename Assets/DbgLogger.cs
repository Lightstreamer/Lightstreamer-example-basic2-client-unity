using System;
using UnityEngine;
using Lightstreamer.DotNet.Logging.Log;
using ILogger = Lightstreamer.DotNet.Logging.Log.ILogger;

internal class DbgLogger : ILogger
{
    bool ILogger.IsDebugEnabled => true;

    bool ILogger.IsInfoEnabled => true;

    bool ILogger.IsWarnEnabled => true;

    bool ILogger.IsErrorEnabled => true;

    bool ILogger.IsFatalEnabled => true;

    void ILogger.Debug(string line)
    {
        Debug.Log(line);
    }

    void ILogger.Debug(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }

    void ILogger.Error(string line)
    {
        Debug.LogError(line);
    }

    void ILogger.Error(string line, Exception exception)
    {
        Debug.LogError(line + exception.Message);
    }

    void ILogger.Fatal(string line)
    {
        Debug.LogError(line);
    }

    void ILogger.Fatal(string line, Exception exception)
    {
        Debug.LogError(line + exception.Message);
    }

    void ILogger.Info(string line)
    {
        Debug.Log(line);
    }

    void ILogger.Info(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }

    void ILogger.Warn(string line)
    {
        Debug.Log(line);
    }

    void ILogger.Warn(string line, Exception exception)
    {
        Debug.Log(line + exception.Message);
    }
}