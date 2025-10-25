using UnityEngine;

public class Semilla : MonoBehaviour
{
    public ControllerSceneLira controller;
    private bool recolectada = false;

    void OnTriggerEnter(Collider other)
    {
        if (!recolectada && other.CompareTag("Player"))
        {
            recolectada = true;
            controller.RecogerSemilla();
            gameObject.SetActive(false);
        }
    }
}
