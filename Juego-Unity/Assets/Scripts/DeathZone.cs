using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint;

    private void Start()
    {
        // Hacer el collider más grueso para asegurar detección
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.size = new Vector3(50f, 5f, 50f); // Más grueso en Y
        }
    }

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Player entró en DeathZone");
        
        PlayerVidas playerVidas = other.GetComponent<PlayerVidas>();
        if (playerVidas != null)
        {
            // ✅ Pasar true para indicar que es muerte por caída
            playerVidas.PerderVida(true);
        }
        else
        {
            // Fallback al sistema antiguo
            PlayerRespawn player = other.GetComponent<PlayerRespawn>();
            if (player != null)
            {
                player.Respawn();
            }
        }
    }
}

}