using UnityEngine;

public class ColorCuerpoManager : MonoBehaviour
{
    [Header("ELEMENTO FUEGO - Partes específicas")]
    public Renderer manoDer;
    public Renderer antebrazoDer;
    public Renderer codoDer;
    public Renderer hombroDer;
    public Renderer piernaDer;
    public Renderer llamaPequenaDer;
    public Renderer llamaGrandeDer;

    [Header("ELEMENTO AGUA - Partes específicas")]
    public Renderer manoIzq;
    public Renderer antebrazoIzq;
    public Renderer codoIzq;
    public Renderer hombroIzq;
    public Renderer piernaIzq;
    public Renderer musloIzq;
    public Renderer colaAgua;

    [Header("ELEMENTO TIERRA - Partes específicas")]
    public Renderer musloDer;
    public Renderer pecho;
    public Renderer cuello;

    [Header("ELEMENTO AIRE - Partes específicas")]
    public Renderer corazon;
    public Renderer cara;
    public Renderer cabello;
    public Renderer ojoDer;
    public Renderer ojoIzq;

    [Header("Colores de Elementos")]
    public Color fuegoColor = Color.red;
    public Color aguaColor = new Color(0f, 0.5f, 1f); // Azul agua
    public Color tierraColor = new Color(0.6f, 0.4f, 0.2f); // Marrón
    public Color aireColor = Color.white;
    public Color colorOjos = Color.black; // Color negro para los ojos

    private int nivelColor = 0;
    private static ColorCuerpoManager instance;
    private static bool primeraEjecucion = true;

    private const string PlayerPrefsNivelColorKey = "NivelColorActual";

