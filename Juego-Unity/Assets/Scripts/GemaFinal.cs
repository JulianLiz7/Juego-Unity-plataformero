using UnityEngine;

public class GemaFinal : MonoBehaviour
{
    [Header("Configuración Gema Final")]
    public ParticleSystem efectoRecoleccion;
    public AudioClip sonidoRecoleccion;

    private GameManagerPortal gameManagerPortal;

    private void Start()
    {
        gameManagerPortal = FindObjectOfType<GameManagerPortal>();
        Debug.Log("💎 Gema final inicializada en mundo Portal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("💎 GEMA FINAL RECOLECTADA - FIN DEL JUEGO");
            RecolectarGemaFinal();
        }
    }

    private void RecolectarGemaFinal()
    {
        // Efectos visuales y de sonido
        if (efectoRecoleccion != null)
        {
            Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);
            Debug.Log("✨ Efecto de recolección activado");
        }
        
        if (sonidoRecoleccion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position);
            Debug.Log("🔊 Sonido de recolección reproducido");
        }

        // Notificar al GameManager
        if (gameManagerPortal != null)
        {
            gameManagerPortal.GemaRecolectada();
        }

        // Desactivar gema
        gameObject.SetActive(false);
        Debug.Log("💎 Gema desactivada");
    }
}