using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Vidas")]
    public TextMeshProUGUI vidasText;
    public Image[] corazones;
    public Sprite corazonLleno;
    public Sprite corazonVacio;

    [Header("Objetos Recolectados")]
    public TextMeshProUGUI objetosText;
    public TextMeshProUGUI objetosTotalText;

    [Header("Posición en Pantalla")]
    public RectTransform panelUI;
    public Corner corner = Corner.TopRight;

    private GameManager gameManager;
    private PlayerVidas playerVidas;

    public enum Corner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerVidas = FindObjectOfType<PlayerVidas>();
        
        if (playerVidas == null)
        {
            Debug.LogError("❌ No se encontró PlayerVidas en la escena");
        }

        AjustarPosicionUI();
        ActualizarUI();
        
        Debug.Log("🖥️ UI Manager iniciado correctamente");
    }

    private void AjustarPosicionUI()
    {
        if (panelUI == null) return;

        Vector2 nuevaAncla = Vector2.zero;
        Vector2 nuevoPivote = Vector2.zero;

        switch (corner)
        {
            case Corner.TopLeft:
                nuevaAncla = new Vector2(0, 1);
                nuevoPivote = new Vector2(0, 1);
                break;
            case Corner.TopRight:
                nuevaAncla = new Vector2(1, 1);
                nuevoPivote = new Vector2(1, 1);
                break;
            case Corner.BottomLeft:
                nuevaAncla = new Vector2(0, 0);
                nuevoPivote = new Vector2(0, 0);
                break;
            case Corner.BottomRight:
                nuevaAncla = new Vector2(1, 0);
                nuevoPivote = new Vector2(1, 0);
                break;
        }

        panelUI.anchorMin = nuevaAncla;
        panelUI.anchorMax = nuevaAncla;
        panelUI.pivot = nuevoPivote;
        panelUI.anchoredPosition = new Vector2(-20, -20); // Pequeño margen
    }

    public void ActualizarUI()
    {
        ActualizarVidas();
        ActualizarObjetos();
    }

    private void ActualizarVidas()
    {
        if (playerVidas == null) return;

        // Actualizar texto de vidas
        if (vidasText != null)
        {
            vidasText.text = $"Vidas: {playerVidas.vidasActuales}";
        }

        // Actualizar corazones
        if (corazones != null && corazones.Length > 0)
        {
            for (int i = 0; i < corazones.Length; i++)
            {
                if (corazones[i] != null)
                {
                    if (i < playerVidas.vidasActuales)
                    {
                        corazones[i].sprite = corazonLleno;
                    }
                    else
                    {
                        corazones[i].sprite = corazonVacio;
                    }
                }
            }
        }

        // Actualizar color y transparencia de corazones (para mejor feedback visual)
        ActualizarCorazones();
    }

    private void ActualizarObjetos()
    {
        if (gameManager == null) return;

        // ✅ CORREGIDO: Usar los métodos que filtran solo objetos que cuentan
        int objetosRecolectados = gameManager.GetObjetosRecolectadosReales();
        int totalObjetosQueCuentan = gameManager.GetTotalObjetosQueCuentan();

        // ✅ NUEVO: Ocultar completamente la UI de objetos si no hay objetos que contar
        bool mostrarUIObjetos = totalObjetosQueCuentan > 0;

        if (objetosText != null)
        {
            objetosText.gameObject.SetActive(mostrarUIObjetos);
            if (mostrarUIObjetos)
            {
                objetosText.text = $"Objetos: {objetosRecolectados}";
            }
        }

        if (objetosTotalText != null)
        {
            objetosTotalText.gameObject.SetActive(mostrarUIObjetos);
            if (mostrarUIObjetos)
            {
                objetosTotalText.text = $"{objetosRecolectados}/{totalObjetosQueCuentan}";
            }
        }

        // ✅ DEBUG: Mostrar información en consola para verificar
        if (Input.GetKeyDown(KeyCode.P)) // Presiona P para debug
        {
            Debug.Log($"📊 UI - Objetos recolectados: {objetosRecolectados}, Total que cuentan: {totalObjetosQueCuentan}, Mostrar UI: {mostrarUIObjetos}");
        }
    }

    // ✅ ELIMINADO: Ya no necesitamos este método porque usamos los del GameManager
    // private int GetObjetosRecolectados()

    private void Update()
    {
        // Actualizar UI cada frame (puedes optimizar esto con eventos)
        ActualizarUI();

        // Comando de testing para probar vidas
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (playerVidas != null)
            {
                playerVidas.PerderVida();
            }
        }

        // Comando de testing para debug de objetos
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (gameManager != null)
            {
                int objetosReales = gameManager.GetObjetosRecolectadosReales();
                int totalQueCuentan = gameManager.GetTotalObjetosQueCuentan();
                Debug.Log($"🔍 DEBUG OBJETOS - Recolectados: {objetosReales}/{totalQueCuentan}");
                
                // Mostrar qué mundos cuentan y cuáles no
                for (int i = 0; i < gameManager.mundos.Length; i++)
                {
                    bool cuenta = gameManager.mundos[i].contarEnEstadisticas;
                    Debug.Log($"🔍 Mundo {i}: {(cuenta ? "✅ CUENTA" : "❌ NO CUENTA")}");
                }
            }
        }
    }

    private void ActualizarCorazones()
    {
        if (playerVidas == null || corazones == null) return;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] != null)
            {
                Image imagenCorazon = corazones[i].GetComponent<Image>();
                if (imagenCorazon != null)
                {
                    if (i < playerVidas.vidasActuales)
                    {
                        // Corazón lleno - color normal
                        imagenCorazon.color = Color.white;
                    }
                    else
                    {
                        // Corazón vacío - gris semi-transparente
                        imagenCorazon.color = new Color(0.3f, 0.3f, 0.3f, 0.4f);
                    }
                }
            }
        }
    }

    public void ModoMundoFinal(bool esMundoFinal)
    {
        if (esMundoFinal)
        {
            Debug.Log("🖥️ Configurando UI para MUNDO FINAL");
            
            // Ocultar elementos de objetos recolectados
            if (objetosText != null) 
            {
                objetosText.gameObject.SetActive(false);
                Debug.Log("❌ Texto de objetos ocultado");
            }
            if (objetosTotalText != null) 
            {
                objetosTotalText.gameObject.SetActive(false);
                Debug.Log("❌ Contador total ocultado");
            }
            
            // Solo mostrar vidas
            if (vidasText != null) 
            {
                vidasText.gameObject.SetActive(true);
                Debug.Log("✅ Texto de vidas visible");
            }
            if (corazones != null) 
            {
                foreach (Image corazon in corazones)
                {
                    if (corazon != null) corazon.gameObject.SetActive(true);
                }
                Debug.Log("✅ Corazones visibles");
            }
        }
        else
        {
            // ✅ NUEVO: Restaurar UI normal cuando no es mundo final
            Debug.Log("🖥️ Restaurando UI normal");
            
            // Mostrar elementos basado en si hay objetos que contar
            int totalObjetosQueCuentan = gameManager != null ? gameManager.GetTotalObjetosQueCuentan() : 0;
            bool mostrarObjetos = totalObjetosQueCuentan > 0;

            if (objetosText != null) 
            {
                objetosText.gameObject.SetActive(mostrarObjetos);
                Debug.Log($"📊 UI Objetos: {mostrarObjetos} (Total que cuentan: {totalObjetosQueCuentan})");
            }
            if (objetosTotalText != null) 
            {
                objetosTotalText.gameObject.SetActive(mostrarObjetos);
            }
            
            // Asegurar que vidas siempre estén visibles
            if (vidasText != null) vidasText.gameObject.SetActive(true);
            if (corazones != null) 
            {
                foreach (Image corazon in corazones)
                {
                    if (corazon != null) corazon.gameObject.SetActive(true);
                }
            }
        }
    }
}