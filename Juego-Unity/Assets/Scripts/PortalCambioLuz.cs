using UnityEngine;

public class PortalCambioLuz : MonoBehaviour
{
    [Header("Configuración del Portal")]
    public Transform puntoDestino;
    public bool activarModoNoche = true; // ✅ Este portal activa modo noche
    
    [Header("Referencias")]
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("❌ No se encontró GameManager en la escena");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🌀 Portal de cambio de luz activado");
            
            // ✅ CAMBIAR ILUMINACIÓN ANTES DEL TELETRANSPORTE
            if (gameManager != null)
            {
                gameManager.CambiarIluminacion(activarModoNoche);
            }
            
            // ✅ TELETRANSPORTE INMEDIATO
            if (puntoDestino != null)
            {
                TeletransportarJugador(other.transform);
            }
            else
            {
                Debug.LogError("❌ No hay punto destino asignado en el portal");
            }
        }
    }

    private void TeletransportarJugador(Transform jugador)
    {
        // Desactivar CharacterController temporalmente si es necesario
        CharacterController cc = jugador.GetComponent<CharacterController>();
        bool ccEstadoOriginal = cc != null ? cc.enabled : true;
        
        if (cc != null) cc.enabled = false;

        // Aplicar teletransporte
        jugador.position = puntoDestino.position;
        jugador.rotation = puntoDestino.rotation;
        
        Debug.Log($"🚀 Teletransportado a: {puntoDestino.name} (Modo noche: {activarModoNoche})");

        // Reactivar CharacterController
        if (cc != null) 
        {
            StartCoroutine(ReactivarCC(cc, ccEstadoOriginal));
        }
    }

    private System.Collections.IEnumerator ReactivarCC(CharacterController cc, bool estadoOriginal)
    {
        yield return new WaitForEndOfFrame();
        cc.enabled = estadoOriginal;
    }
}