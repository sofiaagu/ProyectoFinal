using UnityEngine;

public class AntorchaLuz : MonoBehaviour
{
    [Header("Partículas de la antorcha")]
    public ParticleSystem fuegoEncendido;   

    [Header("Opcional")]
    public AudioSource sonidoEncender;

    private bool encendida = false;
    private SantuarioLuz santuario;

    void Start()
    {
        // Busca el script del SantuarioLuz
        santuario = FindObjectOfType<SantuarioLuz>();

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
            sonidoEncender.Play();

        // Avisar al santuario que una antorcha fue encendida
        santuario?.RegistrarAntorchaEncendida();
    }
}
