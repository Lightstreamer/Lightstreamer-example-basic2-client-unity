using UnityEngine;
using com.lightstreamer.client;
using System;
using UnityEngine.UI;

public class LightstreamerCubeAsset : LightstreamerAsset
{
    private Renderer myObj;

    private int redC = 0;
    private int greenC = 0;
    private int blueC = 0;
    private int cc = 1;
    private float scale = 3.0F;

    private void addLabel(string name)
    {
        
    }

    new void Start()
    {
        myObj = GetComponent<Renderer>();

        if (ItemName.EndsWith("2"))
        {
            cc = 0;
        }

        myObj.material.color = new Color(1.0F, 0.0F, 0.0F);

        addLabel(this.ItemName);
        
    }

    new void Update()
    {
        myObj.transform.Rotate(Vector3.up, 3.5F * Time.deltaTime);

        myObj.transform.localScale = new Vector3(3.0F, this.scale, 3.0F);

        myObj.material.color = new Color32(Convert.ToByte(this.redC), Convert.ToByte(this.greenC), Convert.ToByte(this.blueC), 0);
    }

    new public void RTUpdates(ItemUpdate update)
    {
        if (!update.ItemName.Equals(this.ItemName)) return;

        if (update.ItemName.StartsWith("item"))
        {
            if (update.isValueChanged(2))
            {
                float ftmp = 1;
                float.TryParse(update.getValue(2), out ftmp);

                int iValue = Mathf.FloorToInt(ftmp);
                int iValue2 = Mathf.FloorToInt(ftmp / 2.0F);

                if (this.cc == 0)
                {
                    this.redC = (this.redC + iValue) % 255;
                    this.blueC = (this.blueC + iValue2) % 255;
                }
                else
                {
                    this.greenC = (this.greenC + iValue) % 255;
                    this.blueC = (this.blueC + iValue2) % 255;
                }
            }
            if (update.isValueChanged(4))
            {
                float ftmp = 1;
                float.TryParse(update.getValue(4), out ftmp);

                Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>: " + ftmp);

                this.scale = 3.0F + (3.0F * ftmp / 5000.0F);

                Debug.Log("New scale: " + this.scale);
            }
        }
    }

    new public void RTStatus(String status)
    {

        if (status.Contains("CLOSE"))
        {
            this.blueC = 0;
            this.greenC = 0;
            this.redC = 255;
        }
        else if (status.Contains("CLOSE"))
        {
            this.blueC = 50;
            this.greenC = 130;
            this.redC = 155;
        }
        else if (status.Contains("POLLING"))
        {
            this.blueC = 0;
            this.greenC = 200;
            this.redC = 55;
        }
        else if (status.Contains("STREAMING"))
        {
            this.blueC = 70;
            this.greenC = 90;
            this.redC = 70;
        }

    }
}