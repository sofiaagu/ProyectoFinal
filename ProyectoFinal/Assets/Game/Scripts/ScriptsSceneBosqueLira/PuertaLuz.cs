using UnityEngine;

public class PuertaLuz : MonoBehaviour
{
    [Header("Contenido detrás de la puerta")]
    public GameObject contenidoDetras; 

    bool abierta = false;

    void OnMouseDown()
    {
        if (abierta) return;

        AbrirPuerta();
    }

    void AbrirPuerta()
    {
        abierta = true;

        gameObject.SetActive(false);
    }
}
