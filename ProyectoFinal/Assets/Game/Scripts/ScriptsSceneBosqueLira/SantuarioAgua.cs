using System.Collections.Generic;
using UnityEngine;

public class SantuarioAgua : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject semillaAgua;          // Arrastra aquí la semilla del agua
    public ParticleSystem efectoAparicion;  // Efecto cuando se completa el puzzle
    public GameObject portalAgua;           // Portal que se activará después de recoger la semilla

    [Header("Configuración del puzzle")]
    public int[] ordenCorrecto = { 1, 3, 2 };
    private int indiceActual = 0;
    private bool completado = false;

    private List<PiedraPuzzle> piedrasActivadas = new List<PiedraPuzzle>();

    void Start()
    {
        if (semillaAgua != null)
            semillaAgua.SetActive(false);

        if (portalAgua != null)
            portalAgua.SetActive(false);
    }

    public bool ClickEnPiedra(int id, PiedraPuzzle piedra)
    {
        if (completado) return false;

        if (id == ordenCorrecto[indiceActual])
        {
            if (piedra != null && !piedrasActivadas.Contains(piedra))
                piedrasActivadas.Add(piedra);

            indiceActual++;
            Debug.Log($"✅ Piedra correcta: {id}");

            if (indiceActual >= ordenCorrecto.Length)
                Completado();

            return true;
        }
        else
        {
            Debug.Log("❌ Secuencia incorrecta. Reiniciando...");
            indiceActual = 0;
            RestaurarPiedras();
            return false;
        }
    }

    private void RestaurarPiedras()
    {
        foreach (var p in piedrasActivadas)
        {
            if (p != null)
                p.Reactivar();
        }
        piedrasActivadas.Clear();
    }

    private void Completado()
    {
        completado = true;
        Debug.Log("🌊 ¡Puzzle completado! Aparece la semilla...");

        piedrasActivadas.Clear();

        if (efectoAparicion != null)
            efectoAparicion.Play();

        // Activa la semilla para que el jugador pueda recogerla
        if (semillaAgua != null)
            semillaAgua.SetActive(true);
    }

    // Este método lo llamará la semilla cuando el jugador la recoja
    public void ActivarPortal()
    {
        if (portalAgua != null)
        {
            portalAgua.SetActive(true);
            Debug.Log("🌀 El portal del agua ha sido activado");
        }
    }
}
