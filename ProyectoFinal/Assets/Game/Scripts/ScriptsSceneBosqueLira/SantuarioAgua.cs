using System.Collections.Generic;
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

    [Header("Efectos adicionales")]
    public ParticleSystem particulasDesaparecer; // 💨 Efecto que debe apagarse al completar el puzzle

    [Header("Configuración del puzzle")]
    public int[] ordenCorrecto = { 1, 3, 2 };
    private int indiceActual = 0;
    private bool completado = false;

    private PiedraPuzzle[] todasLasPiedras;

    void Start()
    {
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
    }

    private void Completado()
    {
        completado = true;
        Debug.Log("🌊 ¡Puzzle completado! Aparece la semilla...");

        if (efectoAparicion != null)
            efectoAparicion.Play();

        if (semillaAgua != null)
            semillaAgua.SetActive(true);

        // 🔹 Cambiar prefabs
        if (prefabDesaparecer != null)
            prefabDesaparecer.SetActive(false);

        if (prefabAparecer != null)
            prefabAparecer.SetActive(true);

        // 💨 Desactivar partículas
        if (particulasDesaparecer != null)
        {
            particulasDesaparecer.Stop();
            particulasDesaparecer.gameObject.SetActive(false);
        }
    }

    // Este se llama desde la semilla
    public void ActivarPortel()
    {
        if (portelAgua != null)
        {
            portelAgua.SetActive(true);
            Debug.Log("🌀 Portel del agua activado");
        }
    }
}
