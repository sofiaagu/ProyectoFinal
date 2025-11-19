using UnityEngine;

public class Semilla : MonoBehaviour
{
    [Header("Configuración de la Semilla")]
    public string tipoSemilla; // "Agua", "Tierra", "Luz"
    private bool recolectada = false;

    [Header("Efectos al recolectar")]
    public ParticleSystem efectoRecoleccion;
    public ParticleSystem particulaExtra1;
    public ParticleSystem particulaExtra2;
    public ParticleSystem particulaExtra3;

    [Header("Sonido")]
    public AudioClip sonidoRecoleccion;

    private AudioSource audioSource;

    void Start()
    {
        // Crear un AudioSource automáticamente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = 1f;
    }

    void OnMouseDown()
    {
        if (recolectada) return;
        recolectada = true;

        Debug.Log($"Semilla de {tipoSemilla} recogida");

        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.MarcarSemillaRecogida(tipoSemilla);

        ControllerSceneLira controller = FindObjectOfType<ControllerSceneLira>();
        if (controller != null)
        {
            controller.RecogerSemilla();
            controller.MostrarMensaje($"Sigue el camino de luz para usar el Portal");
        }

        ReproducirEfectos();

        if (tipoSemilla == "Agua")
        {
            SantuarioAgua santuarioAgua = FindObjectOfType<SantuarioAgua>();
            if (santuarioAgua != null)
                santuarioAgua.ActivarPortel();
        }
        else if (tipoSemilla == "Tierra")
        {
            SantuarioTierra santuarioTierra = FindObjectOfType<SantuarioTierra>();
            if (santuarioTierra != null)
            {
                santuarioTierra.ActivarPortal();
                santuarioTierra.ActivarPrefab();
            }
        }
        else if (tipoSemilla == "Luz")
        {
            SantuarioLuz santuarioLuz = FindObjectOfType<SantuarioLuz>();
            if (santuarioLuz != null)
                santuarioLuz.ActivarPortel();
        }

        Invoke(nameof(Desactivar), 0.4f);
    }

    private void ReproducirEfectos()
    {
        ActivarYReproducir(efectoRecoleccion);
        ActivarYReproducir(particulaExtra1);
        ActivarYReproducir(particulaExtra2);
        ActivarYReproducir(particulaExtra3);

     
        if (sonidoRecoleccion != null)
            audioSource.PlayOneShot(sonidoRecoleccion);
    }

    private void ActivarYReproducir(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }

    private void Desactivar()
    {
        gameObject.SetActive(false);
    }
}
