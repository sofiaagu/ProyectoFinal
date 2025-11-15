using UnityEngine;

public class AntorchaLuz : MonoBehaviour
{
    [Header("Partículas de la antorcha")]
    public ParticleSystem fuegoEncendido;   // Sistema de partículas de fuego (apagado al inicio)

    [Header("Opcional")]
    public AudioSource sonidoEncender;

    private bool encendida = false;
    private SantuarioLuz santuario;

    void Start()
    {
        // Buscar el script del santuario
        santuario = FindObjectOfType<SantuarioLuz>();

        // Asegurar que la antorcha empieza apagada
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

        // Sonido opcional
        if (sonidoEncender != null)
            sonidoEncender.Play();

        // Avisar al santuario que una antorcha fue encendida
        santuario?.RegistrarAntorchaEncendida();
    }
}
