using UnityEngine;

public class DanioLava : MonoBehaviour
{
    [Header("Configuración Lava")]
    public bool usarCollisionEnLugarDeTrigger = true;
    
    private void Start()
    {
        // Si estamos usando collision, desactivar trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null && usarCollisionEnLugarDeTrigger)
        {
            collider.isTrigger = false;
        }
    }

    // Para cuando el Character Controller colisiona (NO trigger)
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lava detectó jugador por ControllerColliderHit");
            MatarJugador(hit.gameObject);
        }
    }

    // Para colliders normales (backup)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lava detectó jugador por Collision");
            MatarJugador(collision.gameObject);
        }
    }

    // Para triggers (si decides usarlos)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Lava detectó jugador por Trigger");
            MatarJugador(other.gameObject);
        }
    }

    private void MatarJugador(GameObject jugador)
    {
        PlayerRespawn playerRespawn = jugador.GetComponent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.Respawn();
            Debug.Log("✅ Jugador murió en la lava - Respawn ejecutado");
        }
        else
        {
            Debug.LogError("❌ No se encontró PlayerRespawn en el jugador");
        }
    }
}