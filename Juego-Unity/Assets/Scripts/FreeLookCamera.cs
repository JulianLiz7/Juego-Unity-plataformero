using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public Transform target;

    [Header("Distancias")]
    public float targetDistance = 6f;   // Distancia deseada
    public float height = 3f;           // Altura
    public float minDistance = 1f;      // Distancia mínima cuando chocamos
    public float smoothSpeed = 20f;

    [Header("Rotación")]
    public float rotationSpeed = 150f;
    private float yaw = 0f;
    private float pitch = 15f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Colisiones")]
    public LayerMask collisionMask;     // Qué capas puede chocar la cámara
    public float collisionRadius = 0.3f;

    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentDistance = targetDistance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleRotation();
        HandleCollision();
        UpdatePosition();
    }

    void HandleRotation()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void HandleCollision()
    {
        // Posición objetivo de la cámara sin colisiones
        Vector3 desiredDir = Quaternion.Euler(pitch, yaw, 0) * Vector3.back;
        Vector3 desiredPos = target.position + Vector3.up * height + desiredDir * targetDistance;

        // Hacemos un raycast desde la cabeza del personaje hacia la cámara
        Vector3 origin = target.position + Vector3.up * height;

        if (Physics.SphereCast(origin, collisionRadius, desiredDir, out RaycastHit hit, targetDistance, collisionMask))
        {
            // Si tocamos algo, reducimos la distancia
            currentDistance = Mathf.Clamp(hit.distance, minDistance, targetDistance);
        }
        else
        {
            // Si no chocamos, regresamos suave a la distancia normal
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * smoothSpeed);
        }
    }

    void UpdatePosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * (Vector3.back * currentDistance);

        Vector3 targetPos = target.position + Vector3.up * height + offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * height * 0.8f);
    }
}
