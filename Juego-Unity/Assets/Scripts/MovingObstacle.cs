using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Puntos de Movimiento")]
    public Transform upperPoint;
    public Transform lowerPoint;

    [Header("Ajustes")]
    public float speed = 2f;
    public bool startAtTop = false;

    private Vector3 targetPos;
    private GameObject playerOnPlatform;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        targetPos = startAtTop ? lowerPoint.position : upperPoint.position;
    }

    void Update()
    {
        // Mover plataforma
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Cambiar dirección
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            targetPos = targetPos == upperPoint.position ? lowerPoint.position : upperPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerOnPlatform == null)
        {
            playerOnPlatform = other.gameObject;
            // Hacer al jugador hijo de la plataforma
            playerOnPlatform.transform.SetParent(transform);
            Debug.Log("Jugador pegado a la plataforma (Parenting)");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerOnPlatform == other.gameObject)
        {
            // Quitar parentesco
            playerOnPlatform.transform.SetParent(null);
            playerOnPlatform = null;
            Debug.Log("Jugador liberado de la plataforma");
        }
    }

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