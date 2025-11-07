//using UnityEngine;

//[RequireComponent(typeof(Collider))]
//public class PortalTrigger : MonoBehaviour
//{
//    [Header("Configuración del Portal")]
//    [Tooltip("Arrastra aquí el objeto que representa el plano actual (por ejemplo, 'PlanoAgua')")]
//    public GameObject planoActualObj;

//    [Tooltip("Punto donde aparecerá el jugador al usar el portal (opcional, si es teletransporte local).")]
//    public Transform puntoLlegada;

//    [Tooltip("Partículas o efecto visual al usar el portal (opcional).")]
//    public ParticleSystem efectoPortal;

//    private void Reset()
//    {
//        // Asegura que el collider sea un trigger
//        Collider col = GetComponent<Collider>();
//        if (col != null) col.isTrigger = true;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        // Solo reacciona si el objeto tiene el tag "Player"
//        if (!other.CompareTag("Player"))
//            return;

//        if (efectoPortal != null)
//            efectoPortal.Play();

//        // Si hay un punto de llegada dentro del mismo plano, teletransporta localmente
//        if (puntoLlegada != null)
//        {
//            CharacterController controller = other.GetComponent<CharacterController>();
//            if (controller != null) controller.enabled = false;

//            other.transform.position = puntoLlegada.position;

//            if (controller != null) controller.enabled = true;

//            Debug.Log($"🌀 Teletransportado dentro del plano a {puntoLlegada.position}");
//        }

//        // Si hay un plano asignado, busca su nombre para pasar al GameFlowManager
//        if (planoActualObj != null && GameFlowManager.Instance != null)
//        {
//            string nombrePlano = planoActualObj.name;
//            Debug.Log($"🌍 Portal desde: {nombrePlano}");
//            GameFlowManager.Instance.IrASiguientePlano(nombrePlano);
//        }
//        else
//        {
//            Debug.LogWarning("⚠️ PortalTrigger: No se asignó el plano actual o falta GameFlowManager.");
//        }
//    }
//}
