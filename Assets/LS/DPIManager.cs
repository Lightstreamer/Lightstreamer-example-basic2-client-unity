using UnityEngine;

public class DPIManager : MonoBehaviour
{
    [Header("Riferimenti Base")]
    public float baseDPI = 96f; // DPI di riferimento (Windows standard)
    public Vector2 baseResolution = new Vector2(1920f, 1080f); // Risoluzione di riferimento
    
    [Header("Correzioni Manuali")]
    public bool useManualCorrection = false;
    public float manualScaleCorrection = 1.0f;
    
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
        // Aggiorna risoluzione corrente
        currentResolution = new Vector2(Screen.width, Screen.height);
        
        // Calcola il fattore di scala basato sulla risoluzione
        float resolutionScale = Mathf.Min(
            currentResolution.x / baseResolution.x,
            currentResolution.y / baseResolution.y
        );
        
        // Usa SOLO il DPI scale come fattore principale
        // Il resolution scale viene usato solo se è significativamente diverso
        float finalScale = dpiScale;
        
        // Se la risoluzione è molto diversa da quella base, applica una correzione
        float resolutionDifference = Mathf.Abs(resolutionScale - 1.0f);
        if (resolutionDifference > 0.3f)
        {
            // Applica una correzione più leggera basata sulla risoluzione
            finalScale = dpiScale * (1.0f + (resolutionScale - 1.0f) * 0.3f);
        }
        
        // Limita il range per evitare scale estreme
        finalScale = Mathf.Clamp(finalScale, 0.7f, 1.5f);
        
        // Applica correzione manuale se abilitata
        if (useManualCorrection)
        {
            finalScale *= manualScaleCorrection;
            Debug.Log($"Manual correction applied: {manualScaleCorrection:F2}, Final adjusted scale: {finalScale:F2}");
        }
        
        Debug.Log($"Scale calculation - DPI Scale: {dpiScale:F2}, Resolution Scale: {resolutionScale:F2}, Final Scale: {finalScale:F2}");
        
        return finalScale;
    }
    
    /// <summary>
    /// Restituisce il fattore di scala normalizzato (per debug)
    /// </summary>
    public float GetNormalizedScaleFactor()
    {
        return GetScaleFactor() / 1f; // Base scale factor = 1
    }
}