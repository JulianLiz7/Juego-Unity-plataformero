using UnityEngine;
using TMPro;

public class GameManagerPortal : MonoBehaviour
{
    [Header("UI Mundo Portal")]
    public UIManager uiManager;
    
    [Header("Gema Final")]
    public GameObject gemaFinal;
    
    private PlayerVidas playerVidas;

    private void Start()
    {
        Debug.Log("🌌 INICIANDO MUNDO PORTAL - MODO FINAL");
        
        // Recuperar vidas guardadas
        playerVidas = FindObjectOfType<PlayerVidas>();
        if (playerVidas != null)
        {
            int vidasGuardadas = PlayerPrefs.GetInt("VidasActuales", 3);
            playerVidas.vidasActuales = vidasGuardadas;
            Debug.Log($"❤️ Vidas recuperadas: {vidasGuardadas}");
        }

        // Configurar UI para mundo final
        if (uiManager != null)
        {
            uiManager.ModoMundoFinal(true);
            Debug.Log("🖥️ UI configurada en modo mundo final");
        }
        
        // Activar gema final
        if (gemaFinal != null)
        {
            gemaFinal.SetActive(true);
            Debug.Log("💎 Gema final activada");
        }
    }

    // Método para cuando se recolecta la gema final
    public void GemaRecolectada()
    {
        Debug.Log("🎉 GEMA FINAL RECOLECTADA - FIN DEL JUEGO");
        
        // Aquí puedes agregar efectos adicionales antes de los créditos
        Invoke("CargarCreditos", 2f);
    }

    private void CargarCreditos()
    {
        Debug.Log("🎬 Cargando escena de créditos...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Creditos");
    }
}