using UnityEngine;

public class Palabra : MonoBehaviour
{
    public string nombrePalabra;
    public GameObject portalAsociado; 
    public UIestado uiEstado;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Controller2 controller = FindFirstObjectByType<Controller2>();

            if (controller != null)
            {
                controller.AgregarPalabra(nombrePalabra);
                Debug.Log("Palabra recolectada: " + nombrePalabra);
            }

            // Activar el portal asociado al recoger esta palabra
            if (portalAsociado != null)
            {
                portalAsociado.SetActive(true);
                Debug.Log("Portal activado tras recoger: " + nombrePalabra);
                uiEstado.ActualizarEstado("Portal activado ");
            }
            else
            {
                Debug.LogWarning("No hay portal asociado para " + nombrePalabra);
            }

            Destroy(gameObject);
        }
    }
}
