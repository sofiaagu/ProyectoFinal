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
    public AudioSource sonidoRecoleccion;

    void OnMouseDown()
    {
        if (recolectada) return;
        recolectada = true;

        Debug.Log($"🌱 Semilla de {tipoSemilla} recogida");

        // ✅ 1. Marcar semilla globalmente
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.MarcarSemillaRecogida(tipoSemilla);

        // ✅ 2. Mostrar mensaje y actualizar contador
        ControllerSceneLira controller = FindObjectOfType<ControllerSceneLira>();
        if (controller != null)
        {
            controller.RecogerSemilla(); // Actualiza texto y contador
            controller.MostrarMensaje($"Sigue el camino de luz para usar el Portal");
        }

        // ✅ 3. Reproducir efectos
        ReproducirEfectos();

        // ✅ 4. Activar portal del santuario correspondiente
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
                santuarioTierra.ActivarPortal();
                santuarioTierra.ActivarPrefab();
        }


        // ✅ 5. Desactivar semilla después del efecto
        Invoke(nameof(Desactivar), 0.2f);
    }

    private void ReproducirEfectos()
    {
        ActivarYReproducir(efectoRecoleccion);
        ActivarYReproducir(particulaExtra1);
        ActivarYReproducir(particulaExtra2);
        ActivarYReproducir(particulaExtra3);

        if (sonidoRecoleccion != null)
        {
            sonidoRecoleccion.gameObject.SetActive(true);
            sonidoRecoleccion.Play();
        }
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
