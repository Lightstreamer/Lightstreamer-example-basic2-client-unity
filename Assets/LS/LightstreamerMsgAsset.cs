using UnityEngine;
using com.lightstreamer.client;
using System;
using TMPro;
using com.lightstreamer.client.@internal.update;

public class LightstreamerMsgAsset : LightstreamerAsset
{
    
    private String msgForTheServer = "";
    private TMP_Text  label;
    
    private const int MAX_LENGTH = 50;

    private
    new void Start()
    {
        // retrieve the TextMeshPro component
        label = GetComponent<TMP_Text>();

        Debug.Log("LightstreamerMsgAsset::Start: " + label.text);
        // label.text = "Hello World!";
    }
    
    new public void Update()
    {

        string msgFromTheUser = Input.inputString;

        if (msgFromTheUser.Length > 0)
        {

            Debug.Log("New input:" + msgFromTheUser);

            if (msgForTheServer.Length < MAX_LENGTH)
            {
                msgForTheServer += msgFromTheUser;
            }
            else
            {
                msgForTheServer = msgFromTheUser;
            }

            this.sender.SndLsMsg("RT|0|" + msgForTheServer);

        }

    }

    new public void RTUpdates(ItemUpdate update)
    {

        Debug.Log("Update 4 item: " + update.ItemName);

        if (!update.ItemName.Equals(this.ItemName)) return;

        if (update.isValueChanged("message"))
        {
            Debug.Log("Message:" + update.getValue("message"));

            label.text = update.getValue("message");
        }
    }
}