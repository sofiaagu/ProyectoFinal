using UnityEngine;

public class SantuarioAgua : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject semillaAgua;          // Arrastra aquí la semilla desde la jerarquía
    public ParticleSystem efectoAparicion;  // Efecto visual que se reproduce al completar el puzzle

    [Header("Configuración del puzzle")]
    public int[] ordenCorrecto = { 1, 3, 2 }; // Puedes cambiar el orden si quieres
    private int indiceActual = 0;
    private bool completado = false;

    // Método llamado por cada piedra cuando se hace clic
    public void ClickEnPiedra(int id)
    {
        if (completado) return;

        // Si el jugador hace clic en la piedra correcta
        if (id == ordenCorrecto[indiceActual])
        {
            indiceActual++;
            Debug.Log($"Piedra correcta: {id}");

            // Si completó toda la secuencia
            if (indiceActual >= ordenCorrecto.Length)
            {
                Completado();
            }
        }
        else
        {
            // Si se equivoca, reinicia la secuencia
            indiceActual = 0;
            Debug.Log("Secuencia incorrecta. Reiniciando...");
        }
    }

    // Se ejecuta al completar el puzzle correctamente
    private void Completado()
    {
        completado = true;
        Debug.Log("¡Puzzle del Santuario del Agua completado! 🌊");

        // Mostrar semilla
        if (semillaAgua != null)
        {
            semillaAgua.SetActive(true);
        }

        // Activar efecto visual
        if (efectoAparicion != null)
        {
            efectoAparicion.Play();
        }
    }
}
