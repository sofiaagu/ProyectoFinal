using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    // Imagen donde se mostrará la textura del fragmento
    public RawImage iconoFragmento;

    // Estado del slot: indica si ya contiene un fragmento
    private bool ocupado = false;

    void Start()
    {
        // Al iniciar, si el icono existe, lo ocultamos
        // Esto deja el slot visualmente vacío
        if (iconoFragmento != null)
        {
            iconoFragmento.enabled = false;
        }
    }

    // Método para consultar si el slot ya está ocupado
    public bool EstaOcupado()
    {
        return ocupado;
    }

    // Método para añadir un fragmento al slot
    // Recibe la textura del fragmento recogido
    public void AgregarFragmento(Texture2D textura)
    {
        // Marcar slot como ocupado
        ocupado = true;
        // Asignar la textura al RawImage
        iconoFragmento.texture = textura;
        // Mostrar la imagen en pantalla
        iconoFragmento.enabled = true;
    }
}