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

    [Header("Sonidos")]
    public AudioClip sonidoAcierto;
    public AudioClip sonidoError;


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
        Stack<string> pilaJugador = new Stack<string>(palabrasUI);
        Stack<string> pilaCorrecta = new Stack<string>(ordenCorrecto);

        if (pilaJugador.Count != pilaCorrecta.Count)
        {
            uiEstado.ActualizarEstado("Faltan palabras.");

            if (sonidoError != null)
                AudioSource.PlayClipAtPoint(sonidoError, transform.position);

            return;
        }

        while (pilaJugador.Count > 0)
        {
            if (pilaJugador.Pop() != pilaCorrecta.Pop())
            {
                uiEstado.ActualizarEstado("Orden incorrecto. Intenta de nuevo.");

                if (sonidoError != null)
                    AudioSource.PlayClipAtPoint(sonidoError, transform.position);

                return;
            }
        }

        // ✔ Orden correcto
        if (sonidoAcierto != null)
            AudioSource.PlayClipAtPoint(sonidoAcierto, transform.position);

        portalFinal?.SetActive(true);
        uiEstado.ActualizarEstado("Portal final ahora está activo.");

        controller.CompletarEscena();
    }



}
