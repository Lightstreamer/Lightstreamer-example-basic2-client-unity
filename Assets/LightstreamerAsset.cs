using UnityEngine;
using com.lightstreamer.client;
using System;

public class LightstreamerAsset : MonoBehaviour
{

    public string ItemName = "";
    public string Schema = "";
    public string DataAdapter = "";
    
    protected LightstreamerClientAsset sender = null;
    
    // Use this for initialization
    public void Start()
    {
      
        Debug.Log("Start " + ItemName);

    }

    // Update is called once per frame
    public void Update()
    {

        // Nothing.
        
    }

    public void addSender(LightstreamerClientAsset s)
    {

        Debug.Log("addSender::");
        this.sender = s;

    }

    public void RTUpdates(ItemUpdate update)
    {

        Debug.Log("Received Update Message: " + update.ItemName + " == " + this.ItemName);
        
    }

    public void RTStatus(String status)
    {

        Debug.Log("Received Status Message: " + status);
        
    }
}
