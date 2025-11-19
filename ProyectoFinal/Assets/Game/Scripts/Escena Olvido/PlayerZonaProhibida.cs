using UnityEngine;

public class PlayerZonaProhibida : MonoBehaviour
{
    [Header("Punto al que regresa el jugador")]
    [Tooltip("Asigna aquí el objeto que marca el punto de reinicio.")]
    public Transform puntoReinicio;

    [Header("UI")]
    public UIestado uiEstado;

    [Header("Sonido")]
    public AudioClip sonidoProhibido;
    private AudioSource audioSource;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Crea un AudioSource si no existe
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (puntoReinicio == null)
            Debug.LogWarning("No se asignó un punto de reinicio al jugador.");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Prohibido"))
            return;

        Debug.Log("Zona prohibida tocada");
        uiEstado.ActualizarEstado("Has tocado una zona prohibida");

        // Sonido del error
        if (sonidoProhibido != null)
            audioSource.PlayOneShot(sonidoProhibido);

        // Regresar al punto de reinicio
        if (puntoReinicio != null)
        {
            controller.enabled = false;
            transform.position = puntoReinicio.position;
            controller.enabled = true;

            Debug.Log($"Jugador devuelto a: {puntoReinicio.position}");
        }
        else
        {
            Debug.LogWarning("No hay punto de reinicio asignado.");
        }
    }
}
