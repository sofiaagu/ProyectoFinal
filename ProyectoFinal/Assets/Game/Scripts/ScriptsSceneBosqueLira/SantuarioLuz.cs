using UnityEngine;

public class SantuarioLuz : MonoBehaviour
{
    [Header("Semilla ya colocada en la escena")]
    public GameObject semillaLuz;   // La semilla que YA está ubicada en la escena

    [Header("Portal del Santuario de Luz")]
    public GameObject portalLuz;    // Portal que aparecerá al recoger la semilla

    private int antorchasEncendidas = 0;
    public int antorchasNecesarias = 3;

    public void RegistrarAntorchaEncendida()
    {
        antorchasEncendidas++;

        Debug.Log("Antorchas encendidas: " + antorchasEncendidas);

        if (antorchasEncendidas >= antorchasNecesarias)
            ActivarSemilla();
    }

    void ActivarSemilla()
    {
        if (semillaLuz != null)
        {
            semillaLuz.SetActive(true);
            Debug.Log("¡La semilla del Santuario de Luz ha aparecido!");
        }
        else
        {
            Debug.LogError("No asignaste la semilla en el inspector.");
        }
    }

    // ⭐ Método que la semilla va a llamar cuando se le haga clic
    public void ActivarPortel()   // Si fue typo, puedes renombrarlo a ActivarPortal
    {
        if (portalLuz != null)
        {
            portalLuz.SetActive(true);
            Debug.Log("Portal del Santuario de Luz ACTIVADO");
        }
        else
        {
            Debug.LogError("No asignaste el portal del Santuario de Luz en el inspector.");
        }
    }
}
