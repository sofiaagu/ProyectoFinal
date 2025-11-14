using UnityEngine;

public class PuertaLuz : MonoBehaviour
{
    [Header("Contenido detrás de la puerta")]
    public GameObject contenidoDetras; // Puede ser una antorcha o vacío

    bool abierta = false;

    void OnMouseDown()
    {
        if (abierta) return;

        AbrirPuerta();
    }

    void AbrirPuerta()
    {
        abierta = true;

        // Desactivar la puerta
        gameObject.SetActive(false);

        // Mostrar lo que tiene atrás
        if (contenidoDetras != null)
            contenidoDetras.SetActive(true);
    }
}
