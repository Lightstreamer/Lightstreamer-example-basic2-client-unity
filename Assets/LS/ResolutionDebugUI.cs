using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script di debug per visualizzare informazioni sulla risoluzione e DPI
/// Utile per diagnosticare problemi di scalatura su diversi PC
/// </summary>
public class ResolutionDebugUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text debugText;
    public Canvas debugCanvas;
    
    [Header("Settings")]
    public bool showDebugInfo = true;
    public KeyCode toggleKey = KeyCode.F1;
    
    private string debugInfo = "";
    
    void Start()
    {
        if (debugCanvas == null)
        {
            CreateDebugUI();
        }
        
        if (showDebugInfo)
        {
            debugCanvas.enabled = true;
        }
    }
    
    void CreateDebugUI()
    {
        // Crea Canvas per debug
        GameObject canvasGO = new GameObject("Debug Canvas");
        debugCanvas = canvasGO.AddComponent<Canvas>();
        debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        debugCanvas.sortingOrder = 999;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Crea testo debug
        GameObject textGO = new GameObject("Debug Text");
        textGO.transform.SetParent(debugCanvas.transform, false);
        
        debugText = textGO.AddComponent<Text>();
        debugText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        debugText.fontSize = 11;
        debugText.color = Color.white;
        debugText.alignment = TextAnchor.UpperLeft;
        debugText.verticalOverflow = VerticalWrapMode.Overflow;
        
        // Posiziona il testo in alto a sinistra
        RectTransform rectTransform = debugText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(10, -10);
        rectTransform.sizeDelta = new Vector2(600, 500);
        
        // Aggiungi background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(debugText.transform, false);
        bgGO.transform.SetAsFirstSibling();
        
        Image bg = bgGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = new Vector2(-5, -5);
        bgRect.offsetMax = new Vector2(5, 5);
        
        DontDestroyOnLoad(canvasGO);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            showDebugInfo = !showDebugInfo;
            debugCanvas.enabled = showDebugInfo;
        }
        
        if (showDebugInfo && debugText != null)
        {
            UpdateDebugInfo();
            debugText.text = debugInfo;
        }
    }
    
    void UpdateDebugInfo()
    {
        debugInfo = "=== RESOLUTION & DPI INFO ===\n";
        debugInfo += $"Screen: {Screen.width}x{Screen.height} | Native: {Display.main.systemWidth}x{Display.main.systemHeight}\n";
        debugInfo += $"DPI: {Screen.dpi} | Fullscreen: {Screen.fullScreenMode} | Refresh: {Screen.currentResolution.refreshRate}Hz\n";
        
        // Info DPI Manager se disponibile
        if (DPIManager.Instance != null)
        {
            debugInfo += $"DPI Manager - Current: {DPIManager.Instance.currentDPI} | Scale: {DPIManager.Instance.dpiScale:F2} | Factor: {DPIManager.Instance.GetScaleFactor():F2}\n";
            if (DPIManager.Instance.useManualCorrection)
            {
                debugInfo += $"Manual Correction: {DPIManager.Instance.manualScaleCorrection:F2}\n";
            }
        }
        
        // Info Resolution Manager se disponibile
        ResolutionManager resManager = FindObjectOfType<ResolutionManager>();
        if (resManager != null)
        {
            debugInfo += "\n=== RESOLUTION MANAGER ===\n";
            debugInfo += $"Current: {resManager.currentResolution}\n";
            debugInfo += $"Native: {resManager.nativeResolution}\n";
            debugInfo += $"Aspect Ratio: {resManager.aspectRatio:F2}\n";
        }
        
        // Info Canvas se disponibile
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length > 0)
        {
            debugInfo += "\n=== CANVAS INFO ===\n";
            debugInfo += $"Number of Canvases: {canvases.Length}\n";
            
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name != "Debug Canvas")
                {
                    CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                    if (scaler != null)
                    {
                        debugInfo += $"{canvas.name}: {scaler.uiScaleMode}, Ref: {scaler.referenceResolution}\n";
                    }
                }
            }
        }
        
        // Info Cubi LightstreamerCubeAsset
        LightstreamerCubeAsset[] cubes = FindObjectsOfType<LightstreamerCubeAsset>();
        if (cubes.Length > 0)
        {
            debugInfo += "\n=== LIGHTSTREAMER CUBES ===\n";
            debugInfo += $"Number of Cubes: {cubes.Length}\n";
            
            for (int i = 0; i < cubes.Length; i++)
            {
                LightstreamerCubeAsset cube = cubes[i];
                if (cube.stockCube != null)
                {
                    Vector3 scale = cube.stockCube.localScale;
                    Vector3 position = cube.stockCube.position;
                    Vector3 worldScale = cube.stockCube.lossyScale;
                    
                    debugInfo += $"[{i + 1}] {cube.ItemName} - Height: {scale.y:F3} (Change: {cube.lastPercentChange:F1}%)\n";
                    debugInfo += $"    LocalScale: ({scale.x:F2},{scale.y:F3},{scale.z:F2}) Pos: ({position.x:F1},{position.y:F2},{position.z:F1})\n";
                    debugInfo += $"    EffectiveScale: {cube.lastEffectiveScale:F3} = RefScale: {cube.refscale:F2}";
                    
                    if (cube.useDPIScaling && DPIManager.Instance != null)
                    {
                        debugInfo += $" × DPI: {DPIManager.Instance.GetScaleFactor():F2}";
                    }
                    if (cube.usePerPCCorrection)
                    {
                        debugInfo += $" × PC: {cube.pcSpecificCorrection:F2}";
                    }
                    debugInfo += "\n";
                }
            }
        }

        debugInfo += $"\n=== SYSTEM ===\n";
        debugInfo += $"OS: {GetOSShortName(SystemInfo.operatingSystem)} | GPU: {GetGPUShortName(SystemInfo.graphicsDeviceName)} | VRAM: {SystemInfo.graphicsMemorySize}MB\n";
        
        debugInfo += $"\n=== CONTROLS ===\n";
        debugInfo += $"F1: Toggle Debug | Alt+Enter: Toggle Fullscreen\n";
    }
    
    string GetOSShortName(string fullOS)
    {
        if (fullOS.Contains("Windows 11")) return "Win11";
        if (fullOS.Contains("Windows 10")) return "Win10";
        if (fullOS.Contains("Windows")) return "Win";
        return "Other";
    }
    
    string GetGPUShortName(string fullGPU)
    {
        if (fullGPU.Contains("NVIDIA GeForce RTX")) return "RTX" + fullGPU.Substring(fullGPU.IndexOf("RTX") + 3, 4);
        if (fullGPU.Contains("NVIDIA GeForce GTX")) return "GTX" + fullGPU.Substring(fullGPU.IndexOf("GTX") + 3, 4);
        if (fullGPU.Contains("NVIDIA")) return "NVIDIA";
        if (fullGPU.Contains("AMD Radeon")) return "AMD";
        if (fullGPU.Contains("Intel")) return "Intel";
        return "Other";
    }
}