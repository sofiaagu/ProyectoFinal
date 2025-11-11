using UnityEngine;

public class SantuarioAgua : MonoBehaviour
{
    [Header("Referencias principales")]
    public GameObject semillaAgua;          // 🌱 Semilla que aparece al completar el puzzle
    public GameObject portelAgua;           // 🌀 Portel que se activa después
    public ParticleSystem efectoAparicion;  // ✨ Efecto al aparecer la semilla

    [Header("Prefabs a alternar")]
    public GameObject prefabDesaparecer;    // 🔹 Prefab que se desactiva
    public GameObject prefabAparecer;       // 🔹 Prefab que se activa

    [Header("Efectos de partículas")]
    public ParticleSystem particulasDesaparecer; // 💨 Efecto que se apaga al completar el puzzle
    public ParticleSystem particulaExtra1;       // 💧 Primer efecto adicional
    public ParticleSystem particulaExtra2;       // 💧 Segundo efecto adicional

    [Header("Configuración del puzzle")]
    public int[] ordenCorrecto = { 1, 3, 2 };
    private int indiceActual = 0;
    private bool completado = false;

    private PiedraPuzzle[] todasLasPiedras;
    private ControllerSceneLira controller;

    void Start()
    {
        controller = FindObjectOfType<ControllerSceneLira>();

        if (semillaAgua != null)
            semillaAgua.SetActive(false);

        if (portelAgua != null)
            portelAgua.SetActive(false);

        if (prefabAparecer != null)
            prefabAparecer.SetActive(false);

        todasLasPiedras = FindObjectsOfType<PiedraPuzzle>();
    }

    public void ClickEnPiedra(int id, PiedraPuzzle piedra)
    {
        if (completado || piedra == null) return;

        if (id == ordenCorrecto[indiceActual])
        {
            piedra.Desactivar();
            indiceActual++;
            controller.MostrarMensaje("Orden correcto");

            if (indiceActual >= ordenCorrecto.Length)
                Completado();
        }
        else
        {
            ReiniciarPuzzle();
        }
    }

    private void ReiniciarPuzzle()
    {
        indiceActual = 0;
        foreach (var piedra in todasLasPiedras)
        {
            if (piedra != null)
                piedra.Reactivar();
        }

        controller.MostrarMensaje("Secuencia incorrecta. Intenta de nuevo.");
    }

    private void Completado()
    {
        completado = true;
        controller.MostrarMensaje("¡Puzzle completado! \n Recolecta la semilla de agua.", 3f);

        if (efectoAparicion != null)
            efectoAparicion.Play();

        if (semillaAgua != null)
            semillaAgua.SetActive(true);

        if (prefabDesaparecer != null)
            prefabDesaparecer.SetActive(false);

        if (prefabAparecer != null)
            prefabAparecer.SetActive(true);

        // 💨 Desactivar partículas principales
        if (particulasDesaparecer != null)
        {
            particulasDesaparecer.Stop();
            particulasDesaparecer.gameObject.SetActive(false);
        }

        // 💧 Desactivar partículas adicionales
        if (particulaExtra1 != null)
        {
            particulaExtra1.Stop();
            particulaExtra1.gameObject.SetActive(false);
        }

        if (particulaExtra2 != null)
        {
            particulaExtra2.Stop();
            particulaExtra2.gameObject.SetActive(false);
        }
    }

    // Este se llama desde la semilla
    public void ActivarPortel()
    {
        if (portelAgua != null)
        {
            portelAgua.SetActive(true);
            Debug.Log("Portel de agua activado");
        }
    }
}
