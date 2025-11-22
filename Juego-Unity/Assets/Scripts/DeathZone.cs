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
            
            PlayerRespawn player = other.GetComponent<PlayerRespawn>();
            if (player != null)
            {
                player.Respawn();
            }
            else
            {
                // Fallback directo
                other.transform.position = respawnPoint.position;
            }
        }
    }
}