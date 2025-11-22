using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI mensajeText;
    public GameObject puente;
    private bool objetoRecolectado = false;
    
    private void Start()
    {
        AsegurarTextoOculto();
        if (puente != null) puente.SetActive(false);
    }
    
    private void AsegurarTextoOculto()
    {
        if (mensajeText != null)
        {
            mensajeText.text = "";
            mensajeText.gameObject.SetActive(false);
        }
    }
    
    public void RecolectarPrimerObjeto()
    {
        if (!objetoRecolectado)
        {
            objetoRecolectado = true;
            StartCoroutine(ProcesarMensajes());
            if (puente != null) puente.SetActive(true);
        }
    }
    
    IEnumerator ProcesarMensajes()
    {
        // Primer mensaje
        if (mensajeText != null)
        {
            mensajeText.text = "Primer objeto recolectado";
            mensajeText.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(3f);
        
        // Segundo mensaje
        if (mensajeText != null)
        {
            mensajeText.text = "Puente abierto";
        }
        
        yield return new WaitForSeconds(3f);
        
        // Ocultar
        AsegurarTextoOculto();
    }
}