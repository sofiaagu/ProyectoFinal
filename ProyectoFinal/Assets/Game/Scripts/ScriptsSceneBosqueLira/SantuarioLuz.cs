using UnityEngine;

public class SantuarioLuz : MonoBehaviour
{
    [Header("Semilla")]
    public GameObject prefabSemilla;   // Prefab de la semilla que aparecerá

    private int antorchasEncendidas = 0;
    public int antorchasNecesarias = 3;

    // Llamado por las antorchas cuando se encienden
    public void RegistrarAntorchaEncendida()
    {
        antorchasEncendidas++;

        Debug.Log("Antorchas encendidas: " + antorchasEncendidas);

        if (antorchasEncendidas >= antorchasNecesarias)
            AparecerSemilla();
    }

    void AparecerSemilla()
    {
        // La semilla aparece donde está este objeto (el SantuarioLuz)
        Instantiate(prefabSemilla, transform.position, Quaternion.identity);

        Debug.Log("¡La semilla del Santuario de Luz ha aparecido!");
    }
}
