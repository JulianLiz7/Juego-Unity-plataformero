using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI mensajeText;

    [Header("Sistema de Mundos/Puentes Secuencial")]
    public MundoData[] mundos;

    [Header("Sistema de Color del Personaje")]
    public ColorCuerpoManager colorManager;

    [Header("Configuración Secuencial")]
    public bool sistemaSecuencial = true;
    
    private int objetosRecolectados = 0;
    private int mundoActual = 0;
    
    private void Start()
    {
        AsegurarTextoOculto();
        InicializarMundosSecuenciales();
    }
    
    private void AsegurarTextoOculto()
    {
        if (mensajeText != null)
        {
            mensajeText.text = "";
            mensajeText.gameObject.SetActive(false);
        }
    }

    private void InicializarMundosSecuenciales()
    {
        for (int i = 0; i < mundos.Length; i++)
        {
            // Solo el primer recolectable activo al inicio
            if (mundos[i].recolectable != null)
            {
                mundos[i].recolectable.SetActive(i == 0);
            }
            
            // Todos los mundos desactivados al inicio
            if (mundos[i].mundo != null)
                mundos[i].mundo.SetActive(false);
            
            // Todos los puentes desactivados al inicio
            if (mundos[i].puente != null)
                mundos[i].puente.SetActive(false);
        }
    }
    
    public void RecolectarObjeto(int numeroMundo)
    {
        // Verificar si es el mundo correcto en sistema secuencial
        if (sistemaSecuencial && numeroMundo != mundoActual)
        {
            Debug.Log($"⚠️ Recolectable {numeroMundo} ignorado. Se esperaba el {mundoActual}");
            return;
        }

        if (numeroMundo >= 0 && numeroMundo < mundos.Length)
        {
            StartCoroutine(ProcesarRecoleccion(numeroMundo));
        }
        else
        {
            Debug.LogError("Número de mundo inválido: " + numeroMundo);
        }
    }

    IEnumerator ProcesarRecoleccion(int numeroMundo)
    {
        objetosRecolectados++;
        MundoData mundoRecolectado = mundos[numeroMundo];

        // DESACTIVAR EL RECOLECTABLE ACTUAL
        if (mundoRecolectado.recolectable != null)
            mundoRecolectado.recolectable.SetActive(false);

        // ACTIVAR EL MUNDO Y PUENTE
        if (mundoRecolectado.mundo != null)
            mundoRecolectado.mundo.SetActive(true);
        
        if (mundoRecolectado.puente != null)
            mundoRecolectado.puente.SetActive(true);

        // ACTIVAR COLOR CORRESPONDIENTE - ✅ ACTUALIZADO
        if (colorManager != null)
        {
            colorManager.AvanzarNivelColor(); // ✅ MÉTODO CORRECTO
        }

        // MENSAJES
        if (mensajeText != null)
        {
            mensajeText.text = $"Objeto {numeroMundo + 1} recolectado";
            mensajeText.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(3f);
        
        if (mensajeText != null)
        {
            mensajeText.text = $"Mundo {numeroMundo + 1} activado";
            if (numeroMundo < mundos.Length - 1)
            {
                mensajeText.text += $"\nBusca el objeto {numeroMundo + 2}";
            }
        }
        
        yield return new WaitForSeconds(3f);
        
        // ACTIVAR SIGUIENTE RECOLECTABLE
        if (sistemaSecuencial && numeroMundo < mundos.Length - 1)
        {
            mundoActual = numeroMundo + 1;
            if (mundos[mundoActual].recolectable != null)
            {
                mundos[mundoActual].recolectable.SetActive(true);
                Debug.Log($"✅ Recolectable {mundoActual + 1} activado");
            }
        }

        // TELETRANSPORTE si está configurado
        if (mundoRecolectado.puntoTeletransporte != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerRespawn playerRespawn = player.GetComponent<PlayerRespawn>();
                if (playerRespawn != null)
                {
                    playerRespawn.TeletransportarAlInicio(mundoRecolectado.puntoTeletransporte);
                }
            }
        }
        
        AsegurarTextoOculto();
    }

    // Método para forzar activación de un mundo (para testing)
    public void ForzarActivacionMundo(int numeroMundo)
    {
        if (numeroMundo >= 0 && numeroMundo < mundos.Length)
        {
            mundoActual = numeroMundo;
            if (mundos[numeroMundo].recolectable != null)
            {
                mundos[numeroMundo].recolectable.SetActive(true);
            }
        }
    }

    // Comandos de testing
    private void Update()
    {
        // Presiona 1, 2, 3, 4 para activar mundos manualmente
        if (Input.GetKeyDown(KeyCode.Alpha1)) ForzarActivacionMundo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ForzarActivacionMundo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ForzarActivacionMundo(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ForzarActivacionMundo(3);
    }
}

// CLASE PARA ORGANIZAR LOS DATOS DE CADA MUNDO
[System.Serializable]
public class MundoData
{
    [Header("Configuración Mundo")]
    public GameObject mundo;          // El mundo completo a activar
    public GameObject puente;         // El puente específico
    public GameObject recolectable;   // El objeto recolectable de ESTE mundo
    public Transform puntoTeletransporte; // Donde reaparece el jugador
    public string nombreMundo;        // Nombre para referencia
}