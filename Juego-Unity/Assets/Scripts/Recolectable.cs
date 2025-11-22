using UnityEngine;

public class Recolectable : MonoBehaviour
{
    [Header("Configuración del Recolectable")]
    public int numeroMundo = 0;  // 0 = primer mundo, 1 = segundo, etc.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.RecolectarObjeto(numeroMundo);
            }
            
            Destroy(gameObject);
        }
    }
}