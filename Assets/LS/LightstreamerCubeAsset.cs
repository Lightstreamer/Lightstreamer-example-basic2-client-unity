using UnityEngine;
using com.lightstreamer.client;
using System;
using UnityEngine.UI;

public class LightstreamerCubeAsset : LightstreamerAsset
{
    private Renderer myObj;

    public Transform stockCube;

    public float refscale = 0.3f;   // quanto incide la % sull’altezza
    
    public float baselineY = 0f;       // la linea base comune (es. piano di riferimento)

    private bool blocK_color = false;

    void SetHeight(Transform cube, float percentChange)
    {
        float newHeight = Mathf.Max(0.1f, Mathf.Abs(percentChange) * refscale);

        Debug.Log("Setting height to: " + newHeight + " (change: " + percentChange + ", scaleFactor: " + refscale + ")");

        // Aggiorna scala
        Vector3 scale = cube.localScale;
        scale.y = newHeight;
        cube.localScale = scale;

        // Calcola posizione per ancorare la base alla baseline
        Vector3 pos = cube.position;

        if (percentChange >= 0)
        {
            // positivo: base rimane sulla baseline, cresce verso l’alto
            pos.y = baselineY + newHeight / 2f;
        }
        else
        {
            // negativo: base rimane sulla baseline, cresce verso il basso
            pos.y = baselineY - newHeight / 2f;
        }

        cube.position = pos;

        myObj.material.color = StockColorUtils.GetStockColor(percentChange);
    }

    private void addLabel(string name)
    {

    }

    new void Start()
    {
        myObj = GetComponent<Renderer>();
        stockCube = myObj.transform;

        myObj.material.color = new Color(1.0F, 0.0F, 0.0F);

        addLabel(this.ItemName);

    }

    void OnMouseOver()
    {
        Debug.Log("Mouse is over :" + ItemName);

        myObj.material.color = Color.yellow;
    }
    void OnMouseExit()
    {
        Debug.Log("Mouse is no longer on :" + ItemName);

        blocK_color = false;
    }

    new void Update()
    {
        // Nothing to do here
    }

    new public void RTUpdates(ItemUpdate update)
    {
        if (!update.ItemName.Equals(this.ItemName)) return;

        if (update.ItemName.StartsWith("item"))
        {
            if (update.isValueChanged("last_price"))
            {

                if (blocK_color) return;

                // int iValue = Mathf.FloorToInt(ftmp);
                // int iValue2 = Mathf.FloorToInt(ftmp / 2.0F);

                // if (this.cc == 0)
                // {
                //     this.redC = (this.redC + iValue) % 255;
                //     this.blueC = (this.blueC + iValue2) % 255;
                // }
                // else
                // {
                //     this.greenC = (this.greenC + iValue) % 255;
                //     this.blueC = (this.blueC + iValue2) % 255;
                // }
            }
            if (update.isValueChanged("pct_change"))
            {
                float change = 1.0F;
                float.TryParse(update.getValue("pct_change"), out change);

                Debug.Log("Percent change: " + change);

                SetHeight(stockCube, change);
            }
        }
    }

    new public void RTStatus(String status)
    {

        if (status.Contains("CLOSE"))
        {
            myObj.material.color = Color.gray;
        }
        else if (status.Contains("POLLING"))
        {
            myObj.material.color = Color.cyan;
        }
        else if (status.Contains("STREAMING"))
        {
            myObj.material.color = new Color(70, 90, 70);
        }

    }
}

public static class StockColorUtils {
    /// <summary>
    /// Restituisce un colore graduale in base alla variazione percentuale.
    /// Da bianco → verde per valori positivi,
    /// da bianco → rosso per valori negativi.
    /// Oltre ±10% il colore rimane al massimo della saturazione.
    /// </summary>
    public static Color GetStockColor(float percentChange) {
        // Normalizza intensità (0 = nessun cambiamento, 1 = massimo effetto)
        float intensity = Mathf.Clamp01(Mathf.Abs(percentChange) / 10f);

        Color baseColor;
        Color targetColor;

        if (percentChange >= 0) {
            // da verde scuro → verde pieno
            baseColor = new Color(0.8f, 1f, 0.8f); // light green
            targetColor = new Color(0f, 0.3f, 0f); // dark green
        } else {
            // da rosso scuro → rosso pieno
            baseColor = new Color(1f, 0.8f, 0.8f); // light red
            targetColor = new Color(0.3f, 0f, 0f); // dark red
        }


        return Color.Lerp(baseColor, targetColor, intensity);
    }
}
