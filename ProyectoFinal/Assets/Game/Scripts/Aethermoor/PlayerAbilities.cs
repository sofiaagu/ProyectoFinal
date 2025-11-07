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

    private bool enPlanoEtereo = false;

    void Start()
    {
        // Configuración inicial del plano
        if (planoFisico != null) planoFisico.SetActive(true);
        if (planoEtereo != null) planoEtereo.SetActive(false);
    }

    void Update()
    {
        // Habilidad de Equilibrio - Cambiar de plano con E
        if (tieneEquilibrio && Input.GetKeyDown(KeyCode.E))
        {
            CambiarPlano();
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
}