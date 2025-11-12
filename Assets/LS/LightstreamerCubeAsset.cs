using UnityEngine;
using com.lightstreamer.client;
using System;
using UnityEngine.UI;

public class LightstreamerCubeAsset : LightstreamerAsset
{
    private Renderer myObj;

    public Transform stockCube;

    public float refscale = 0.3f;   // how much the percentage affects the height
    
    [Header("DPI/Resolution Adaptation")]
    public bool useDPIScaling = true;   // enable automatic scaling
    public float manualScaleFactor = 1f; // manual scale factor (if DPI disabled)
    
    [Header("PC Specific Correction")]
    public bool usePerPCCorrection = false; // enable corrections for specific PCs
    public float pcSpecificCorrection = 1f; // correction for this specific PC
    
    public float baselineY = 0f;       // the common baseline (e.g. reference plane)

    [Header("Debug Info")]
    public float lastPercentChange = 0f;
    public float lastEffectiveScale = 0f;
    public float lastNewHeight = 0f;
    public Vector3 lastFinalScale = Vector3.zero;

    private bool blocK_color = false;

    void SetHeight(Transform cube, float percentChange)
    {
        // Calculate the effective scale factor
        float effectiveScale = refscale;
        
        if (useDPIScaling && DPIManager.Instance != null)
        {
            float dpiScale = DPIManager.Instance.GetScaleFactor();
            effectiveScale = refscale * dpiScale;
            
            Debug.Log($"DPI Scaling - Original: {refscale}, DPI Factor: {dpiScale}, Before PC correction: {effectiveScale}");
        }
        else if (!useDPIScaling)
        {
            effectiveScale = refscale * manualScaleFactor;
            Debug.Log($"Manual Scaling - Original: {refscale}, Manual Factor: {manualScaleFactor}, Effective: {effectiveScale}");
        }
        
        // Apply PC specific correction if enabled
        if (usePerPCCorrection)
        {
            effectiveScale *= pcSpecificCorrection;
            Debug.Log($"PC Specific Correction Applied: {pcSpecificCorrection}, Final Effective Scale: {effectiveScale}");
        }
        
        float newHeight = Mathf.Max(0.1f, Mathf.Abs(percentChange) * effectiveScale);

        // Save values for debugging
        lastPercentChange = percentChange;
        lastEffectiveScale = effectiveScale;
        lastNewHeight = newHeight;

        Debug.Log($"[{ItemName}] Setting height to: {newHeight:F3} (change: {percentChange:F2}%, effectiveScale: {effectiveScale:F3})");

        // Update scale
        Vector3 scale = cube.localScale;
        scale.y = newHeight;
        cube.localScale = scale;

        // Calculate position to anchor the base to the baseline
        Vector3 pos = cube.position;

        if (percentChange >= 0)
        {
            // positive: base stays on baseline, grows upward
            pos.y = baselineY + newHeight / 2f;
        }
        else
        {
            // negative: base stays on baseline, grows downward
            pos.y = baselineY - newHeight / 2f;
        }

        cube.position = pos;

        // Save final scale for debugging
        lastFinalScale = cube.localScale;

        // Final log of dimensions after modification
        Debug.Log($"[{ItemName}] Final dimensions - LocalScale: ({cube.localScale.x:F3}, {cube.localScale.y:F3}, {cube.localScale.z:F3}), " +
                 $"WorldScale: ({cube.lossyScale.x:F3}, {cube.lossyScale.y:F3}, {cube.lossyScale.z:F3}), " +
                 $"Position: ({cube.position.x:F2}, {cube.position.y:F2}, {cube.position.z:F2})");

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
                string changeValue = update.getValue("pct_change");
                
                // Use InvariantCulture to force dot as decimal separator
                if (!float.TryParse(changeValue, System.Globalization.NumberStyles.Float, 
                                   System.Globalization.CultureInfo.InvariantCulture, out change))
                {
                    Debug.LogWarning($"[{ItemName}] Failed to parse pct_change value: '{changeValue}'");
                    change = 0f; // default value
                }

                Debug.Log($"[{ItemName}] Percent change raw: '{changeValue}' -> parsed: {change}");

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
    /// Returns a gradual color based on percentage change.
    /// From white → green for positive values,
    /// from white → red for negative values.
    /// Beyond ±10% the color remains at maximum saturation.
    /// </summary>
    public static Color GetStockColor(float percentChange) {
        // Normalize intensity (0 = no change, 1 = maximum effect)
        float intensity = Mathf.Clamp01(Mathf.Abs(percentChange) / 10f);

        Color baseColor;
        Color targetColor;

        if (percentChange >= 0) {
            // from dark green → full green
            baseColor = new Color(0.8f, 1f, 0.8f); // light green
            targetColor = new Color(0f, 0.3f, 0f); // dark green
        } else {
            // from dark red → full red
            baseColor = new Color(1f, 0.8f, 0.8f); // light red
            targetColor = new Color(0.3f, 0f, 0f); // dark red
        }


        return Color.Lerp(baseColor, targetColor, intensity);
    }
}
