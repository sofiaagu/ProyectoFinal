using UnityEngine;

public class Semilla : MonoBehaviour
{
    public string tipoSemilla; // "Agua", "Tierra" o "Luz"
    private bool recolectada = false;

    [Header("Efectos opcionales")]
    public ParticleSystem efectoRecoleccion;
    public AudioSource sonidoRecoleccion;

    void OnMouseDown()
    {
        if (recolectada) return;
        recolectada = true;

        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.MarcarSemillaRecogida(tipoSemilla);

        // Efectos visuales
        if (efectoRecoleccion != null) efectoRecoleccion.Play();
        if (sonidoRecoleccion != null) sonidoRecoleccion.Play();

        // Activar el portal del santuario correspondiente
        SantuarioAgua santuario = FindObjectOfType<SantuarioAgua>();
        santuario?.ActivarPortel();

        Invoke(nameof(Desactivar), 0.5f);
    }

    void Desactivar()
    {
        gameObject.SetActive(false);
    }
}
