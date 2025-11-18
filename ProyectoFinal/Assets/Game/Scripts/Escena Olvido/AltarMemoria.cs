using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AltarMemoria : MonoBehaviour
{
    public UIestado uiEstado;

    [Header("Referencias")]
    public GameObject uiOrdenarPanel;
    public UIOrdenarPalabras uiOrdenarScript;
    [Header("Portal final")]
    public GameObject portalFinal;

    private Controller2 controller;
    private bool jugadorCerca = false;

    private List<string> ordenCorrecto = new List<string> { "EL", "REINO", "DEL", "OLVIDO" };

    void Start()
    {
        controller = FindFirstObjectByType<Controller2>();
        if (uiOrdenarPanel != null)
            uiOrdenarPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && controller != null)
        {
            jugadorCerca = true;
            uiOrdenarPanel.SetActive(true);
            uiOrdenarScript.Configurar(controller.ObtenerPalabrasRecolectadas(), this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            uiOrdenarPanel.SetActive(false);
        }
    }

    public void VerificarOrdenManual(List<string> palabrasUI)
    {
        // Convierte la lista que dio la UI en una pila para comparar correctamente
        Stack<string> pilaJugador = new Stack<string>(palabrasUI);

        Stack<string> pilaCorrecta = new Stack<string>(ordenCorrecto);

        if (pilaJugador.Count != pilaCorrecta.Count)
        {
            uiEstado.ActualizarEstado("Faltan palabras.");
            return;
        }

        while (pilaJugador.Count > 0)
        {
            if (pilaJugador.Pop() != pilaCorrecta.Pop())
            {
                uiEstado.ActualizarEstado("Orden incorrecto. Intenta de nuevo.");
                return;
            }
        }

        portalFinal?.SetActive(true);
        uiEstado.ActualizarEstado("Portal final ahora está activo.");

        controller.CompletarEscena();
    }


}
