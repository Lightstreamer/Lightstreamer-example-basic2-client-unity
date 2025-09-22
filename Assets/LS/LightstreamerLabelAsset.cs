using UnityEngine;
using com.lightstreamer.client;
using System;
using UnityEngine.UI;
using TMPro;

public class LightstreamerLabelAsset : LightstreamerAsset
{
    private TMP_Text  label;
    new void Start()
    {
        // retrieve the TextMeshPro component
        label = GetComponent<TMP_Text>();
        label.text = "+0.0";
    }

    new public void RTUpdates(ItemUpdate update)
    {
        if (!update.ItemName.Equals(this.ItemName)) return;

        if (update.ItemName.StartsWith("item"))
        {
            if (update.isValueChanged("pct_change"))
            {
                label.text = update.getValue("pct_change") + "%";
            }
        }
    }

}