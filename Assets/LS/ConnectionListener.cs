using UnityEngine;
using com.lightstreamer.client;

public class StocklistConnectionListener : ClientListener
{
    private LightstreamerClientAsset target = null;

    public StocklistConnectionListener(LightstreamerClientAsset target)
    {
        this.target = target;
    }

    void ClientListener.onListenEnd()
    {
        Debug.Log("On Listen End.");
    }

    void ClientListener.onListenStart()
    {
        Debug.Log("On Listen Start.");
    }

    void ClientListener.onPropertyChange(string property)
    {
        Debug.Log("Property changed " + property + ".");
    }

    void ClientListener.onServerError(int errorCode, string errorMessage)
    {
        Debug.Log("On Server Error:" + errorMessage);
    }

    void ClientListener.onStatusChange(string status)
    {
        Debug.Log("Status changed: " + status + "  -> " + System.Environment.TickCount + ".");
        if (status.StartsWith("CONNECTED:WS"))
        {
            if (status.EndsWith("POLLING"))
            {
                this.target.ReStatusUpdate("POLLING");
            }
            else if (status.EndsWith("STREAMING"))
            {
                this.target.ReStatusUpdate("STREAMING");
            }

            this.target.GotConnection();
        }
        else if (status.StartsWith("CONNECTED:HT"))
        {
            if (status.EndsWith("POLLING"))
            {
                this.target.ReStatusUpdate("POLLING");
            }
            else if (status.EndsWith("STREAMING"))
            {
                this.target.ReStatusUpdate("STREAMING");
            }

            this.target.GotConnection();
        }
        else if (status.StartsWith("CONNECTING"))
        {
            // ..
        }
        else if (status.StartsWith("DISCONNECTED"))
        {
            this.target.ReStatusUpdate("CLOSE");
            Debug.Log("Close!");
        }
        else if (status.StartsWith("STALLED"))
        {
            this.target.ReStatusUpdate("WARN");
            Debug.Log("Warn!");
        }
    }
}
