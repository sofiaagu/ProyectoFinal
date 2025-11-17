using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public RawImage iconoFragmento;
    private bool ocupado = false;

    void Start()
    {
        if (iconoFragmento != null)
        {
            iconoFragmento.enabled = false;
        }
    }

    public bool EstaOcupado()
    {
        return ocupado;
    }

    public void AgregarFragmento(Texture2D textura)
    {
        ocupado = true;
        iconoFragmento.texture = textura;
        iconoFragmento.enabled = true;
    }
}