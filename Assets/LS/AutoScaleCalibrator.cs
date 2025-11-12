using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script per auto-calibrare la scalatura basata sui parametri specifici del PC
/// Aiuta a risolvere problemi di scalatura inconsistente tra diversi computer
/// </summary>
public class AutoScaleCalibrator : MonoBehaviour
{
    [Header("Configurazione Auto-Calibrazione")]
    public bool enableAutoCalibration = true;
    public bool applyCalibrationOnStart = true;
    
    [Header("Profili PC Conosciuti")]
    [SerializeField]
    private List<PCProfile> knownPCProfiles = new List<PCProfile>();
    
    [Header("Parametri di Calibrazione")]
    public float defaultScaleCorrection = 1.0f;
    public float dpiThreshold = 10f; // soglia per considerare DPI "simili"
    
    [Header("Debug Info")]
    public string currentPCSignature;
    public float appliedCorrection = 1.0f;
    public bool profileFound = false;
    
    [System.Serializable]
    public class PCProfile
    {
        public string name;
        public float targetDPI;
        public Vector2 targetResolution;
        public float scaleCorrection;
        public string notes;
        
        [Space(10)]
        public string osVersion;
        public string gpuName;
    }
    
    void Start()
    {
        if (enableAutoCalibration && applyCalibrationOnStart)
        {
            PerformAutoCalibration();
        }
    }
    
    public void PerformAutoCalibration()
    {
        // Genera signature del PC corrente
        currentPCSignature = GeneratePCSignature();
        
        Debug.Log($"PC Signature: {currentPCSignature}");
        
        // Cerca un profilo corrispondente
        PCProfile matchingProfile = FindMatchingProfile();
        
        if (matchingProfile != null)
        {
            appliedCorrection = matchingProfile.scaleCorrection;
            profileFound = true;
            ApplyCorrection(appliedCorrection);
            
            Debug.Log($"Found matching profile: {matchingProfile.name}, applying correction: {appliedCorrection}");
        }
        else
        {
            // Applica correzione basata su euristica
            appliedCorrection = CalculateHeuristicCorrection();
            profileFound = false;
            ApplyCorrection(appliedCorrection);
            
            Debug.Log($"No matching profile found, using heuristic correction: {appliedCorrection}");
            
            // Suggerisci di creare un nuovo profilo
            SuggestNewProfile();
        }
    }
    
    string GeneratePCSignature()
    {
        float dpi = Screen.dpi > 0 ? Screen.dpi : 96f;
        Vector2 resolution = new Vector2(Screen.width, Screen.height);
        string os = SystemInfo.operatingSystem;
        string gpu = SystemInfo.graphicsDeviceName;
        
        return $"DPI:{dpi:F0}_RES:{resolution.x}x{resolution.y}_OS:{GetOSShortName(os)}_GPU:{GetGPUShortName(gpu)}";
    }
    
    string GetOSShortName(string fullOS)
    {
        if (fullOS.Contains("Windows 10")) return "Win10";
        if (fullOS.Contains("Windows 11")) return "Win11";
        if (fullOS.Contains("Windows")) return "Win";
        return "Other";
    }
    
    string GetGPUShortName(string fullGPU)
    {
        if (fullGPU.Contains("NVIDIA")) return "NVIDIA";
        if (fullGPU.Contains("AMD")) return "AMD";
        if (fullGPU.Contains("Intel")) return "Intel";
        return "Other";
    }
    
    PCProfile FindMatchingProfile()
    {
        float currentDPI = Screen.dpi > 0 ? Screen.dpi : 96f;
        Vector2 currentRes = new Vector2(Screen.width, Screen.height);
        
        foreach (PCProfile profile in knownPCProfiles)
        {
            // Controlla se DPI è simile
            bool dpiMatch = Mathf.Abs(currentDPI - profile.targetDPI) <= dpiThreshold;
            
            // Controlla se risoluzione è simile (±10%)
            bool resMatch = (Mathf.Abs(currentRes.x - profile.targetResolution.x) / profile.targetResolution.x) < 0.1f &&
                           (Mathf.Abs(currentRes.y - profile.targetResolution.y) / profile.targetResolution.y) < 0.1f;
            
            if (dpiMatch && resMatch)
            {
                return profile;
            }
        }
        
        return null;
    }
    
