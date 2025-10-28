using UnityEngine;

public class DPIManager : MonoBehaviour
{
    [Header("Riferimenti Base")]
    public float baseDPI = 96f; // DPI di riferimento (Windows standard)
    public Vector2 baseResolution = new Vector2(1920f, 1080f); // Risoluzione di riferimento
    
    [Header("Debug Info")]
    public float currentDPI;
    public float dpiScale;
    public Vector2 currentResolution;
    
    private static DPIManager instance;
    public static DPIManager Instance { get { return instance; } }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDPI();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeDPI()
    {
        // Ottieni DPI corrente
        currentDPI = Screen.dpi;
        
        // Se DPI non è disponibile, usa valore di default
        if (currentDPI <= 0)
        {
            currentDPI = baseDPI;
        }
        
        // Calcola fattore di scala DPI
        dpiScale = currentDPI / baseDPI;
        
        // Ottieni risoluzione corrente
        currentResolution = new Vector2(Screen.width, Screen.height);
        
        Debug.Log($"DPI Manager Initialized:");
        Debug.Log($"- Current DPI: {currentDPI}");
        Debug.Log($"- DPI Scale: {dpiScale}");
        Debug.Log($"- Current Resolution: {currentResolution}");
        Debug.Log($"- Base Resolution: {baseResolution}");
    }
    
    /// <summary>
    /// Restituisce il fattore di scala corretto per gli oggetti 3D
    /// basato su DPI e risoluzione
    /// </summary>
    public float GetScaleFactor()
    {
        // Calcola il fattore di scala basato sulla risoluzione
        float resolutionScale = Mathf.Min(
            currentResolution.x / baseResolution.x,
            currentResolution.y / baseResolution.y
        );
        
        // Combina DPI e risoluzione per un fattore di scala finale
        float finalScale = (dpiScale + resolutionScale) / 2f;
        
        // Limita il range per evitare scale estreme
        return Mathf.Clamp(finalScale, 0.5f, 2.0f);
    }
    
    /// <summary>
    /// Restituisce il fattore di scala normalizzato (per debug)
    /// </summary>
    public float GetNormalizedScaleFactor()
    {
        return GetScaleFactor() / 1f; // Base scale factor = 1
    }
}