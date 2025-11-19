using UnityEngine;

public class AntorchaLuz : MonoBehaviour
{
    [Header("Partículas de la antorcha")]
    public ParticleSystem fuegoEncendido;

    [Header("Sonido al encender")]
    public AudioClip sonidoEncender;

    private AudioSource audioSource;
    private bool encendida = false;
    private SantuarioLuz santuario;

    void Start()
    {
        // Busca el script del SantuarioLuz
        santuario = FindObjectOfType<SantuarioLuz>();

        // Crear un AudioSource automáticamente
        audioSource = gameObject.AddComponent<AudioSource>();

        // Asegura que la antorcha empieza apagada
        if (fuegoEncendido != null)
        {
            fuegoEncendido.Stop();
            fuegoEncendido.gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        // Evitar repetir clics
        if (encendida) return;

        encendida = true;

        // Encender partículas
        if (fuegoEncendido != null)
        {
            fuegoEncendido.gameObject.SetActive(true);
            fuegoEncendido.Play();
        }

        // Sonido
        if (sonidoEncender != null)
            audioSource.PlayOneShot(sonidoEncender);

        // Avisar al santuario
        santuario?.RegistrarAntorchaEncendida();
    }
}
