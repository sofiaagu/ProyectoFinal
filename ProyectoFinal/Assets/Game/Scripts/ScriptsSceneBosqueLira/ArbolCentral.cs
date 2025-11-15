using UnityEngine;

public class ArbolCentral : MonoBehaviour
{
    [Header("Portal final que aparecerá")]
    public GameObject portalFinal;

    [Header("UI del juego")]
    public ControllerSceneLira controller;

    void Start()
    {
        // Seguridad
        if (controller == null)
            controller = FindObjectOfType<ControllerSceneLira>();

        if (portalFinal != null)
            portalFinal.SetActive(false);
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

        // Si tiene las 3 → activar portal
        ActivarPortalFinal();
    }

    void ActivarPortalFinal()
    {
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
    }
}
