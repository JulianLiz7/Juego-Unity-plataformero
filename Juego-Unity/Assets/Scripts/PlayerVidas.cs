using UnityEngine;
using System.Collections;

public class PlayerVidas : MonoBehaviour
{
    [Header("Configuración de Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales = 3;
    public float tiempoInvencibilidad = 2f;

    [Header("Referencias")]
    public UIManager uiManager;
    public PlayerRespawn playerRespawn;

    private bool esInvencible = false;
    private Renderer playerRenderer;
    private Color colorOriginal;

    public event System.Action OnVidaPerdida;
    public event System.Action OnGameOver;

    private void Start()
    {
        playerRespawn = GetComponent<PlayerRespawn>();
        playerRenderer = GetComponent<Renderer>();
        
        if (playerRenderer != null)
        {
            colorOriginal = playerRenderer.material.color;
        }

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        Debug.Log($"❤️ Sistema de vidas iniciado: {vidasActuales}/{vidasMaximas} vidas");
    }

public void PerderVida(bool esMuertePorCaida = false)
{
    // Permitir perder vida si no es invencible, o si la muerte es por caída debe pasar siempre
    if (esInvencible && !esMuertePorCaida) return;

    vidasActuales--;
    Debug.Log($"💔 Vida perdida. Vidas restantes: {vidasActuales}");

    // Notificar a UI
    OnVidaPerdida?.Invoke();
    if (uiManager != null)
    {
        uiManager.ActualizarUI();
    }

    if (vidasActuales <= 0)
    {
        GameOver();
    }
    else
    {
        // SOLO activar invencibilidad si NO es muerte por caída
        if (!esMuertePorCaida)
        {
            StartCoroutine(ActivarInvencibilidad());
        }
        
        // Respawn
        if (playerRespawn != null)
        {
            playerRespawn.Respawn();
        }
    }
}

    private IEnumerator ActivarInvencibilidad()
    {
        esInvencible = true;
        Debug.Log("🛡️ Invencibilidad activada");

        float tiempoTranscurrido = 0f;
        bool visible = true;

        // Efecto de parpadeo
        while (tiempoTranscurrido < tiempoInvencibilidad)
        {
            if (playerRenderer != null)
            {
                playerRenderer.enabled = visible;
                visible = !visible;
            }

            yield return new WaitForSeconds(0.1f);
            tiempoTranscurrido += 0.1f;
        }

        // Restaurar visibilidad normal
        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
            playerRenderer.material.color = colorOriginal;
        }

        esInvencible = false;
        Debug.Log("🛡️ Invencibilidad desactivada");
    }

    private void GameOver()
    {
        Debug.Log("💀 GAME OVER - No quedan vidas");
        
        OnGameOver?.Invoke();
        
        // Aquí puedes agregar lógica de game over:
        // - Mostrar pantalla de game over
        // - Reiniciar nivel
        // - Volver al menú principal
        
        // Por ahora, simplemente recargamos la escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GanarVida()
    {
        if (vidasActuales < vidasMaximas)
        {
            vidasActuales++;
            Debug.Log($"➕ Vida ganada. Vidas totales: {vidasActuales}");
            
            if (uiManager != null)
            {
                uiManager.ActualizarUI();
            }
        }
    }

    public void RestablecerVidas()
    {
        vidasActuales = vidasMaximas;
        Debug.Log($"🔄 Vidas restablecidas: {vidasActuales}");
        
        if (uiManager != null)
        {
            uiManager.ActualizarUI();
        }
    }

    // Comandos de testing
    private void Update()
    {
        // Presiona F1 para perder vida (testing)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PerderVida();
        }
        
        // Presiona F2 para ganar vida (testing)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GanarVida();
        }
    }
}