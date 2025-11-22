using UnityEngine;
using TMPro; // ¡Importante para TextMeshPro!
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI mensajeText; // Cambiado a TextMeshProUGUI

    public void MostrarMensaje(string mensaje, float duracion)
    {
        StartCoroutine(MostrarMensajeTemporal(mensaje, duracion));
    }

    IEnumerator MostrarMensajeTemporal(string mensaje, float duracion)
    {
        mensajeText.text = mensaje;
        mensajeText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duracion);

        mensajeText.gameObject.SetActive(false);
    }
}