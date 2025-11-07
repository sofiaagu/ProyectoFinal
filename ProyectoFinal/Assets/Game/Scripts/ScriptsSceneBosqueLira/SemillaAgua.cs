using UnityEngine;

public class SemillaAgua : MonoBehaviour
{
    private bool recogida = false;
    private SantuarioAgua santuario;

    void Start()
    {
        santuario = FindObjectOfType<SantuarioAgua>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (recogida) return;

        if (other.CompareTag("Player"))
        {
            recogida = true;
            Debug.Log("🌱 Semilla del Agua recogida");

            // Activa el portal desde el Santuario
            if (santuario != null)
                santuario.ActivarPortal();

            // Puedes añadir aquí un sonido o partícula de recolección
            Destroy(gameObject);
        }
    }
}