    float CalculateHeuristicCorrection()
    {
        float currentDPI = Screen.dpi > 0 ? Screen.dpi : 96f;
        Vector2 currentRes = new Vector2(Screen.width, Screen.height);
        
        // Euristica basata sui tuoi dati:
        // - Il tuo PC (120 DPI, scale factor 1.13) funziona bene
        // - PC collega (96 DPI, scale factor 1.2) ha oggetti troppo alti
        
        float correction = 1.0f;
        
        // Se DPI è basso (come PC del collega) riduci la scala
        if (currentDPI <= 100f)
        {
            correction = 0.85f; // Riduci del 15%
            Debug.Log("Low DPI detected, reducing scale");
        }
        // Se DPI è alto, mantieni scala normale o aumenta leggermente
        else if (currentDPI >= 120f)
        {
            correction = 1.0f; // Mantieni normale
            Debug.Log("High DPI detected, maintaining normal scale");
        }
        // DPI intermedio
        else
        {
            // Interpolazione lineare tra 100 e 120 DPI
            float t = (currentDPI - 100f) / 20f;
            correction = Mathf.Lerp(0.85f, 1.0f, t);
            Debug.Log($"Medium DPI detected ({currentDPI}), interpolated correction: {correction}");
        }
        
        return correction;
    }
    
    void ApplyCorrection(float correction)
    {
        // Applica al DPIManager se disponibile
        if (DPIManager.Instance != null)
        {
            DPIManager.Instance.useManualCorrection = true;
            DPIManager.Instance.manualScaleCorrection = correction;
        }
        
        // Applica a tutti i LightstreamerCubeAsset nella scena
        LightstreamerCubeAsset[] cubes = FindObjectsOfType<LightstreamerCubeAsset>();
        foreach (LightstreamerCubeAsset cube in cubes)
        {
            cube.usePerPCCorrection = true;
            cube.pcSpecificCorrection = correction;
        }
        
        Debug.Log($"Applied scale correction {correction} to {cubes.Length} cube assets");
    }
    
    void SuggestNewProfile()
    {
        float currentDPI = Screen.dpi > 0 ? Screen.dpi : 96f;
        Vector2 currentRes = new Vector2(Screen.width, Screen.height);
        
        Debug.Log("=== SUGGESTED NEW PC PROFILE ===");
        Debug.Log($"Name: PC_{GetOSShortName(SystemInfo.operatingSystem)}_{currentDPI:F0}DPI");
        Debug.Log($"Target DPI: {currentDPI}");
        Debug.Log($"Target Resolution: {currentRes}");
        Debug.Log($"Scale Correction: {appliedCorrection}");
        Debug.Log($"OS Version: {SystemInfo.operatingSystem}");
        Debug.Log($"GPU Name: {SystemInfo.graphicsDeviceName}");
        Debug.Log("Copy these values to create a new profile in the inspector!");
    }
    
    [ContextMenu("Test Auto Calibration")]
    public void TestAutoCalibration()
    {
        PerformAutoCalibration();
    }
    
    [ContextMenu("Add Current PC as Profile")]
    public void AddCurrentPCAsProfile()
    {
        float currentDPI = Screen.dpi > 0 ? Screen.dpi : 96f;
        Vector2 currentRes = new Vector2(Screen.width, Screen.height);
        
        PCProfile newProfile = new PCProfile
        {
            name = $"PC_{GetOSShortName(SystemInfo.operatingSystem)}_{currentDPI:F0}DPI",
            targetDPI = currentDPI,
            targetResolution = currentRes,
            scaleCorrection = appliedCorrection,
            osVersion = SystemInfo.operatingSystem,
            gpuName = SystemInfo.graphicsDeviceName,
            notes = "Auto-generated profile"
        };
        
        knownPCProfiles.Add(newProfile);
        Debug.Log($"Added new profile: {newProfile.name}");
    }
}