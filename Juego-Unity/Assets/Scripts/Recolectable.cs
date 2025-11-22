using UnityEngine;

public class Recolectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscar el GameManager en la escena
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.RecolectarPrimerObjeto();
            }
            
            Destroy(gameObject);
        }
    }
}