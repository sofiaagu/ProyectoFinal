using UnityEngine;

public class PlayerZonaProhibida : MonoBehaviour
{
    private Vector3 puntoInicio;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        puntoInicio = transform.position; // Guarda la posición inicial
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Detecta si tocó un objeto con el tag "Prohibido"
        if (hit.collider.CompareTag("Prohibido"))
        {
            Debug.Log("Zona prohibida tocada");

            // Reinicia la posición del jugador
            controller.enabled = false; // Desactiva momentáneamente para evitar bugs
            transform.position = puntoInicio;
            controller.enabled = true;

            // (Opcional) agrega un efecto o sonido aquí
        }
    }
}

