using UnityEngine;

public class Semilla : MonoBehaviour
{
    private ControllerSceneLira controller;
    private bool recolectada = false;

    [Header("Efectos opcionales")]
    public ParticleSystem efectoRecoleccion;
    public AudioSource sonidoRecoleccion;

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

            // Efectos visuales y sonoros
            if (efectoRecoleccion != null) efectoRecoleccion.Play();
            if (sonidoRecoleccion != null) sonidoRecoleccion.Play();

            // Desactivar objeto después de un breve tiempo
            Invoke(nameof(Desactivar), 0.5f);
        }
    }

    private void Desactivar()
    {
        gameObject.SetActive(false);
    }
}
