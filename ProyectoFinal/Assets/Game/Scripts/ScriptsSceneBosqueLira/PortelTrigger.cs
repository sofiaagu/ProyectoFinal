using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortelTrigger : MonoBehaviour
{
    [Header("Configuración del Portel")]
    [Tooltip("Arrastra aquí el objeto del plano actual")]
    public GameObject planoActualObj;

    [Tooltip("Punto donde aparecerá el jugador al usar el portal")]
    public Transform puntoLlegada;

    [Tooltip("Efecto visual al usar el portal")]
    public ParticleSystem efectoPortel;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (efectoPortel != null)
            efectoPortel.Play();

        if (puntoLlegada != null)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            other.transform.position = puntoLlegada.position;

            if (controller != null) controller.enabled = true;

            Debug.Log($"Teletransportado a {puntoLlegada.position}");
        }

        if (planoActualObj != null && GameFlowManager.Instance != null)
        {
            string nombrePlano = planoActualObj.name;
            GameFlowManager.Instance.IrASiguientePlano(nombrePlano);
        }
        else
        {
            Debug.LogWarning("No se asignó el plano actual o falta GameFlowManager.");
        }
    }
}
