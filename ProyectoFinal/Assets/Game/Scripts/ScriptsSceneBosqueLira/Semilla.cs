using UnityEngine;

public class Semilla : MonoBehaviour
{
    [Header("Configuración de la Semilla")]
    public string tipoSemilla; // "Agua", "Tierra", "Luz"
    private bool recolectada = false;

    [Header("Efectos al recolectar")]
    public ParticleSystem efectoRecoleccion;  // 🌟 Efecto principal
    public ParticleSystem particulaExtra1;    // ✨ Partícula extra 1
    public ParticleSystem particulaExtra2;    // 💫 Partícula extra 2
    public ParticleSystem particulaExtra3;    // 🌈 Partícula extra 3
    public AudioSource sonidoRecoleccion;     // 🔊 Sonido opcional

    void OnMouseDown()
    {
        if (recolectada) return;
        recolectada = true;

        // 🌱 Marcar semilla como recogida globalmente
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.MarcarSemillaRecogida(tipoSemilla);

        // 🎇 Activar partículas
        if (efectoRecoleccion != null)
            efectoRecoleccion.Play();

        if (particulaExtra1 != null)
            particulaExtra1.Play();

        if (particulaExtra2 != null)
            particulaExtra2.Play();

        if (particulaExtra3 != null)
            particulaExtra3.Play();

        // 🔊 Sonido (opcional)
        if (sonidoRecoleccion != null)
            sonidoRecoleccion.Play();

        // 🌀 Activar portal del santuario correspondiente
        SantuarioAgua santuario = FindObjectOfType<SantuarioAgua>();
        santuario?.ActivarPortel();

        // 🕐 Esperar un poco antes de desactivar la semilla
        Invoke(nameof(Desactivar), 1.2f);
    }

    void Desactivar()
    {
        gameObject.SetActive(false);
    }
}
