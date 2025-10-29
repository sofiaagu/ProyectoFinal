using UnityEngine;

public class Semilla : MonoBehaviour
{
    private ControllerSceneLira controller;
    private bool recolectada = false;

    void Start()
    {
        controller = FindObjectOfType<ControllerSceneLira>();
    }

    void OnMouseDown()
    {
        if (!recolectada)
        {
            recolectada = true;
            controller.RecogerSemilla();

            // Desaparece o emite un efecto
            gameObject.SetActive(false);
        }
    }
}
