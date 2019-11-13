using System;

using com.lightstreamer.client;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class LightstreamerClientAsset : MonoBehaviour
{

    private int StateMachine = 0;
    private static LightstreamerClient client = null;
    public string pushUrl = "http://push.lightstreamer.com/";
    public string adaptersSet = "DEMO";
    private static Boolean goFlag = false;

    //private IUpdateInfo nextUpdate = null;
    private Queue nextUpdate = new Queue();
    private String nextStatus = null;

    private Queue subscriptionsLS = new Queue();

    public void GotConnection()
    {
        this.StateMachine = 2;
    }

    public void ReDispatchUpdate(ItemUpdate update)
    {
        this.StateMachine = 2001;
        this.nextUpdate.Enqueue(update);
    }

    
    public void ReStatusUpdate(String status)
    {
        this.StateMachine = 2010;
        this.nextStatus = status;
    }

    // Use this for initialization
    void Start()
    {
        if (client == null)
        {
            client = new LightstreamerClient(pushUrl, adaptersSet);
            LightstreamerClient.setLoggerProvider(new LogProvider());

            Debug.Log("LSClient initialized!");

            try
            {
                var th = new Thread(LightStreanerConnect);
                th.Start();
            }
            catch (SystemException se)
            {
                Debug.LogError("Unexpected error: " + se.Message);
            }

        }

        nextUpdate = Queue.Synchronized(nextUpdate);

        // Only for development.
        System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

        Debug.Log("Goooooooodmorning Lightstreamer ... ");

        Component i = GetComponent<Component>();
        
        Component[] Children = i.GetComponentsInChildren(typeof(Renderer));
        foreach (Component child in Children)
        {
            //child is your child transform
            Debug.Log("My child: " + child.tag);
            if (child.tag.StartsWith("lightstreamer"))
            {
                LightstreamerAsset ls = child.GetComponent(typeof(LightstreamerAsset)) as LightstreamerAsset;

                if (ls != null)
                {
                    Debug.Log("My child: " + ls.ItemName.Split(',')[0]);

                    // Add info for a subscription
                    Subscription sub = new Subscription("MERGE", ls.ItemName.Split(','), ls.Schema.Split(','))
                    {
                        DataAdapter = ls.DataAdapter,
                        RequestedSnapshot = "yes"
                    };
                    sub.addListener(new StocklistHandyTableListener(this));

                    subscriptionsLS.Enqueue(sub);

                    ls.addSender(this);
                }
            }
        }
    }

    private void LightStreanerConnect()
    {
        
            // StreamingTimeoutMillis = 10000,
            // EnableStreamSense = false
        
        try
        {
            Debug.Log("Let's go!  -> " + System.Environment.TickCount);
            client.addListener(new StocklistConnectionListener(this));
            client.connectionOptions.ReconnectTimeout = 10000;

            client.connect();
        }
        catch (Exception pfe)
        {
            Debug.LogError("Error: " + pfe.Message);
            this.StateMachine = 0;
        }
    }

    public void SndLsMsg(string msg)
    {
        if (client != null)
        {
            Debug.Log("Msg to Snd: " + msg);

            client.sendMessage(msg);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.StateMachine == 0)
        {
            this.StateMachine = 1;

        }
        else if (this.StateMachine == 1)
        {
            if (goFlag)
            {
                this.StateMachine = 2;
            }
        }
        else if (this.StateMachine == 2)
        {

            Thread.Sleep(400);

            this.StateMachine = 3;
            if (subscriptionsLS.Count > 0)
            {
                Debug.Log("Subscribing ...");

                Subscription tab = this.subscriptionsLS.Dequeue() as Subscription;
                while (tab != null)
                {

                    Debug.Log("Client: " + client.Status);

                    client.subscribe(tab);

                    Debug.Log("Subscribe " + tab.DataAdapter + " " + tab.Mode);

                    if (subscriptionsLS.Count > 0)
                        tab = this.subscriptionsLS.Dequeue() as Subscription;
                    else
                        tab = null;
                }

                if (!goFlag) goFlag = true;
            }
        }
        else if (this.StateMachine == 2001)
        {
            while (this.nextUpdate.Count != 0)
            {
                Debug.Log("SendMessage Update.");
                BroadcastMessage("RTUpdates", (ItemUpdate)this.nextUpdate.Dequeue());
            }
            
            this.StateMachine = 3;
        }
        else if (this.StateMachine == 2010)
        {
            if (this.nextStatus != null)
            {
                Debug.Log("SendMessage Status: " + this.nextStatus);
                BroadcastMessage("RTStatus", this.nextStatus);
            }
            this.nextStatus = null;
            this.StateMachine = 3;
        }
    }
}