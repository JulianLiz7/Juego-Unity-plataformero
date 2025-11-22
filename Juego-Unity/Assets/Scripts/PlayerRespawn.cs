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
        
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = initialPosition;
        }
    }

    public void TeletransportarAlInicio(Transform puntoTeletransporte)
    {
        if (isTeleporting || puntoTeletransporte == null)
            return;
        
        StartCoroutine(TeletransporteCoroutine(puntoTeletransporte));
    }

    private IEnumerator TeletransporteCoroutine(Transform puntoTeletransporte)
    {
        isTeleporting = true;

        foreach (var script in movementScripts)
        {
            if (script != this && script != null && script.enabled)
            {
                script.enabled = false;
            }
        }

        if (characterController != null)
        {
            characterController.enabled = false;
            yield return null;
        }

        if (teleportParticles != null)
        {
            teleportParticles.Play();
        }

        yield return new WaitForSeconds(0.1f);

        transform.position = puntoTeletransporte.position;
        transform.rotation = puntoTeletransporte.rotation;

        yield return null;
        yield return new WaitForSeconds(0.1f);

        if (characterController != null)
        {
            characterController.enabled = true;
            yield return null;
        }

        foreach (var script in movementScripts)
        {
            if (script != this && script != null)
            {
                script.enabled = true;
            }
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isTeleporting = false;
    }

    public void Respawn()
    {
        if (isDead || isTeleporting)
            return;
        
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        isDead = true;

        if (deathParticles != null)
            deathParticles.Play();
        
        if (playerRenderer != null)
            playerRenderer.enabled = false;

        if (characterController != null)
            characterController.enabled = false;
        
        foreach (var script in movementScripts)
        {
            if (script != this && script != null && script.enabled)
            {
                script.enabled = false;
            }
        }

        yield return new WaitForSeconds(respawnDelay);

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        if (playerRenderer != null)
            playerRenderer.enabled = true;
        
        if (characterController != null)
            characterController.enabled = true;
        
        foreach (var script in movementScripts)
        {
            if (script != this && script != null)
            {
                script.enabled = true;
            }
        }

        isDead = false;
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}