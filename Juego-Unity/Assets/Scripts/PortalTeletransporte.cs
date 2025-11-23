using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalTeletransporte : MonoBehaviour
{
    [Header("Configuración Portal")]
    public string nombreEscenaDestino = "Portal";
    public KeyCode teclaActivacion = KeyCode.E;
    public float rangoActivacion = 3f;
    
    [Header("Previsualización")]
    public GameObject panelPrevisualizacion;
    public Renderer planoPrevisualizacion;
    public Texture imagenPrevisualizacion;

    [Header("Efectos de transición")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.75f;
    
    private Transform player;
    private bool jugadorCerca = false;
    private bool isLoading = false;

    private void Start()
    {
        // ✅ BUSCAR PLAYER PERSISTENTE (puede no estar en la escena inicialmente)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("🎮 Player encontrado para portal");
        }
        else
        {
            Debug.Log("⚠️ Player no encontrado al iniciar portal - Se buscará en Update");
        }
        
        // Configurar previsualización
        if (planoPrevisualizacion != null && imagenPrevisualizacion != null)
        {
            planoPrevisualizacion.material.mainTexture = imagenPrevisualizacion;
        }
        
        if (panelPrevisualizacion != null)
        {
            panelPrevisualizacion.SetActive(false);
        }
        
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }
        
        Debug.Log("🌀 Portal inicializado - Escena destino: " + nombreEscenaDestino);
    }

    private void Update()
    {
        // ✅ BUSCAR PLAYER SI NO SE ENCONTRÓ
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("🎮 Player encontrado en Update");
            }
        }

        if (player == null || isLoading) return;

        float distancia = Vector3.Distance(transform.position, player.position);
        jugadorCerca = distancia <= rangoActivacion;

        // Mostrar/ocultar previsualización
        if (panelPrevisualizacion != null)
        {
            panelPrevisualizacion.SetActive(jugadorCerca);
        }

        // Teletransportar al presionar tecla
        if (jugadorCerca && Input.GetKeyDown(teclaActivacion))
        {
            StartCoroutine(TeletransportarAsync());
        }
    }

    public void Teletransportar()
    {
        // Para llamadas externas
        StartCoroutine(TeletransportarAsync());
    }

    private IEnumerator TeletransportarAsync()
    {
        if (isLoading) yield break;
        isLoading = true;

        Debug.Log($"🚀 Iniciando teletransporte a: {nombreEscenaDestino}");

        // Mostrar fade
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < fadeDuration)
            {
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        // ✅ EL PLAYER ES PERSISTENTE - NO necesitamos guardar estado
        // Solo cargar la nueva escena, el Player sobrevivirá automáticamente

        // ✅ POSICIONAR AL PLAYER EN UN PUNTO SEGURO ANTES DE CAMBIAR ESCENA
        if (player != null)
        {
            // Mover el player lejos temporalmente para evitar conflictos
            player.position = new Vector3(1000, 1000, 1000);
            Debug.Log("📍 Player movido a posición temporal segura");
        }

        // ✅ CARGAR ESCENA - EL PLAYER PERSISTENTE SE MANTENDRÁ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nombreEscenaDestino);
        
        // ✅ DESACTIVAR AUTO-ACTIVACIÓN DE ESCENA (para más control)
        asyncLoad.allowSceneActivation = false;

        // Esperar a que la carga esté casi completa
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // ✅ PERMITIR ACTIVACIÓN DE LA ESCENA
        asyncLoad.allowSceneActivation = true;

        // Esperar a que la escena esté completamente cargada
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("✅ Escena cargada completamente");

        // ✅ BUSCAR PUNTO DE RESPAWN EN LA NUEVA ESCENA Y POSICIONAR AL PLAYER
        yield return new WaitForSeconds(0.1f); // Pequeño delay para asegurar que todo se inicialice
        
        GameObject respawnObj = GameObject.Find("RESPAWN");
        if (respawnObj != null && player != null)
        {
            player.position = respawnObj.transform.position;
            player.rotation = respawnObj.transform.rotation;
            Debug.Log($"📍 Player posicionado en RESPAWN: {respawnObj.transform.position}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró objeto RESPAWN en la nueva escena");
        }

        // Quitar fade después de cargar y posicionar
        if (fadeCanvasGroup != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        isLoading = false;
        Debug.Log("🎉 Teletransporte completado - Player persistente en nueva escena");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Teletransporte automático al tocar
        if (other.CompareTag("Player") && !isLoading)
        {
            Debug.Log("🎯 Player entró en el portal - Teletransporte automático");
            StartCoroutine(TeletransportarAsync());
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizar rango de activación en el editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangoActivacion);
    }
}