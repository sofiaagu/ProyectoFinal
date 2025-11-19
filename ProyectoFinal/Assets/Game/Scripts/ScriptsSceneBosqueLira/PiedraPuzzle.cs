using UnityEngine;

public class PiedraPuzzle : MonoBehaviour
{
    [Header("ID de la piedra (para el puzzle)")]
    public int id;

    [Header("Sonidos")]
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;

    private SantuarioAgua santuario;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private bool activa = true;

    void Start()
    {
        santuario = FindObjectOfType<SantuarioAgua>();
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    void OnMouseDown()
    {
        if (activa && santuario != null)
        {
            santuario.ClickEnPiedra(id, this);
        }
    }

    // Llamar desde SantuarioAgua cuando la piedra debe desactivarse permanentemente
    public void Desactivar()
    {
        activa = false;
        gameObject.SetActive(false);
    }

    // Llamar desde SantuarioAgua cuando la piedra debe reactivarse
    public void Reactivar()
    {
        activa = true;
        gameObject.SetActive(true);
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
    }

    public void ReproducirResultado(bool correcto)
    {
        if (correcto && sonidoCorrecto != null)
        {
            AudioSource.PlayClipAtPoint(sonidoCorrecto, transform.position);
        }
        else if (!correcto && sonidoIncorrecto != null)
        {
            AudioSource.PlayClipAtPoint(sonidoIncorrecto, transform.position);
        }
    }
}
