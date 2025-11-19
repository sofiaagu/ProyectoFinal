using UnityEngine;

public class Caida : MonoBehaviour
{
    public Transform puntoInicial;
    private CharacterController controller;
    private PlayerHealth playerHealth;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Lava"))
        {
            // Quitar una vida
            if (playerHealth != null)
                playerHealth.TakeDamage(1);

            // Teletransportar
            controller.enabled = false;
            transform.position = puntoInicial.position;
            controller.enabled = true;
        }
    }
}