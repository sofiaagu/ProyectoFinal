using UnityEngine;

public class PuertaLuz : MonoBehaviour
{

    [Header("Sonido al abrir")]
    public AudioClip sonidoAbrir;

    private AudioSource audioSource;
    private bool abierta = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        if (abierta) return;

        AbrirPuerta();
    }

    void AbrirPuerta()
    {
        abierta = true;

        // Reproducir sonido si existe
        if (sonidoAbrir != null)
            audioSource.PlayOneShot(sonidoAbrir);

        Destroy(gameObject, 0.2f);
    }
}
