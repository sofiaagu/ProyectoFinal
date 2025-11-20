using UnityEngine;
using System.Collections;

public class PiedraPuzzle : MonoBehaviour
{
    [Header("ID de la piedra (para el puzzle)")]
    public int id;

    [Header("Sonidos")]
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;

    private SantuarioAgua santuario;
    private AudioSource audioSource;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private bool activa = true;

    void Start()
    {
        santuario = FindObjectOfType<SantuarioAgua>();
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; 
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1f;
    }

    void OnMouseDown()
    {
        if (activa && santuario != null)
        {
            santuario.ClickEnPiedra(id, this);
        }
    }

    public void Desactivar()
    {
        activa = false;
        gameObject.SetActive(false);
    }

    public void Reactivar()
    {
        activa = true;
        gameObject.SetActive(true);
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
    }


    public void ReproducirResultado(bool correcto)
    {
        AudioClip clip = correcto ? sonidoCorrecto : sonidoIncorrecto;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);

            if (correcto)
                StartCoroutine(DesactivarLuego(clip.length));
        }
    }

    IEnumerator DesactivarLuego(float t)
    {
        yield return new WaitForSeconds(t);
        Desactivar();
    }
}
