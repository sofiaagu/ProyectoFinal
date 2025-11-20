using UnityEngine;

public class Caida : MonoBehaviour
{
    // Referencia al punto donde el jugador se reinicia
    public Transform puntoInicial;

    // Referencias a componentes
    private CharacterController controller;
    private PlayerHealth playerHealth;

    private void Start()
    {
        // Obtener referencias a componentes del jugador
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Se llama automáticamente cuando el CharacterController colisiona con algo
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verificar si el jugador cayó en lava
        if (hit.gameObject.CompareTag("Lava"))
        {
            // Quitar una vida
            // Desactivar temporalmente el CharacterController para mover al jugador
            if (playerHealth != null)
                playerHealth.TakeDamage(1);

            // Teletransportar
            controller.enabled = false;
            // Mover jugador
            transform.position = puntoInicial.position;
            // Reactivar CharacterController
            controller.enabled = true;
        }
    }
}