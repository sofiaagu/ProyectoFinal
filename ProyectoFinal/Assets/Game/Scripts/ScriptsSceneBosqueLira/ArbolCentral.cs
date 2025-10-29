using UnityEngine;

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
        }
    }
}
