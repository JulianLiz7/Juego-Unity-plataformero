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
    
    [Header("Estado del Juego")]
    public int mundoActual = 0; // ✅ AHORA ES PÚBLICO
    
    public int objetosRecolectados = 0;
    
   private void Start()
{
    // ✅ RESETEAR COLOR MANAGER al iniciar nueva partida
    ColorCuerpoManager colorManager = FindObjectOfType<ColorCuerpoManager>();
    if (colorManager != null)
    {
        colorManager.IniciarNuevaPartida();
    }

    AsegurarTextoOculto();
    InicializarMundosSecuenciales();
    
    Debug.Log("🎮 GAME MANAGER INICIADO - NUEVA PARTIDA");
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
        Debug.Log("🌍 INICIALIZANDO MUNDOS SECUENCIALES");
        
        for (int i = 0; i < mundos.Length; i++)
        {
            // Solo el primer recolectable activo al inicio
            if (mundos[i].recolectable != null)
            {
                if (i == 0)
                {
                    mundos[i].recolectable.SetActive(true);
                    Debug.Log($"✅ Mundo {i}: Recolectable ACTIVADO - {mundos[i].recolectable.name}");
                }
                else
                {
                    mundos[i].recolectable.SetActive(false);
                    Debug.Log($"❌ Mundo {i}: Recolectable DESACTIVADO - {mundos[i].recolectable.name}");
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Mundo {i}: No tiene recolectable asignado");
            }
            
            // Todos los mundos desactivados al inicio
            if (mundos[i].mundo != null)
            {
                mundos[i].mundo.SetActive(false);
                Debug.Log($"❌ Mundo {i}: Mundo DESACTIVADO - {mundos[i].mundo.name}");
            }
            
            // Todos los puentes desactivados al inicio
            if (mundos[i].puente != null)
            {
                mundos[i].puente.SetActive(false);
                Debug.Log($"❌ Mundo {i}: Puente DESACTIVADO - {mundos[i].puente.name}");
            }

            // Verificar puntos de respawn
            if (mundos[i].puntoTeletransporte != null)
            {
                Debug.Log($"📍 Mundo {i}: Punto respawn ASIGNADO - {mundos[i].puntoTeletransporte.name}");
            }
            else
            {
                Debug.LogError($"❌ Mundo {i}: NO tiene punto de respawn asignado!");
            }
        }
        
        Debug.Log("🎯 INICIALIZACIÓN DE MUNDOS COMPLETADA");
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
            Debug.Log($"🎮 INICIANDO RECOLECCIÓN: Mundo {numeroMundo}");
            StartCoroutine(ProcesarRecoleccion(numeroMundo));
        }
        else
        {
            Debug.LogError("❌ Número de mundo inválido: " + numeroMundo);
        }
    }

    IEnumerator ProcesarRecoleccion(int numeroMundo)
    {
        objetosRecolectados++;
        MundoData mundoRecolectado = mundos[numeroMundo];

        // DEBUG INICIAL
        Debug.Log($"🎮 INICIANDO RECOLECCIÓN MUNDO {numeroMundo}");

        // DESACTIVAR RECOLECTABLE ACTUAL
        if (mundoRecolectado.recolectable != null)
        {
            mundoRecolectado.recolectable.SetActive(false);
            Debug.Log($"❌ Recolectable {numeroMundo} desactivado: {mundoRecolectado.recolectable.name}");
        }

        // ACTIVAR MUNDO Y PUENTE
        if (mundoRecolectado.mundo != null)
        {
            mundoRecolectado.mundo.SetActive(true);
            Debug.Log($"🌍 Mundo {numeroMundo} activado: {mundoRecolectado.mundo.name}");
        }
        
        if (mundoRecolectado.puente != null)
        {
            mundoRecolectado.puente.SetActive(true);
            Debug.Log($"🌉 Puente {numeroMundo} activado: {mundoRecolectado.puente.name}");
        }

        // ✅ ACTUALIZAR RESPAWN - ESTO ES LO MÁS IMPORTANTE
        Debug.Log($"🔄 ACTUALIZANDO RESPAWN AL MUNDO {numeroMundo}");
        ActualizarRespawnJugador(numeroMundo);

        // ACTIVAR COLOR
        if (colorManager != null)
        {
            colorManager.AvanzarNivelColor();
            Debug.Log($"🎨 Color avanzado a nivel: {colorManager.GetNivelColorActual()}");
        }
        else
        {
            Debug.LogError("❌ ColorManager no asignado en GameManager");
        }

        // MENSAJES UI
        if (mensajeText != null)
        {
            // Usar mensajes personalizados por mundo si están definidos, de lo contrario usar mensajes por defecto
            string mensaje1 = !string.IsNullOrEmpty(mundos[numeroMundo].mensajeRecolectado) ? mundos[numeroMundo].mensajeRecolectado : $"Objeto {numeroMundo + 1} recolectado";
            mensajeText.text = mensaje1;
            mensajeText.gameObject.SetActive(true);
            Debug.Log($"📱 UI: {mensaje1}");
        }
        
        yield return new WaitForSeconds(3f);
        
        if (mensajeText != null)
        {
            string mensaje2 = !string.IsNullOrEmpty(mundos[numeroMundo].mensajeActivado) ? mundos[numeroMundo].mensajeActivado : $"Mundo {numeroMundo + 1} activado";
            mensajeText.text = mensaje2;
            Debug.Log($"📱 UI: {mensaje2}");
        }
        
        yield return new WaitForSeconds(3f);
        
        // ACTIVAR SIGUIENTE RECOLECTABLE
        if (sistemaSecuencial && numeroMundo < mundos.Length - 1)
        {
            mundoActual = numeroMundo + 1;
            Debug.Log($"🔜 Mundo actual actualizado a: {mundoActual}");
            
            if (mundos[mundoActual].recolectable != null)
            {
                mundos[mundoActual].recolectable.SetActive(true);
                Debug.Log($"✅ Recolectable {mundoActual} activado: {mundos[mundoActual].recolectable.name}");
            }
        }

        // TELETRANSPORTE INMEDIATO
        if (mundoRecolectado.puntoTeletransporte != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerRespawn playerRespawn = player.GetComponent<PlayerRespawn>();
                if (playerRespawn != null)
                {
                    Debug.Log($"🚀 Teletransportando al nuevo respawn: {mundoRecolectado.puntoTeletransporte.name}");
                    playerRespawn.TeletransportarAlInicio(mundoRecolectado.puntoTeletransporte);
                }
                else
                {
                    Debug.LogError("❌ No se encontró PlayerRespawn en el jugador");
                }
            }
            else
            {
                Debug.LogError("❌ No se encontró el jugador en la escena");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No hay punto de teletransporte asignado para este mundo");
        }
        
        AsegurarTextoOculto();
        Debug.Log($"✅ RECOLECCIÓN MUNDO {numeroMundo} COMPLETADA");
    }

    // MÉTODO PARA ACTUALIZAR EL RESPAWN DEL JUGADOR
    public void ActualizarRespawnJugador(int numeroMundo)
    {
        if (numeroMundo >= 0 && numeroMundo < mundos.Length)
        {
            Debug.Log($"🎯 Intentando actualizar respawn al mundo {numeroMundo}");
            
            if (mundos[numeroMundo].puntoTeletransporte == null)
            {
                Debug.LogError($"❌ ERROR: Mundo {numeroMundo} no tiene punto de teletransporte asignado");
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("❌ ERROR: No se encontró el jugador en la escena");
                return;
            }

            PlayerRespawn playerRespawn = player.GetComponent<PlayerRespawn>();
            if (playerRespawn == null)
            {
                Debug.LogError("❌ ERROR: El jugador no tiene componente PlayerRespawn");
                return;
            }

            // ✅ ESTA ES LA LÍNEA CRÍTICA QUE ACTUALIZA EL RESPAWN
            playerRespawn.SetRespawnPoint(mundos[numeroMundo].puntoTeletransporte);
            Debug.Log($"✅ RESPAWN ACTUALIZADO: Mundo {numeroMundo} -> {mundos[numeroMundo].puntoTeletransporte.name} en posición {mundos[numeroMundo].puntoTeletransporte.position}");
        }
        else
        {
            Debug.LogError($"❌ Número de mundo inválido: {numeroMundo}");
        }
    }

    // Método para forzar activación de un mundo (para testing)
    public void ForzarActivacionMundo(int numeroMundo)
    {
        if (numeroMundo >= 0 && numeroMundo < mundos.Length)
        {
            mundoActual = numeroMundo;
            Debug.Log($"🔧 FORZANDO activación del mundo {numeroMundo}");
            
            if (mundos[numeroMundo].recolectable != null)
            {
                mundos[numeroMundo].recolectable.SetActive(true);
                Debug.Log($"✅ Recolectable {numeroMundo} forzado a ACTIVADO");
            }
            
            // Actualizar respawn también
            ActualizarRespawnJugador(numeroMundo);
        }
    }

    // Comandos de testing
    private void Update()
    {
        // Presiona 1, 2, 3, 4 para activar mundos manualmente
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            Debug.Log("🔧 TEST: Tecla 1 presionada - Mundo 0");
            ForzarActivacionMundo(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            Debug.Log("🔧 TEST: Tecla 2 presionada - Mundo 1");
            ForzarActivacionMundo(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            Debug.Log("🔧 TEST: Tecla 3 presionada - Mundo 2");
            ForzarActivacionMundo(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            Debug.Log("🔧 TEST: Tecla 4 presionada - Mundo 3");
            ForzarActivacionMundo(3);
        }
        
        // Verificar estado actual
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"📊 ESTADO ACTUAL - Mundo: {mundoActual}, Objetos: {objetosRecolectados}");
        }
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

    [Header("Mensajes personalizados")]
    [TextArea]
    public string mensajeRecolectado;  // Mensaje al recolectar el objeto de este mundo
    [TextArea]
    public string mensajeActivado;      // Mensaje al activar este mundo
}
