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

        // Obtener objetos recolectados (necesitamos hacer público este valor en GameManager)
        int objetosRecolectados = GetObjetosRecolectados();

        if (objetosText != null)
        {
            objetosText.text = $"Objetos: {objetosRecolectados}";
        }

        if (objetosTotalText != null)
        {
            objetosTotalText.text = $"{objetosRecolectados}/{gameManager.mundos.Length}";
        }
    }

    private int GetObjetosRecolectados()
    {
        // Necesitamos acceder a los objetos recolectados del GameManager
        return gameManager.objetosRecolectados;
    }

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
}