using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Configuración Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 1f;
    
    [Header("Efectos Visuales")]
    public ParticleSystem deathParticles;
    public ParticleSystem teleportParticles;
    public Renderer playerRenderer;
    
    private Vector3 initialPosition;
    private CharacterController characterController;
    private bool isDead = false;
    private bool isTeleporting = false;
    private MonoBehaviour[] movementScripts;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementScripts = GetComponents<MonoBehaviour>();
        initialPosition = transform.position;
        
        Debug.Log("🎮 PLAYER RESPAWN INICIADO");
        
        // Si no hay respawnPoint, buscar el del GameManager
        if (respawnPoint == null)
        {
            Debug.Log("🔍 Buscando respawn inicial...");
            BuscarRespawnInicial();
        }
        else
        {
            Debug.Log($"📍 Respawn inicial asignado: {respawnPoint.name}");
        }
    }

    private void BuscarRespawnInicial()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.mundos != null && gameManager.mundos.Length > 0)
        {
            // ✅ SOLUCIÓN: Usar mundoActual que ahora es público
            int mundoParaRespawn = gameManager.mundoActual;
            
            Debug.Log($"🔍 Buscando respawn para mundo: {mundoParaRespawn}");
            
            // Verificar que el mundo actual tenga punto de respawn válido
            if (mundoParaRespawn < gameManager.mundos.Length && 
                gameManager.mundos[mundoParaRespawn].puntoTeletransporte != null)
            {
                respawnPoint = gameManager.mundos[mundoParaRespawn].puntoTeletransporte;
                Debug.Log($"✅ Respawn inicial asignado desde GameManager (Mundo {mundoParaRespawn}): {respawnPoint.name}");
            }
            else
            {
                // Fallback: buscar cualquier mundo que tenga respawn point
                for (int i = gameManager.mundos.Length - 1; i >= 0; i--)
                {
                    if (gameManager.mundos[i].puntoTeletransporte != null)
                    {
                        respawnPoint = gameManager.mundos[i].puntoTeletransporte;
                        Debug.Log($"🔄 Respawn asignado al mundo {i} (fallback): {respawnPoint.name}");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró GameManager o array de mundos vacío");
        }
        
        // Si aún no hay respawnPoint, crear uno
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = initialPosition;
            Debug.Log("🆕 Respawn point creado en posición inicial");
        }
    }

    public void TeletransportarAlInicio(Transform puntoTeletransporte)
    {
        if (isTeleporting)
        {
            Debug.Log("⚠️ Teletransporte ignorado - ya se está ejecutando uno");
            return;
        }
        
        if (puntoTeletransporte == null)
        {
            Debug.LogError("❌ ERROR: Punto de teletransporte es NULL");
            return;
        }
        
        Debug.Log($"🚀 Iniciando teletransporte a: {puntoTeletransporte.name}");
        StartCoroutine(TeletransporteCoroutine(puntoTeletransporte));
    }

    private IEnumerator TeletransporteCoroutine(Transform puntoTeletransporte)
    {
        isTeleporting = true;
        Debug.Log("1️⃣ Iniciando corrutina de teletransporte");

        // Desactivar scripts de movimiento
        foreach (var script in movementScripts)
        {
            if (script != this && script != null && script.enabled)
            {
                script.enabled = false;
            }
        }

        // Desactivar Character Controller
        if (characterController != null)
        {
            characterController.enabled = false;
            yield return null;
        }

        // Efectos visuales
        if (teleportParticles != null)
        {
            teleportParticles.Play();
            Debug.Log("4️⃣ Efectos de partículas activados");
        }

        yield return new WaitForSeconds(0.1f);

        // TELETRANSPORTE
        Debug.Log($"5️⃣ Realizando teletransporte a: {puntoTeletransporte.position}");
        transform.position = puntoTeletransporte.position;
        transform.rotation = puntoTeletransporte.rotation;

        yield return null;
        yield return new WaitForSeconds(0.1f);

        // Reactivar Character Controller
        if (characterController != null)
        {
            characterController.enabled = true;
            yield return null;
        }

        // Reactivar scripts de movimiento
        foreach (var script in movementScripts)
        {
            if (script != this && script != null)
            {
                script.enabled = true;
            }
        }

        // Limpiar física
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isTeleporting = false;
        Debug.Log("🎉 Teletransporte completado exitosamente!");
    }

    public void Respawn()
    {
        if (isDead || isTeleporting)
        {
            Debug.Log("⚠️ Respawn ignorado - isDead: " + isDead + ", isTeleporting: " + isTeleporting);
            return;
        }
        
        Debug.Log("💀 Iniciando respawn...");
        
        // ✅ VERIFICAR RESPAWN ACTUAL ANTES DE REAPARECER
        VerificarRespawnActual();
        
        if (respawnPoint != null)
        {
            Debug.Log($"📍 Respawn point actual: {respawnPoint.name} en {respawnPoint.position}");
        }
        else
        {
            Debug.LogError("❌ Respawn point es NULL!");
        }
        
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        isDead = true;
        Debug.Log("1️⃣ Iniciando corrutina de respawn");

        // Efectos de muerte
        if (deathParticles != null)
        {
            deathParticles.Play();
            Debug.Log("2️⃣ Partículas de muerte activadas");
        }
        
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false;
            Debug.Log("3️⃣ Renderer desactivado");
        }

        // Desactivar componentes
        if (characterController != null)
        {
            characterController.enabled = false;
            Debug.Log("4️⃣ Character Controller desactivado");
        }
        
        foreach (var script in movementScripts)
        {
            if (script != this && script != null && script.enabled)
            {
                script.enabled = false;
            }
        }

        // Esperar delay
        Debug.Log("5️⃣ Esperando " + respawnDelay + " segundos...");
        yield return new WaitForSeconds(respawnDelay);

        // Teletransportar a respawn
        if (respawnPoint != null)
        {
            Debug.Log("6️⃣ Teletransportando a respawn point...");
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
            Debug.Log($"   ✅ Nueva posición: {transform.position}");
        }
        else
        {
            transform.position = initialPosition;
            Debug.Log($"   ✅ Nueva posición (inicial): {initialPosition}");
        }

        // Reactivar componentes
        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
            Debug.Log("7️⃣ Renderer reactivado");
        }
        
        if (characterController != null)
        {
            characterController.enabled = true;
            Debug.Log("8️⃣ Character Controller reactivado");
        }
        
        foreach (var script in movementScripts)
        {
            if (script != this && script != null)
            {
                script.enabled = true;
            }
        }

        isDead = false;
        Debug.Log("🎉 RESPAWN COMPLETADO");
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint != null)
        {
            respawnPoint = newRespawnPoint;
            Debug.Log($"🔄 RESPAWN ACTUALIZADO - Nueva posición: {respawnPoint.position}");
            Debug.Log($"🔄 GameObject del respawn: {respawnPoint.gameObject.name}");
        }
        else
        {
            Debug.LogError("❌ Intento de asignar respawn point nulo");
        }
    }

    // ✅ NUEVO MÉTODO: Verificar que el respawn sea el correcto antes de reaparecer
    private void VerificarRespawnActual()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.mundos != null && gameManager.mundos.Length > 0)
        {
            int mundoActual = gameManager.mundoActual;
            
            // Si el respawn actual no coincide con el mundo actual, actualizarlo
            if (mundoActual < gameManager.mundos.Length && 
                gameManager.mundos[mundoActual].puntoTeletransporte != null &&
                respawnPoint != gameManager.mundos[mundoActual].puntoTeletransporte)
            {
                Debug.Log($"🔄 Corrigiendo respawn: Mundo {mundoActual} -> {gameManager.mundos[mundoActual].puntoTeletransporte.name}");
                respawnPoint = gameManager.mundos[mundoActual].puntoTeletransporte;
            }
        }
    }

    // Para muerte por caída (DeathZone)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("💀 Player cayó en DeathZone - Respawn automático");
            Respawn();
        }
    }

    // Comandos de testing
    private void Update()
    {
        // Presiona R para respawn manual (testing)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("🔧 Respawn manual por tecla R");
            Respawn();
        }
        
        // Presiona T para teletransporte al respawn (testing)
        if (Input.GetKeyDown(KeyCode.T) && respawnPoint != null)
        {
            Debug.Log("🔧 Teletransporte manual por tecla T");
            TeletransportarAlInicio(respawnPoint);
        }
        
        // Verificar respawn actual
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (respawnPoint != null)
            {
                Debug.Log($"📍 Respawn actual: {respawnPoint.name} en {respawnPoint.position}");
            }
            else
            {
                Debug.Log("❌ Respawn point es NULL");
            }
        }
    }
}