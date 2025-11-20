using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    [Header("Destino del portal")]
    [Tooltip("Objeto vacío o punto donde aparecerá el jugador al usar el portal.")]
    public Transform puntoLlegada; // Asigna aquí el objeto ZonaLlegada

    [Header("Efecto opcional")]
    [Tooltip("Partículas o efecto visual al teletransportarse (opcional).")]
    public ParticleSystem efectoPortal;

    private void Reset()
    {
        // Asegura que el collider sea un trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo reacciona si el objeto tiene el tag Player
        if (!other.CompareTag("Player"))
            return;

        if (puntoLlegada == null)
        {
            Debug.LogWarning("PortalTrigger: No se ha asignado el punto de llegada.");
            return;
        }

        // Intenta obtener el CharacterController
        CharacterController controller = other.GetComponent<CharacterController>();

        // Desactiva temporalmente el CharacterController para evitar conflictos
        if (controller != null)
            controller.enabled = false;

        // Teletransporta al jugador
        other.transform.position = puntoLlegada.position;

        // Rehabilita el CharacterController si existía
        if (controller != null)
            controller.enabled = true;

        // Activa efecto visual si existe
        if (efectoPortal != null)
            efectoPortal.Play();

        Debug.Log($"Jugador teletransportado a: {puntoLlegada.position}");
    }
}