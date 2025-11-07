using UnityEngine;

public class Palabra : MonoBehaviour
{
    public string nombrePalabra;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Controller2 controller = FindFirstObjectByType<Controller2>();

            if (controller != null)
            {
                controller.AgregarPalabra(nombrePalabra);
                Debug.Log("✅ Palabra recolectada: " + nombrePalabra);
            }

            Destroy(gameObject);
        }
    }
}
