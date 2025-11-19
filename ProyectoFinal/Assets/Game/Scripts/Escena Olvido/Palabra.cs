using UnityEngine;

public class Palabra : MonoBehaviour
{
    public string nombrePalabra;
    public GameObject portalAsociado;
    public UIestado uiEstado;

    [Header("Sonido")]
    public AudioClip sonidoPalabra;

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

            // Reproducir sonido sin cortarse
            if (sonidoPalabra != null)
                AudioSource.PlayClipAtPoint(sonidoPalabra, transform.position, 1f);

            // Activar el portal asociado
            if (portalAsociado != null)
            {
                portalAsociado.SetActive(true);
                Debug.Log("Portal activado tras recoger: " + nombrePalabra);

                if (uiEstado != null)
                    uiEstado.ActualizarEstado("Portal activado");
            }
            else
            {
                Debug.LogWarning("No hay portal asociado para " + nombrePalabra);
            }

            Destroy(gameObject);
        }
    }
}
