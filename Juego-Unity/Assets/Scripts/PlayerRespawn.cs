using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint; // Asignado por GameManager o PlayerPersistente

    private CharacterController controller;
    private PlayerVidas playerVidas;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerVidas = GetComponent<PlayerVidas>();
    }

    private void Start()
    {
        Debug.Log("PlayerRespawn listo. RespawnPoint actual: " +
                  (respawnPoint != null ? respawnPoint.name : "Ninguno"));
    }

    /// <summary>
    /// Hace respawn del jugador usando el respawnPoint actual.
    /// </summary>
    public void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("⚠️ Respawn llamado pero NO hay respawnPoint asignado.");
            return;
        }

        Debug.Log("🔄 Respawn ejecutado en: " + respawnPoint.position);

        // Teleport seguro
        if (controller != null)
        {
            controller.enabled = false;
            transform.position = respawnPoint.position;
            controller.enabled = true;
        }
        else
        {
            transform.position = respawnPoint.position;
        }
    }

    /// <summary>
    /// Asignado desde PlayerPersistente o GameManager.
    /// </summary>
    public void SetRespawnPoint(Transform point)
    {
        respawnPoint = point;
        Debug.Log("📌 Nuevo RespawnPoint asignado: " + point.name);
    }

    /// <summary>
    /// Cambia posición directamente a un punto específico (usado por teletransporte de mundos).
    /// </summary>
    public void TeletransportarAlInicio(Transform punto)
    {
        if (punto == null)
        {
            Debug.LogWarning("⚠️ TeletransportarAlInicio llamado sin punto valido.");
            return;
        }

        Debug.Log("🌀 Teletransportando al inicio: " + punto.name);

        if (controller != null)
        {
            controller.enabled = false;
            transform.position = punto.position;
            controller.enabled = true;
        }
        else
        {
            transform.position = punto.position;
        }

        // Actualizamos el respawn para esta escena
        respawnPoint = punto;
    }
}
