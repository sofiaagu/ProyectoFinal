using UnityEngine;

public class PlayerZonaProhibida : MonoBehaviour
{
    [Header("🏁 Punto al que regresa el jugador")]
    [Tooltip("Asigna aquí el objeto que marca el punto de reinicio.")]
    public Transform puntoReinicio;

    private CharacterController controller;
    public UIestado uiEstado;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Verifica que haya un punto de reinicio asignado
        if (puntoReinicio == null)
        {
            Debug.LogWarning("⚠️ No se asignó un punto de reinicio al jugador.");
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Prohibido"))
        {
            Debug.Log("🚫 Zona prohibida tocada");
            uiEstado.ActualizarEstado("Zona prohibida tocada");

            if (puntoReinicio != null)
            {
                controller.enabled = false;
                transform.position = puntoReinicio.position; // ✅ Teletransporte al punto marcado
                controller.enabled = true;

                Debug.Log($"🔄 Jugador devuelto a: {puntoReinicio.position}");
            }
            else
            {
                Debug.LogWarning("⚠️ No se pudo regresar porque no hay punto de reinicio asignado.");
            }

            // (Opcional) aquí puedes reproducir un sonido o animación
        }
    }
}
