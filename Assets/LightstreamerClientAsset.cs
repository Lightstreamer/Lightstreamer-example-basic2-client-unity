using System;

using Lightstreamer.DotNet.Client;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class LightstreamerClientAsset : MonoBehaviour
{

    private int StateMachine = 0;
    private static LSClient client = null;
    private static ConnectionInfo cInfo;
    private static Boolean goFlag = false;

    public String Hostname = "https://push.lightstreamer.com/";
    public String AdapterSet = "DEMO";

    //private IUpdateInfo nextUpdate = null;
    private Queue nextUpdate = new Queue();
    private String nextStatus = null;

    private Queue subscriptionsLS = new Queue();

    public void GotConnection()
    {
        this.StateMachine = 2;
    }

    public void ReDispatchUpdate(IUpdateInfo update)
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
            client = new LSClient();

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
                    Debug.Log("My child: " + ls.ItemName);

                    // Add info for a subscription
                    ExtendedTableInfo tableInfo = new ExtendedTableInfo(
                        ls.ItemName.Split(','),
                       "MERGE",
                       ls.Schema.Split(','),
                       true)
                    {
                        DataAdapter = ls.DataAdapter
                    };
                    subscriptionsLS.Enqueue(tableInfo);

                    ls.addSender(this);
                }
            }
        }
    }

    private void LightStreanerConnect()
    {
        cInfo = new ConnectionInfo()
        {
            PushServerUrl = Hostname,
            Adapter = AdapterSet,
            StreamingTimeoutMillis = 10000,
            EnableStreamSense = false
        };
        try
        {
            Debug.Log("Let's go!  -> " + System.Environment.TickCount);

            client.OpenConnection(cInfo, new StocklistConnectionListener(this));
        }
        catch (PushConnException pfe)
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

            client.SendMessage(msg);
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
            this.StateMachine = 3;
            if (subscriptionsLS.Count > 0)
            {
                Debug.Log("Subscribing ...");

                ExtendedTableInfo tab = this.subscriptionsLS.Dequeue() as ExtendedTableInfo;
                while (tab != null)
                {
                    client.SubscribeTable(
                        tab,
                        new StocklistHandyTableListener(this),
                        false);

                    Debug.Log("Subscribe " + tab.DataAdapter);

                    if (subscriptionsLS.Count > 0)
                        tab = this.subscriptionsLS.Dequeue() as ExtendedTableInfo;
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
                BroadcastMessage("RTUpdates", (IUpdateInfo)this.nextUpdate.Dequeue());
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