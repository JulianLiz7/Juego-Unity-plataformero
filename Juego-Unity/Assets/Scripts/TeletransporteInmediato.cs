using UnityEngine;
using System.Collections;

public class TeletransporteInmediato : MonoBehaviour
{
    [Header("Configuración del Teletransporte")]
    public Transform puntoDestino;
    public GameObject mundoDestino;
    public string tagMundos = "Mundo";

    private bool teletransportando = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !teletransportando)
        {
            Debug.Log("✅ Jugador entró en el trigger del teletransporte");
            StartCoroutine(EjecutarTeletransporteSeguro(other.transform));
        }
    }

    private IEnumerator EjecutarTeletransporteSeguro(Transform jugador)
    {
        teletransportando = true;

        // 1. Primero manejar los mundos
        if (mundoDestino != null)
        {
            GameObject[] todosLosMundos = GameObject.FindGameObjectsWithTag(tagMundos);
            foreach (GameObject mundo in todosLosMundos)
            {
                if (mundo != mundoDestino) 
                    mundo.SetActive(false);
            }
            mundoDestino.SetActive(true);
            Debug.Log($"🌍 Mundo activado: {mundoDestino.name}");
        }

        // 2. Esperar un frame para que Unity procese los cambios
        yield return new WaitForEndOfFrame();

        // 3. Aplicar teletransporte SIN desactivar CharacterController
        jugador.position = puntoDestino.position;
        jugador.rotation = puntoDestino.rotation;
        Debug.Log($"📍 Teletransportado a: {puntoDestino.position}");

        // 4. Forzar una actualización del CharacterController
        CharacterController cc = jugador.GetComponent<CharacterController>();
        if (cc != null)
        {
            // Mover ligeramente para "reactivar" el controller
            cc.Move(Vector3.zero);
            Debug.Log("🔄 CharacterController actualizado con movimiento cero");
        }

        teletransportando = false;
        Debug.Log("🎉 Teletransporte completado exitosamente");
    }
}