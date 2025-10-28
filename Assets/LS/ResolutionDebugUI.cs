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
        debugText.fontSize = 14;
        debugText.color = Color.white;
        
        // Posiziona il testo in alto a sinistra
        RectTransform rectTransform = debugText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(10, -10);
        rectTransform.sizeDelta = new Vector2(400, 300);
        
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
        debugInfo = "=== RESOLUTION DEBUG INFO ===\n";
        debugInfo += $"Screen Resolution: {Screen.width}x{Screen.height}\n";
        debugInfo += $"DPI: {Screen.dpi}\n";
        debugInfo += $"Fullscreen Mode: {Screen.fullScreenMode}\n";
        debugInfo += $"Native Resolution: {Display.main.systemWidth}x{Display.main.systemHeight}\n";
        debugInfo += $"Refresh Rate: {Screen.currentResolution.refreshRate}Hz\n";
        
        // Info DPI Manager se disponibile
        if (DPIManager.Instance != null)
        {
            debugInfo += "\n=== DPI MANAGER ===\n";
            debugInfo += $"Current DPI: {DPIManager.Instance.currentDPI}\n";
            debugInfo += $"DPI Scale: {DPIManager.Instance.dpiScale:F2}\n";
            debugInfo += $"Scale Factor: {DPIManager.Instance.GetScaleFactor():F2}\n";
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
        
        debugInfo += $"\n=== SYSTEM INFO ===\n";
        debugInfo += $"OS: {SystemInfo.operatingSystem}\n";
        debugInfo += $"GPU: {SystemInfo.graphicsDeviceName}\n";
        debugInfo += $"GPU Memory: {SystemInfo.graphicsMemorySize}MB\n";
        
        debugInfo += $"\n=== CONTROLS ===\n";
        debugInfo += $"Press {toggleKey} to toggle this debug info\n";
        debugInfo += $"Press Alt+Enter to toggle fullscreen\n";
    }
}