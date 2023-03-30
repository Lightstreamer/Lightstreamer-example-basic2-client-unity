using System;

using com.lightstreamer.client;
using com.lightstreamer.log;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

public class LightstreamerClientAsset : MonoBehaviour
{

    private int StateMachine = 0;
    private static LightstreamerClient client = null;
    public string pushUrl = "https://push.lightstreamer.com/";
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
        // Only for development.
        // System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
        // ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

        // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        if (client == null)
        {
            Debug.Log("LSClient initialized: " + pushUrl + " - " + adaptersSet);

            client = new LightstreamerClient(pushUrl, adaptersSet);
            // client.connectionOptions.RetryDelay = 50000;
            // client.connectionOptions.ReconnectTimeout = 50000;
            LightstreamerClient.setLoggerProvider(new ConsoleLoggerProvider(ConsoleLogLevel.DEBUG));

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
        try
        {
            Debug.Log("Let's go!  -> " + System.Environment.TickCount);
            client.addListener(new StocklistConnectionListener(this));
            //client.connectionOptions.ReconnectTimeout = 10000;

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

    //void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus)
    //    {
    //        Debug.Log("Pausa");
    //        client.disconnect();
    //    }
    //}

    void OnDestroy()
    {
        client.disconnect();

        Debug.Log("OnDestroy1");
    }

    void OnMouseOver()
    {
        Debug.Log("Mouse is over W.");
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
            lock (subscriptionsLS) {
                if (subscriptionsLS.Count > 0)
                {
                    Debug.Log("Subscribing ... " + subscriptionsLS.Count);

                    Subscription tab = this.subscriptionsLS.Dequeue() as Subscription;
                    if (tab != null)
                    {

                        Debug.Log("Client: " + client.Status);

                        client.subscribe(tab);

                        Debug.Log("Subscribe " + tab.DataAdapter + " " + tab.Mode);

                    } else
                    {
                        this.StateMachine = 3;
                    }

                    if (!goFlag)
                    {
                        goFlag = true;
                    }
                }
            }
        }
        else if (this.StateMachine == 2001)
        {
            while (this.nextUpdate.Count != 0)
            {
                // Debug.Log($"SendMessage Update {(ItemUpdate)this.nextUpdate.Dequeue()}");


                //Debug.Log("SendMessage Update.");
                BroadcastMessage("RTUpdates", (ItemUpdate)this.nextUpdate.Dequeue());
            }
            
            this.StateMachine = 3;
        }
        else if (this.StateMachine == 2010)
        {
            if (this.nextStatus != null)
            {
                // Debug.Log("SendMessage Status: " + this.nextStatus);
                BroadcastMessage("RTStatus", this.nextStatus);
            }
            this.nextStatus = null;
            this.StateMachine = 3;
        }
    }
}