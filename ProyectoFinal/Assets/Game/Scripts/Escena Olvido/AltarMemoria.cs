using System.Collections.Generic;
using UnityEngine;

public class AltarMemoria : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject uiOrdenarPanel;
    public UIOrdenarPalabras uiOrdenarScript;

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

    public void VerificarOrdenManual(List<string> palabras)
    {
        if (palabras.Count != ordenCorrecto.Count)
        {
            Debug.Log("Faltan palabras.");
            return;
        }

        for (int i = 0; i < palabras.Count; i++)
        {
            if (palabras[i] != ordenCorrecto[i])
            {
                Debug.Log("Orden incorrecto. Intenta de nuevo.");
                return;
            }
        }

        Debug.Log("Orden correcto. Portal activado.");
        controller.CompletarEscena();
    }
}
