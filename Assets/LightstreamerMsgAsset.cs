using UnityEngine;
using Lightstreamer.DotNetStandard.Client;
using System;

public class LightstreamerMsgAsset : LightstreamerAsset
{
    
    private String msgForTheServer = "";
    
    new public void Update()
    {

        string msgFromTheUser = Input.inputString;

        if (msgFromTheUser.Length > 0)
        {

            Debug.Log("New input:" + msgFromTheUser);

            if (msgForTheServer.Length < 20)
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

    new public void RTUpdates(IUpdateInfo update)
    {

        if (!update.ItemName.Equals(this.ItemName)) return;

        if (update.IsValueChanged("message"))
        {
            Debug.Log("Message:" + update.GetNewValue("message"));

            GetComponent<TextMesh>().text = update.GetNewValue("message");
        }
    }
}