using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Puntos de Movimiento")]
    public Transform upperPoint;   // Punto superior
    public Transform lowerPoint;   // Punto inferior

    [Header("Ajustes")]
    public float speed = 2f;       // Velocidad
    public bool startAtTop = false;

    private Vector3 targetPos;

    void Start()
    {
        // Elige dónde comenzar
        targetPos = startAtTop ? lowerPoint.position : upperPoint.position;
    }

    void Update()
    {
        // Mover hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Si llega al objetivo, cambiar dirección
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            // Cambiar entre upper y lower
            targetPos = targetPos == upperPoint.position ? lowerPoint.position : upperPoint.position;
        }
    }

    // Vista en editor de los puntos
    void OnDrawGizmos()
    {
        if (upperPoint != null && lowerPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(upperPoint.position, 0.15f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lowerPoint.position, 0.15f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(upperPoint.position, lowerPoint.position);
        }
    }
}
