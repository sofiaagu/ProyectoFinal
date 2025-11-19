using UnityEngine;

public class SantuarioLuz : MonoBehaviour
{
    [Header("Semilla del santuario")]
    public GameObject semillaLuz;  

    [Header("Portal del Santuario de Luz")]
    public GameObject portalLuz;    

    [Header("Configuración del Puzzle")]
    public int antorchasNecesarias = 3;

    private int antorchasEncendidas = 0;
    private ControllerSceneLira controller;

    [Header("Planos a activar/desactivar al completar el minijuego")]
    public GameObject planoDesaparecer;   
    public GameObject planoAparecer;      

    [Header("Nuevos planos que también deben aparecer")]
    public GameObject planoAparecer2;     
    public GameObject planoAparecer3;    

    [Header("Objetos adicionales")]
    public GameObject objetoADesaparecer; 
    public GameObject prefabAparecer;    

    void Start()
    {
        controller = FindObjectOfType<ControllerSceneLira>();

        if (semillaLuz != null)
            semillaLuz.SetActive(false);

        if (portalLuz != null)
            portalLuz.SetActive(false);

        if (planoAparecer != null)
            planoAparecer.SetActive(false);

        if (planoAparecer2 != null)
            planoAparecer2.SetActive(false);

        if (planoAparecer3 != null)
            planoAparecer3.SetActive(false);

        if (prefabAparecer != null)
            prefabAparecer.SetActive(false);
    }

  
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

    void ActivarSemilla()
    {
        if (controller != null)
        {
            controller.MostrarMensaje(
                "¡Has encendido las tres antorchas sagradas!\n\nRecolecta la semilla de luz.",
                4f
            );
        }

        
        if (semillaLuz != null)
            semillaLuz.SetActive(true);

      
        if (planoDesaparecer != null)
            planoDesaparecer.SetActive(false);

       
        if (planoAparecer != null)
            planoAparecer.SetActive(true);

        if (planoAparecer2 != null)
            planoAparecer2.SetActive(true);

        if (planoAparecer3 != null)
            planoAparecer3.SetActive(true);

        if (objetoADesaparecer != null)
            objetoADesaparecer.SetActive(false);


        if (prefabAparecer != null)
            prefabAparecer.SetActive(true);

        Debug.Log("¡Santuario de Luz completado! Planos y objetos actualizados.");
    }


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
