using UnityEngine;

/// <summary>
/// Script per gestire la risoluzione dell'applicazione in modo adattivo
/// per evitare problemi di scalatura su diversi PC
/// </summary>
public class ResolutionManager : MonoBehaviour
{
    [Header("Impostazioni Risoluzione")]
    public bool autoDetectBestResolution = true;
    public bool maintainAspectRatio = true;
    public Vector2 preferredResolution = new Vector2(1920, 1080);
    public Vector2 minimumResolution = new Vector2(1024, 768);
    
    [Header("Modalità Fullscreen")]
    public FullScreenMode fullscreenMode = FullScreenMode.FullScreenWindow;
    public bool allowFullscreenToggle = true;
    
    [Header("Debug Info")]
    public Vector2 currentResolution;
    public Vector2 nativeResolution;
    public float aspectRatio;
    
    void Start()
    {
        InitializeResolution();
    }
    
    void InitializeResolution()
    {
        // Ottieni risoluzione nativa del monitor
        nativeResolution = new Vector2(Display.main.systemWidth, Display.main.systemHeight);
        aspectRatio = nativeResolution.x / nativeResolution.y;
        
        Debug.Log($"Native Resolution: {nativeResolution}, Aspect Ratio: {aspectRatio:F2}");
        
        if (autoDetectBestResolution)
        {
            SetBestResolution();
        }
        else
        {
            SetCustomResolution();
        }
        
        // Permetti di cambiare fullscreen con Alt+Enter
        if (allowFullscreenToggle)
        {
            StartCoroutine(CheckFullscreenToggle());
        }
    }
    
    void SetBestResolution()
    {
        Vector2 targetResolution = preferredResolution;
        
        // Se la risoluzione preferita è maggiore di quella nativa, usa quella nativa
        if (preferredResolution.x > nativeResolution.x || preferredResolution.y > nativeResolution.y)
        {
            targetResolution = nativeResolution;
        }
        
        // Assicurati che non sia sotto la risoluzione minima
        if (targetResolution.x < minimumResolution.x || targetResolution.y < minimumResolution.y)
        {
            targetResolution = minimumResolution;
        }
        
        // Se maintainAspectRatio è true, calcola la risoluzione mantenendo l'aspect ratio
        if (maintainAspectRatio)
        {
            targetResolution = CalculateAspectRatioResolution(targetResolution);
        }
        
        currentResolution = targetResolution;
        Screen.SetResolution((int)targetResolution.x, (int)targetResolution.y, fullscreenMode);
        
        Debug.Log($"Resolution set to: {targetResolution} (Mode: {fullscreenMode})");
    }
    
    void SetCustomResolution()
    {
        currentResolution = preferredResolution;
        Screen.SetResolution((int)preferredResolution.x, (int)preferredResolution.y, fullscreenMode);
        
        Debug.Log($"Custom resolution set to: {preferredResolution} (Mode: {fullscreenMode})");
    }
    
    Vector2 CalculateAspectRatioResolution(Vector2 targetRes)
    {
        float targetAspect = targetRes.x / targetRes.y;
        
        // Se l'aspect ratio è molto diverso da quello nativo, aggiusta
        if (Mathf.Abs(targetAspect - aspectRatio) > 0.1f)
        {
            // Mantieni la larghezza e aggiusta l'altezza
            float newHeight = targetRes.x / aspectRatio;
            return new Vector2(targetRes.x, newHeight);
        }
        
        return targetRes;
    }
    
    System.Collections.IEnumerator CheckFullscreenToggle()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Return))
            {
                ToggleFullscreen();
            }
            yield return null;
        }
    }
    
    public void ToggleFullscreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Screen.SetResolution((int)nativeResolution.x, (int)nativeResolution.y, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution((int)preferredResolution.x, (int)preferredResolution.y, FullScreenMode.Windowed);
        }
        
        currentResolution = new Vector2(Screen.width, Screen.height);
        Debug.Log($"Toggled fullscreen. New resolution: {currentResolution}");
    }
    
    /// <summary>
    /// Imposta una risoluzione specifica (utile per test)
    /// </summary>
    public void SetSpecificResolution(int width, int height, FullScreenMode mode)
    {
        Screen.SetResolution(width, height, mode);
        currentResolution = new Vector2(width, height);
        Debug.Log($"Set specific resolution: {width}x{height} (Mode: {mode})");
    }
    
    void Update()
    {
        // Aggiorna info di debug
        if (currentResolution.x != Screen.width || currentResolution.y != Screen.height)
        {
            currentResolution = new Vector2(Screen.width, Screen.height);
        }
    }
}