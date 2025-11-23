using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPersistente : MonoBehaviour
{
    private static PlayerPersistente instance;

    private CharacterController controller;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            controller = GetComponent<CharacterController>();

            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("PLAYER persistente configurado correctamente");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(TeleportSafe());
    }

    private IEnumerator TeleportSafe()
    {
        // 1) Desactivar CharacterController para evitar empujes, gravedad y colisiones
        if (controller != null)
            controller.enabled = false;

        // Esperar 2 frames para que la escena termine de cargar
        yield return null;
        yield return null;

        // 2) Reposicionar al jugador en el respawn de la escena Portal
        GameObject spawnPoint = GameObject.FindWithTag("RespawnPoint");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;

            Debug.Log("Jugador reposicionado correctamente al RespawnPoint");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró ningún objeto con tag 'RespawnPoint'");
        }

        // 3) Reactivar CharacterController
        yield return new WaitForSeconds(0.1f);

        if (controller != null)
            controller.enabled = true;
    }
}
