using UnityEngine;
using System.Collections;

public class ArbolCentral : MonoBehaviour
{
    private ControllerSceneLira controlador;

    void Start()
    {
        controlador = FindObjectOfType<ControllerSceneLira>();
    }

    void OnMouseDown()
    {
        if (controlador.TieneTodasLasSemillas())
        {
            controlador.RestaurarBosque();
        }
        else
        {
            controlador.mensajeUI.text = "Aún te faltan semillas...";
            StartCoroutine(LimpiarMensaje());
        }
    }

    IEnumerator LimpiarMensaje()
    {
        yield return new WaitForSeconds(3f); // Espera 3 segundos
        controlador.mensajeUI.text = "";     // Limpia el texto
    }
}
