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
    
    [Header("Configuración de Mensajes")]
    public float tiempoMensajeRecolectado = 3f;
    public float tiempoMensajeActivado = 3f;
    public bool mostrarMensajes = true;
    
    [Header("Configuración de Iluminación")]
    public Light luzDireccional;
    public Color colorLuzDiurna = Color.white;
    public Color colorLuzNocturna = new Color(0.1f, 0.1f, 0.3f);
    public float intensidadDiurna = 1f;
    public float intensidadNocturna = 0.1f;
    
    [Header("Estado del Juego")]
    public int mundoActual = 0;
    public int objetosRecolectados = 0;
    
    private void Start()
    {
        // ✅ RESETEAR COLOR MANAGER al iniciar nueva partida
        ColorCuerpoManager colorManager = FindObjectOfType<ColorCuerpoManager>();
        if (colorManager != null)
        {
            colorManager.IniciarNuevaPartida();
        }

        // ✅ BUSCAR LUZ DIRECCIONAL AUTOMÁTICAMENTE
        if (luzDireccional == null)
        {
            Light[] todasLasLuces = FindObjectsOfType<Light>();
            foreach (Light luz in todasLasLuces)
            {
                if (luz.type == LightType.Directional)
                {
                    luzDireccional = luz;
                    Debug.Log("✅ Luz direccional encontrada automáticamente: " + luz.name);
                    break;
                }
            }
            
            if (luzDireccional == null)
            {
                Debug.LogWarning("⚠️ No se encontró luz direccional en la escena");
            }
        }

        AsegurarTextoOculto();
        InicializarMundosSecuenciales();
        
        // ✅ ASEGURAR ILUMINACIÓN DIURNA AL INICIO
        ConfigurarIluminacionDiurna();
        
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
        MundoData mundoRecolectado = mundos[numeroMundo];
        
        // ✅ VERIFICAR SI SE DEBE CONTAR EN EL CONTADOR
        bool contarEnEstadisticas = mundoRecolectado.contarEnEstadisticas;
        
        if (contarEnEstadisticas)
        {
            objetosRecolectados++;
            Debug.Log($"🔢 Contador incrementado: {objetosRecolectados} objetos");
        }
        else
        {
            Debug.Log($"🔢 Objeto NO contado en estadísticas: {mundoRecolectado.nombreMundo}");
        }

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

        // ✅ ACTUALIZAR RESPAWN
        Debug.Log($"🔄 ACTUALIZANDO RESPAWN AL MUNDO {numeroMundo}");
        ActualizarRespawnJugador(numeroMundo);

        // ACTIVAR COLOR (solo si se cuenta en estadísticas)
        if (colorManager != null && contarEnEstadisticas)
        {
            colorManager.AvanzarNivelColor();
            Debug.Log($"🎨 Color avanzado a nivel: {colorManager.GetNivelColorActual()}");
        }
        else if (colorManager != null && !contarEnEstadisticas)
        {
            Debug.Log("🎨 Color NO avanzado - Objeto no cuenta en estadísticas");
        }
        else
        {
            Debug.LogError("❌ ColorManager no asignado en GameManager");
        }

        // ✅ DETECTAR SI ES EL ÚLTIMO MUNDO
        bool esUltimoMundo = (numeroMundo == mundos.Length - 1);

        // ✅ CONFIGURACIÓN ESPECIAL PARA EL ÚLTIMO MUNDO
        if (esUltimoMundo)
        {
            Debug.Log("🎯 ES EL ÚLTIMO MUNDO - Mostrando mensaje FINAL");
            
            // ✅ MOSTRAR MENSAJE FINAL ANTES DEL TELETRANSPORTE
            if (mensajeText != null)
            {
                string mensajeFinal = "¡VOLVISTE A CONSEGUIR TU ALMA, FIN!";
                mensajeText.text = mensajeFinal;
                mensajeText.gameObject.SetActive(true);
                Debug.Log($"📱 UI MOSTRANDO MENSAJE FINAL: {mensajeFinal}");
                
                // Esperar un tiempo para que se vea el mensaje "Fin"
                yield return new WaitForSeconds(3f);
                
                // Ocultar el mensaje después del tiempo
                AsegurarTextoOculto();
                Debug.Log("📱 Mensaje FINAL ocultado");
            }
            
            // ✅ TELETRANSPORTE DESPUÉS DEL MENSAJE
            if (mundoRecolectado.puntoTeletransporte != null)
            {
                Debug.Log($"🚀 TELETRANSPORTE FINAL a: {mundoRecolectado.puntoTeletransporte.name}");
                EjecutarTeletransporteInmediato(mundoRecolectado.puntoTeletransporte);
            }
            
            Debug.Log($"✅ ÚLTIMA RECOLECCIÓN COMPLETADA - Mundo {numeroMundo}");
            yield break; // Salir de la corutina
        }

        // ✅ CONFIGURACIÓN NORMAL PARA MUNDOS QUE NO SON EL ÚLTIMO
        float tiempoRecolectado = mundoRecolectado.tiempoMensajeRecolectado > 0 ? 
            mundoRecolectado.tiempoMensajeRecolectado : tiempoMensajeRecolectado;
            
        float tiempoActivado = mundoRecolectado.tiempoMensajeActivado > 0 ? 
            mundoRecolectado.tiempoMensajeActivado : tiempoMensajeActivado;

        bool mostrarMensajesMundo = mundoRecolectado.mostrarMensajes;

        // ✅ MENSAJES UI (SOLO PARA MUNDOS QUE NO SON EL ÚLTIMO)
        if (mensajeText != null && mostrarMensajesMundo)
        {
            // Primer mensaje
            string mensaje1 = !string.IsNullOrEmpty(mundos[numeroMundo].mensajeRecolectado) ? 
                mundos[numeroMundo].mensajeRecolectado : $"Objeto {numeroMundo + 1} recolectado";
            
            mensajeText.text = mensaje1;
            mensajeText.gameObject.SetActive(true);
            Debug.Log($"📱 UI: {mensaje1} (Tiempo: {tiempoRecolectado}s)");

            if (tiempoRecolectado > 0)
            {
                yield return new WaitForSeconds(tiempoRecolectado);
            }
            else
            {
                yield return null;
            }

            // Segundo mensaje
            string mensaje2 = !string.IsNullOrEmpty(mundos[numeroMundo].mensajeActivado) ? 
                mundos[numeroMundo].mensajeActivado : $"Mundo {numeroMundo + 1} activado";
            
            mensajeText.text = mensaje2;
            Debug.Log($"📱 UI: {mensaje2} (Tiempo: {tiempoActivado}s)");

            if (tiempoActivado > 0)
            {
                yield return new WaitForSeconds(tiempoActivado);
            }
            else
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("📱 Mensajes desactivados para este mundo");
            yield return new WaitForSeconds(0.1f);
        }
        
        // ACTIVAR SIGUIENTE RECOLECTABLE (solo si no es el último)
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

        // ✅ TELETRANSPORTE INMEDIATO PARA MUNDOS QUE NO SON EL ÚLTIMO
        bool teletransportarInmediato = mundoRecolectado.teletransporteInmediato;

        if (mundoRecolectado.puntoTeletransporte != null && teletransportarInmediato)
        {
            Debug.Log($"🚀 EJECUTANDO TELETRANSPORTE INMEDIATO al mundo {numeroMundo}");
            EjecutarTeletransporteInmediato(mundoRecolectado.puntoTeletransporte);
        }
        else if (mundoRecolectado.puntoTeletransporte != null)
        {
            Debug.Log("📍 Punto de teletransporte disponible, pero teletransporte inmediato desactivado");
        }
        
        AsegurarTextoOculto();
        Debug.Log($"✅ RECOLECCIÓN MUNDO {numeroMundo} COMPLETADA");
    }

    // ✅ MÉTODO PARA TELETRANSPORTE INMEDIATO
    private void EjecutarTeletransporteInmediato(Transform puntoDestino)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerRespawn playerRespawn = player.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                Debug.Log($"🚀 Teletransportando inmediatamente a: {puntoDestino.name}");
                playerRespawn.TeletransportarAlInicio(puntoDestino);
            }
            else
            {
                Debug.LogError("❌ No se encontró PlayerRespawn en el jugador");
                // Fallback: teletransporte directo
                player.transform.position = puntoDestino.position;
                player.transform.rotation = puntoDestino.rotation;
            }
        }
        else
        {
            Debug.LogError("❌ No se encontró el jugador en la escena");
        }
    }

    // ✅ MÉTODOS DE ILUMINACIÓN (AHORA PÚBLICOS)
    public void CambiarIluminacion(bool esNoche)
    {
        if (esNoche)
        {
            ConfigurarIluminacionNocturna();
        }
        else
        {
            ConfigurarIluminacionDiurna();
        }
    }

    private void ConfigurarIluminacionNocturna()
    {
        if (luzDireccional != null)
        {
            luzDireccional.color = colorLuzNocturna;
            luzDireccional.intensity = intensidadNocturna;
            luzDireccional.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            Debug.Log("🌙 Iluminación cambiada a modo NOCHE");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay luz direccional asignada para cambiar a modo noche");
        }
    }

    private void ConfigurarIluminacionDiurna()
    {
        if (luzDireccional != null)
        {
            luzDireccional.color = colorLuzDiurna;
            luzDireccional.intensity = intensidadDiurna;
            luzDireccional.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Debug.Log("☀️ Iluminación cambiada a modo DÍA");
        }
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
            if (playerRespawn != null)
            {
                playerRespawn.SetRespawnPoint(mundos[numeroMundo].puntoTeletransporte);
                Debug.Log($"✅ RESPAWN ACTUALIZADO: Mundo {numeroMundo} -> {mundos[numeroMundo].puntoTeletransporte.name}");
            }
            else
            {
                Debug.LogError("❌ ERROR: El jugador no tiene componente PlayerRespawn");
            }
        }
        else
        {
            Debug.LogError($"❌ Número de mundo inválido: {numeroMundo}");
        }
    }

    // ✅ MÉTODO PARA OBTENER EL CONTADOR REAL (solo objetos que cuentan)
    public int GetObjetosRecolectadosReales()
    {
        return objetosRecolectados;
    }

    // ✅ MÉTODO PARA OBTENER EL TOTAL DE OBJETOS QUE CUENTAN
    public int GetTotalObjetosQueCuentan()
    {
        int total = 0;
        foreach (MundoData mundo in mundos)
        {
            if (mundo.contarEnEstadisticas)
            {
                total++;
            }
        }
        return total;
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

    // ✅ MÉTODO PARA TELETRANSPORTE MANUAL DESDE OTROS SCRIPTS
    public void TeletransportarAMundo(int numeroMundo)
    {
        if (numeroMundo >= 0 && numeroMundo < mundos.Length)
        {
            Debug.Log($"🚀 TELETRANSPORTE MANUAL al mundo {numeroMundo}");
            
            // Actualizar respawn primero
            ActualizarRespawnJugador(numeroMundo);
            
            // Ejecutar teletransporte inmediato
            if (mundos[numeroMundo].puntoTeletransporte != null)
            {
                EjecutarTeletransporteInmediato(mundos[numeroMundo].puntoTeletransporte);
            }
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
        
        // Teletransporte rápido con T + número
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Input.GetKey(KeyCode.Alpha1)) TeletransportarAMundo(0);
            if (Input.GetKey(KeyCode.Alpha2)) TeletransportarAMundo(1);
            if (Input.GetKey(KeyCode.Alpha3)) TeletransportarAMundo(2);
            if (Input.GetKey(KeyCode.Alpha4)) TeletransportarAMundo(3);
        }

        // Comando para probar iluminación
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ConfigurarIluminacionNocturna();
            }
            else
            {
                ConfigurarIluminacionDiurna();
            }
        }
        
        // Verificar estado actual
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"📊 ESTADO ACTUAL - Mundo: {mundoActual}, Objetos: {objetosRecolectados}");
            Debug.Log($"📊 OBJETOS QUE CUENTAN: {GetObjetosRecolectadosReales()}/{GetTotalObjetosQueCuentan()}");
        }
    }
}

// ✅ CLASE MUNDODATA - VA AL FINAL DEL MISMO ARCHIVO
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
    
    [Header("Tiempos de Mensaje")]
    public float tiempoMensajeRecolectado = 0f; // 0 = usar valor global
    public float tiempoMensajeActivado = 0f;    // 0 = usar valor global
    public bool mostrarMensajes = true;         // Mostrar mensajes para este mundo
    
    [Header("Teletransporte")]
    public bool teletransporteInmediato = true; // Teletransportar inmediatamente después de recolectar
    
    [Header("Estadísticas")]
    public bool contarEnEstadisticas = true;    // ✅ Si este objeto se cuenta en el contador
}