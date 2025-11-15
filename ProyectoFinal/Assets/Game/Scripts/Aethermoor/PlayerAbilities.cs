using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Habilidades Desbloqueadas")]
    public bool tieneLuminiscencia = false;
    public bool tieneEquilibrio = false;

    [Header("Equilibrio - Configuración")]
    public GameObject planoFisico;
    public GameObject planoEtereo;
    public Camera playerCamera;

    [Header("Equilibrio Temporal (Q)")]
    public GameObject estructurasTemporales;        // Agrupa aquí todas las plataformas que deben aparecer
    public float duracionEquilibrioTemporal = 12f;  // 10–15 segundos recomendado
    private bool usandoEquilibrioTemporal = false;
    private float timerEquilibrio = 0f;

    private bool enPlanoEtereo = false;

    void Start()
    {
        // Configuración inicial del plano
        if (planoFisico != null) planoFisico.SetActive(true);
        if (planoEtereo != null) planoEtereo.SetActive(false);

        // Asegurar que las estructuras temporales inicien apagadas
        if (estructurasTemporales != null)
            estructurasTemporales.SetActive(false);
    }

    void Update()
    {
        // Habilidad de Equilibrio - Cambiar de plano con E
        if (tieneEquilibrio && Input.GetKeyDown(KeyCode.R))
        {
            CambiarPlano();
        }

        // Activar estructuras temporales con Q
        if (tieneEquilibrio && Input.GetKeyDown(KeyCode.Q))
        {
            ActivarEquilibrioTemporal();
        }

        // Manejar duración del Equilibrio Temporal
        if (usandoEquilibrioTemporal)
        {
            timerEquilibrio -= Time.deltaTime;

            if (timerEquilibrio <= 0)
            {
                DesactivarEquilibrioTemporal();
            }
        }
    }

    void CambiarPlano()
    {
        enPlanoEtereo = !enPlanoEtereo;

        if (planoFisico != null) planoFisico.SetActive(!enPlanoEtereo);
        if (planoEtereo != null) planoEtereo.SetActive(enPlanoEtereo);

        // Cambiar color de la cámara
        if (playerCamera != null)
        {
            playerCamera.backgroundColor = enPlanoEtereo ?
                new Color(0.4f, 0f, 0.6f) : // Morado
                new Color(0.1f, 0.2f, 0.4f); // Azul
        }

        Debug.Log("Cambiado a plano: " + (enPlanoEtereo ? "Etéreo" : "Físico"));
    }

    // ============================================================
    //   MÉTODOS DEL EQUILIBRIO TEMPORAL (Q)
    // ============================================================

    void ActivarEquilibrioTemporal()
    {
        if (estructurasTemporales == null) return;

        if (!usandoEquilibrioTemporal)
        {
            estructurasTemporales.SetActive(true);
            usandoEquilibrioTemporal = true;
            timerEquilibrio = duracionEquilibrioTemporal;

            Debug.Log("Equilibrio Temporal ACTIVADO por " + duracionEquilibrioTemporal + " segundos.");
        }
    }

    void DesactivarEquilibrioTemporal()
    {
        if (estructurasTemporales == null) return;

        estructurasTemporales.SetActive(false);
        usandoEquilibrioTemporal = false;

        Debug.Log("Equilibrio Temporal DESACTIVADO.");
    }
}
