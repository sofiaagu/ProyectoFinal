using UnityEngine;

/// <summary>
/// Script simple para que la cámara siga suavemente al jugador
/// Adjuntar al CameraRig
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("El jugador u objeto a seguir")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("Offset de posición respecto al target")]
    public Vector3 offset = new Vector3(0, 0, 0);

    [Tooltip("Velocidad de seguimiento (1-20). Mayor = más rápido")]
    [Range(1f, 20f)]
    public float followSpeed = 10f;

    [Header("Rotation Settings")]
    [Tooltip("¿La cámara debe rotar con el jugador?")]
    public bool followRotation = false;

    [Tooltip("Velocidad de rotación si followRotation está activo")]
    [Range(1f, 20f)]
    public float rotationSpeed = 5f;

    [Header("Height Smoothing")]
    [Tooltip("Suavizado adicional para movimiento vertical")]
    public bool smoothHeight = true;

    [Range(1f, 20f)]
    public float heightSpeed = 5f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // Si no se asignó target, intentar encontrar al jugador
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("CameraFollow: Target encontrado automáticamente - " + player.name);
            }
            else
            {
                Debug.LogError("CameraFollow: No se encontró target. Asigna el Player manualmente o dale el tag 'Player'.");
            }
        }

        // Posicionar inmediatamente en el primer frame
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calcular posición objetivo
        Vector3 targetPosition = target.position + offset;

        // Suavizado de altura opcional
        if (smoothHeight)
        {
            Vector3 currentPos = transform.position;
            float newY = Mathf.Lerp(currentPos.y, targetPosition.y, heightSpeed * Time.deltaTime);
            targetPosition.y = newY;
        }

        // Seguir al target con suavizado
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Rotación opcional
        if (followRotation)
        {
            Quaternion targetRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    // Visualización en Scene View
    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Línea del CameraRig al target
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);

        // Esfera en la posición objetivo
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position + offset, 0.5f);
    }
}