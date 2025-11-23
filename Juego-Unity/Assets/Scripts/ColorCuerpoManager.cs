using UnityEngine;

public class ColorCuerpoManager : MonoBehaviour
{
    [Header("ELEMENTO FUEGO - Partes específicas")]
    public Renderer manoDer;
    public Renderer antebrazoDer;
    public Renderer codoDer;
    public Renderer hombroDer;
    public Renderer piernaDer;
    public Renderer llamaPequenaDer;
    public Renderer llamaGrandeDer;

    [Header("ELEMENTO AGUA - Partes específicas")]
    public Renderer manoIzq;
    public Renderer antebrazoIzq;
    public Renderer codoIzq;
    public Renderer hombroIzq;
    public Renderer piernaIzq;
    public Renderer musloIzq;
    public Renderer colaAgua;

    [Header("ELEMENTO TIERRA - Partes específicas")]
    public Renderer musloDer;
    public Renderer pecho;
    public Renderer cuello;

    [Header("ELEMENTO AIRE - Partes específicas")]
    public Renderer corazon;
    public Renderer cara;
    public Renderer cabello;
    public Renderer ojoDer;
    public Renderer ojoIzq;

    [Header("Colores de Elementos")]
    public Color fuegoColor = Color.red;
    public Color aguaColor = new Color(0f, 0.5f, 1f); // Azul agua
    public Color tierraColor = new Color(0.6f, 0.4f, 0.2f); // Marrón
    public Color aireColor = Color.white;
    public Color colorOjos = Color.black; // Color negro para los ojos

    private int nivelColor = 0;

    public void AvanzarNivelColor()
    {
        nivelColor++;
        Debug.Log("Activando elemento nivel: " + nivelColor);
        
        switch (nivelColor)
        {
            case 1:
                ActivarFuego();
                break;
            case 2:
                ActivarAgua();
                break;
            case 3:
                ActivarTierra();
                break;
            case 4:
                ActivarAire();
                break;
        }
    }

    public void ActivarFuego()
    {
        Debug.Log("Activando ELEMENTO FUEGO");
        Pintar(manoDer, fuegoColor);
        Pintar(antebrazoDer, fuegoColor);
        Pintar(codoDer, fuegoColor);
        Pintar(hombroDer, fuegoColor);
        Pintar(piernaDer, fuegoColor);
        Pintar(llamaPequenaDer, fuegoColor);
        Pintar(llamaGrandeDer, fuegoColor);
    }

    public void ActivarAgua()
    {
        Debug.Log("Activando ELEMENTO AGUA");
        Pintar(manoIzq, aguaColor);
        Pintar(antebrazoIzq, aguaColor);
        Pintar(codoIzq, aguaColor);
        Pintar(hombroIzq, aguaColor);
        Pintar(piernaIzq, aguaColor);
        Pintar(musloIzq, aguaColor);
        Pintar(colaAgua, aguaColor);
    }

    public void ActivarTierra()
    {
        Debug.Log("Activando ELEMENTO TIERRA");
        Pintar(musloDer, tierraColor);
        Pintar(pecho, tierraColor);
        Pintar(cuello, tierraColor);
    }

    public void ActivarAire()
    {
        Debug.Log("Activando ELEMENTO AIRE");
        Pintar(corazon, aireColor);
        Pintar(cara, aireColor);
        Pintar(cabello, aireColor);
        
        // ✅ OJOS EN NEGRO (no se pintan del color aire)
        Pintar(ojoDer, colorOjos);
        Pintar(ojoIzq, colorOjos);
    }

    private void Pintar(Renderer rend, Color color)
    {
        if (rend != null)
        {
            foreach (var mat in rend.materials)
            {
                mat.color = color;
                mat.EnableKeyword("_EMISSION");
                
                float intensidad = 0.3f;
                if (color == fuegoColor) intensidad = 0.6f;
                if (color == aguaColor) intensidad = 0.4f;
                if (color == tierraColor) intensidad = 0.2f;
                if (color == aireColor) intensidad = 0.8f;
                if (color == colorOjos) intensidad = 0f; // ✅ Ojos sin brillo
                
                mat.SetColor("_EmissionColor", color * intensidad);
            }
        }
        else
        {
            Debug.LogWarning("Renderer no asignado");
        }
    }

    public int GetNivelColorActual()
    {
        return nivelColor;
    }

    public void ResetearColores()
    {
        nivelColor = 0;
        Debug.Log("Todos los colores reseteados");
    }

    // ⚠️ MÉTODO DE COMPATIBILIDAD TEMPORAL - ELIMINAR DESPUÉS
    public void ActivaFruegoDerecha()
    {
        Debug.Log("Usando método de compatibilidad - activando fuego");
        ActivarFuego();
    }
}