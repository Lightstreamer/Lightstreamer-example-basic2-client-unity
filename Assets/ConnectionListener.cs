using UnityEngine;
using Lightstreamer.DotNetStandard.Client;

public class StocklistConnectionListener : IConnectionListener
{
    private LightstreamerClientAsset target = null;

    public StocklistConnectionListener(LightstreamerClientAsset target)
    {
        this.target = target;
    }

    void IConnectionListener.OnActivityWarning(bool warningOn)
    {

        this.target.ReStatusUpdate("WARN");
        Debug.Log("Warn!");

    }

    void IConnectionListener.OnClose()
    {

        this.target.ReStatusUpdate("CLOSE");
        Debug.Log("Close!");
        
    }

    void IConnectionListener.OnConnectionEstablished()
    {
        Debug.Log("3..............");
    }

    void IConnectionListener.OnDataError(PushServerException e)
    {
        Debug.Log("4..............");
    }

    void IConnectionListener.OnEnd(int cause)
    {
        Debug.Log("5..............");
    }

    void IConnectionListener.OnFailure(PushServerException e)
    {
        Debug.Log("6..............");
    }

    void IConnectionListener.OnFailure(PushConnException e)
    {
        Debug.Log("7..............");
    }

    void IConnectionListener.OnNewBytes(long bytes)
    {
        //
    }

    void IConnectionListener.OnSessionStarted(bool isPolling)
    {
        Debug.Log("9.............. OnSessionStarted: " + isPolling + "  -> " + System.Environment.TickCount);

        if (isPolling)
        {
            this.target.ReStatusUpdate("POLLING");
        }
        else
        {
            this.target.ReStatusUpdate("STREAMING");
        }

        this.target.GotConnection();
    }
}
