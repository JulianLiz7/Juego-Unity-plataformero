using UnityEngine;

public class AnimacionLava : MonoBehaviour
{
    public Material materialLava;
    public float velocidad = 1f;
    
    private void Update()
    {
        if (materialLava != null)
        {
            // Animar el offset para movimiento
            float offsetX = Time.time * velocidad * 0.1f;
            float offsetY = Time.time * velocidad * 0.05f;
            
            materialLava.mainTextureOffset = new Vector2(offsetX, offsetY);
        }
    }
}