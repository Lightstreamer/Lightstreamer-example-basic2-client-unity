using UnityEngine;
using com.lightstreamer.client;

public class StocklistHandyTableListener : SubscriptionListener
{

    private LightstreamerClientAsset target = null;

    public StocklistHandyTableListener(LightstreamerClientAsset target)
    {
        this.target = target;

        Debug.Log("Qui.");
    }

    void SubscriptionListener.onClearSnapshot(string itemName, int itemPos)
    {
        Debug.Log("ClearSnapshot for Item " + itemName + ".");
    }

    void SubscriptionListener.onCommandSecondLevelItemLostUpdates(int lostUpdates, string key)
    {
        throw new System.NotImplementedException();
    }

    void SubscriptionListener.onCommandSecondLevelSubscriptionError(int code, string message, string key)
    {
        throw new System.NotImplementedException();
    }

    void SubscriptionListener.onEndOfSnapshot(string itemName, int itemPos)
    {
        Debug.Log("Snapshot End for Item " + itemName + ".");
    }

    void SubscriptionListener.onItemLostUpdates(string itemName, int itemPos, int lostUpdates)
    {
        Debug.Log("Updates Lost for Item " + itemName + ".");
    }

    void SubscriptionListener.onItemUpdate(ItemUpdate itemUpdate)
    {
        Debug.Log("Update received for " + itemUpdate.ItemName + ".");

        target.ReDispatchUpdate(itemUpdate);
    }

    void SubscriptionListener.onListenEnd(Subscription subscription)
    {
        // ...
    }

    void SubscriptionListener.onListenStart(Subscription subscription)
    {
        // ...
    }

    void SubscriptionListener.onRealMaxFrequency(string frequency)
    {
        // ...
    }

    void SubscriptionListener.onSubscription()
    {
        Debug.Log("On Subscription.");
    }

    void SubscriptionListener.onSubscriptionError(int code, string message)
    {
        Debug.Log("Subscription error: " + message + " (" + code + ").");
    }

    void SubscriptionListener.onUnsubscription()
    {
        Debug.Log("On Unsubscription.");
    }

}