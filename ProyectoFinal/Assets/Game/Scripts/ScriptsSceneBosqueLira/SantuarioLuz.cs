using UnityEngine;

public class SantuarioLuz : MonoBehaviour
{
    [Header("Semilla del santuario")]
    public GameObject semillaLuz;   // La semilla que aparecerá al completar el minijuego

    [Header("Portal del Santuario de Luz")]
    public GameObject portalLuz;    // Portal que aparecerá al recoger la semilla

    [Header("Configuración del Puzzle")]
    public int antorchasNecesarias = 3;

    private int antorchasEncendidas = 0;
    private ControllerSceneLira controller;

    [Header("Planos a activar/desactivar al completar el minijuego")]
    public GameObject planoDesaparecer;   // 🔴 Este plano se oculta
    public GameObject planoAparecer;      // 🟢 Este plano aparece

    [Header("Nuevos planos que también deben aparecer")]
    public GameObject planoAparecer2;     // 🟢 Segundo plano que aparece
    public GameObject planoAparecer3;     // 🟢 Tercer plano que aparece

    [Header("Objetos adicionales")]
    public GameObject objetoADesaparecer; // 🔴 Objeto que desaparece
    public GameObject prefabAparecer;     // 🟢 Prefab u objeto que aparece

    // -------------------------------------------------------------------
    void Start()
    {
        controller = FindObjectOfType<ControllerSceneLira>();

        if (semillaLuz != null)
            semillaLuz.SetActive(false);

        if (portalLuz != null)
            portalLuz.SetActive(false);

        // Objetos que deben iniciar ocultos
        if (planoAparecer != null)
            planoAparecer.SetActive(false);

        if (planoAparecer2 != null)
            planoAparecer2.SetActive(false);

        if (planoAparecer3 != null)
            planoAparecer3.SetActive(false);

        if (prefabAparecer != null)
            prefabAparecer.SetActive(false);
    }

    // -------------------------------------------------------------------
    // ⭐ LLAMADO desde cada antorcha cuando se enciende
    // -------------------------------------------------------------------
    public void RegistrarAntorchaEncendida()
    {
        antorchasEncendidas++;

        if (controller != null)
        {
            controller.MostrarMensaje(
                $"¡{antorchasEncendidas} de {antorchasNecesarias} antorchas encendidas!"
            );
        }

        if (antorchasEncendidas >= antorchasNecesarias)
            ActivarSemilla();
    }

    // -------------------------------------------------------------------
    // ⭐ Se activa cuando se encienden TODAS las antorchas
    // -------------------------------------------------------------------
    void ActivarSemilla()
    {
        if (controller != null)
        {
            controller.MostrarMensaje(
                "¡Has encendido las tres antorchas sagradas!\n\nRecolecta la semilla de luz.",
                4f
            );
        }

        // 🌟 Activar semilla
        if (semillaLuz != null)
            semillaLuz.SetActive(true);

        // 🔴 Desaparecer plano principal
        if (planoDesaparecer != null)
            planoDesaparecer.SetActive(false);

        // 🟢 Mostrar los planos nuevos
        if (planoAparecer != null)
            planoAparecer.SetActive(true);

        if (planoAparecer2 != null)
            planoAparecer2.SetActive(true);

        if (planoAparecer3 != null)
            planoAparecer3.SetActive(true);

        // 🔴 Desaparecer objeto adicional
        if (objetoADesaparecer != null)
            objetoADesaparecer.SetActive(false);

        // 🟢 Activar prefab adicional
        if (prefabAparecer != null)
            prefabAparecer.SetActive(true);

        Debug.Log("¡Santuario de Luz completado! Planos y objetos actualizados.");
    }



// -------------------------------------------------------------------
// ⭐ Método que la semilla llama cuando se hace clic en ella
// -------------------------------------------------------------------
public void ActivarPortel()
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
