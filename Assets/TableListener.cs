using UnityEngine;
using Lightstreamer.DotNet.Client;

public class StocklistHandyTableListener : IHandyTableListener
{

    private LightstreamerClientAsset target = null;

    public StocklistHandyTableListener(LightstreamerClientAsset target)
    {
        this.target = target;
    }

    void IHandyTableListener.OnRawUpdatesLost(int itemPos, string itemName, int lostUpdates)
    {
        Debug.Log("OnRawUpdatesLost.");
    }

    void IHandyTableListener.OnSnapshotEnd(int itemPos, string itemName)
    {
        Debug.Log("OnSnapshotEnd.");
    }

    void IHandyTableListener.OnUnsubscr(int itemPos, string itemName)
    {
        Debug.Log("OnUnsubscr.");
    }

    void IHandyTableListener.OnUnsubscrAll()
    {
        Debug.Log("OnUnsubscrAll.");
    }

    void IHandyTableListener.OnUpdate(int itemPos, string itemName, IUpdateInfo update)
    {

        this.target.ReDispatchUpdate(update);
        
    }
}