    private void Awake()
    {
        // ✅ PATRÓN SINGLETON - Mantener una sola instancia entre escenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // ✅ RESETEAR en la PRIMERA ejecución del juego
            if (primeraEjecucion)
            {
                nivelColor = 0; // Empieza en blanco y negro
                primeraEjecucion = false;
                Debug.Log("🎨 ColorManager - PRIMERA EJECUCIÓN, reset a nivel 0");
                
                // ✅ LIMPIAR PlayerPrefs al inicio del juego
                PlayerPrefs.DeleteKey(PlayerPrefsNivelColorKey);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log("🎨 ColorManager - Ejecución continua, manteniendo estado");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // ✅ SIEMPRE empezar en nivel 0 en la escena inicial
        if (currentScene == "MundoInicial" || currentScene == "Main" || currentScene == "SampleScene")
        {
            Debug.Log("🎮 Escena inicial detectada - Forzando nivel color 0");
            nivelColor = 0;
            ResetearColores();
        }
        else
        {
            // ✅ Para otras escenas, cargar el estado guardado
            LoadColorState();
        }
        
        Debug.Log($"🎨 ColorManager iniciado - Nivel actual: {nivelColor}, Escena: {currentScene}");
    }

    public void SaveColorState()
    {
        PlayerPrefs.SetInt(PlayerPrefsNivelColorKey, nivelColor);
        PlayerPrefs.Save();
        Debug.Log($"💾 NivelColor guardado: {nivelColor}");
    }

    public void LoadColorState()
    {
        int savedNivel = PlayerPrefs.GetInt(PlayerPrefsNivelColorKey, 0);

        if (savedNivel > 0)
        {
            Debug.Log($"💽 Restaurando NivelColor guardado: {savedNivel}");
            nivelColor = savedNivel;
            AplicarNivelColor(savedNivel);
        }
        else
        {
            Debug.Log("💽 No hay nivel de color guardado - Iniciando en 0");
            nivelColor = 0;
            ResetearColores();
        }
    }

    // Nuevo método para aplicar color directamente sin incrementar nivelColor
    public void AplicarNivelColor(int nivel)
    {
        nivelColor = nivel;
        Debug.Log($"🎨 Aplicando nivel de color: {nivel}");
        
        switch (nivel)
        {
            case 1:
                ActivarFuego();
                break;
            case 2:
                ActivarAgua();
                break;
            case 3:
                ActivarTierra();
                break;
            case 4:
                ActivarAire();
                break;
            default:
                ResetearColores();
                break;
        }
        
        // ✅ GUARDAR automáticamente cuando se aplica un nivel
        SaveColorState();
    }

    public void AvanzarNivelColor()
    {
        nivelColor++;
        Debug.Log($"🎨 Avanzando a nivel de color: {nivelColor}");
        
        switch (nivelColor)
        {
            case 1:
                ActivarFuego();
                break;
            case 2:
                ActivarAgua();
                break;
            case 3:
                ActivarTierra();
                break;
            case 4:
                ActivarAire();
                break;
            default:
                Debug.LogWarning($"🎨 Nivel de color máximo alcanzado: {nivelColor}");
                break;
        }
        
        // ✅ GUARDAR automáticamente cuando se avanza
        SaveColorState();
    }

    public void ActivarFuego()
    {
        Debug.Log("🔥 Activando ELEMENTO FUEGO");
        Pintar(manoDer, fuegoColor);
        Pintar(antebrazoDer, fuegoColor);
        Pintar(codoDer, fuegoColor);
        Pintar(hombroDer, fuegoColor);
        Pintar(piernaDer, fuegoColor);
        Pintar(llamaPequenaDer, fuegoColor);
        Pintar(llamaGrandeDer, fuegoColor);
    }

    public void ActivarAgua()
    {
        Debug.Log("💧 Activando ELEMENTO AGUA");
        Pintar(manoIzq, aguaColor);
        Pintar(antebrazoIzq, aguaColor);
        Pintar(codoIzq, aguaColor);
        Pintar(hombroIzq, aguaColor);
        Pintar(piernaIzq, aguaColor);
        Pintar(musloIzq, aguaColor);
        Pintar(colaAgua, aguaColor);
    }

    public void ActivarTierra()
    {
        Debug.Log("🌱 Activando ELEMENTO TIERRA");
        Pintar(musloDer, tierraColor);
        Pintar(pecho, tierraColor);
        Pintar(cuello, tierraColor);
    }

    public void ActivarAire()
    {
        Debug.Log("💨 Activando ELEMENTO AIRE");
        Pintar(corazon, aireColor);
        Pintar(cara, aireColor);
        Pintar(cabello, aireColor);
        
        // ✅ OJOS EN NEGRO (no se pintan del color aire)
        Pintar(ojoDer, colorOjos);
        Pintar(ojoIzq, colorOjos);
    }

    private void Pintar(Renderer rend, Color color)
    {
        if (rend != null)
        {
            foreach (var mat in rend.materials)
            {
                mat.color = color;
                mat.EnableKeyword("_EMISSION");
                
                float intensidad = 0.3f;
                if (color == fuegoColor) intensidad = 0.6f;
                if (color == aguaColor) intensidad = 0.4f;
                if (color == tierraColor) intensidad = 0.2f;
                if (color == aireColor) intensidad = 0.8f;
                if (color == colorOjos) intensidad = 0f; // ✅ Ojos sin brillo
                
                mat.SetColor("_EmissionColor", color * intensidad);
            }
        }
        else
        {
            Debug.LogWarning("🎨 Renderer no asignado");
        }
    }

    public int GetNivelColorActual()
    {
        return nivelColor;
    }

    public void ResetearColores()
    {
        nivelColor = 0;
        Debug.Log("🔄 Todos los colores reseteados a estado inicial");
        
        // ✅ LIMPIAR PlayerPrefs cuando se resetea
        PlayerPrefs.DeleteKey(PlayerPrefsNivelColorKey);
        PlayerPrefs.Save();
    }

    // ✅ MÉTODO PARA FORZAR RESET COMPLETO (usar en GameManager al iniciar)
    public void IniciarNuevaPartida()
    {
        Debug.Log("🎮 INICIANDO NUEVA PARTIDA - Reset completo de colores");
        nivelColor = 0;
        ResetearColores();
    }

    // ⚠️ MÉTODO DE COMPATIBILIDAD TEMPORAL - ELIMINAR DESPUÉS
    public void ActivaFruegoDerecha()
    {
        Debug.Log("Usando método de compatibilidad - activando fuego");
        ActivarFuego();
    }
}