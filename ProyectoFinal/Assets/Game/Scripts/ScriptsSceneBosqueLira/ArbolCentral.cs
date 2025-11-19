using UnityEngine;

public class ArbolCentral : MonoBehaviour
{
    [Header("Portal final que aparecerá")]
    public GameObject portalFinal;

    [Header("Objeto que debe DESAPARECER cuando el portal aparezca")]
    public GameObject objetoADesaparecer;

    [Header("Objeto que debe APARECER cuando el portal aparezca")]
    public GameObject objetoAAparecer;

    [Header("UI del juego")]
    public ControllerSceneLira controller;

    void Start()
    {
        // Seguridad
        if (controller == null)
            controller = FindObjectOfType<ControllerSceneLira>();

        // Asegurar que el portal esté apagado al inicio
        if (portalFinal != null)
            portalFinal.SetActive(false);

        // Asegurar que el objeto que aparecerá esté apagado al inicio
        if (objetoAAparecer != null)
            objetoAAparecer.SetActive(false);
    }

    private void OnMouseDown()
    {
        // Si NO existe GameFlowManager
        if (GameFlowManager.Instance == null)
        {
            Debug.LogError("No existe GameFlowManager en escena.");
            return;
        }

        // Si NO tiene todas las semillas
        if (!GameFlowManager.Instance.TieneTodasLasSemillas())
        {
            controller?.MostrarMensaje("Aún te faltan semillas...", 3f);
            Debug.Log("❌ No tienes todas las semillas");
            return;
        }

        // Si tiene las 3 → activar portal + cambiar objetos
        ActivarPortalFinal();
    }

    void ActivarPortalFinal()
    {
        // Activar el portal
        if (portalFinal != null)
        {
            portalFinal.SetActive(true);
            controller?.MostrarMensaje("¡El portal final ha aparecido!", 3f);
            Debug.Log("✨ Portal final activado");
        }
        else
        {
            Debug.LogError("No asignaste el portalFinal en el inspector.");
        }

        // Apagar GameObject
        if (objetoADesaparecer != null)
        {
            objetoADesaparecer.SetActive(false);
            Debug.Log("🔻 Objeto ocultado.");
        }

        // Encender GameObject
        if (objetoAAparecer != null)
        {
            objetoAAparecer.SetActive(true);
            Debug.Log("🔺 Objeto mostrado.");
        }
    }
}
