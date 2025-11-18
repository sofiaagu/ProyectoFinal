using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("Destino del portal")]
    [Tooltip("Lugar al que el jugador será transportado.")]
    public Transform destino;

    [Header("Configuración del portal")]
    public bool siempreActivo = false;
    public bool persistente = false;

    public UIestado uiEstado;
    private void Start()
    {
        // Solo el primer portal estará activo desde el inicio
        if (!siempreActivo)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"El jugador ha entrado al portal: {gameObject.name}");

            if (destino != null)
            {
                CharacterController cc = other.GetComponent<CharacterController>();

                if (cc != null) cc.enabled = false;

                // Teletransporte del jugador
                other.transform.position = destino.position;
                other.transform.rotation = destino.rotation;

                Debug.Log($"Jugador transportado a: {destino.position}");

                if (cc != null) cc.enabled = true;
            }
            else
            {
                Debug.LogWarning($"El portal {gameObject.name} no tiene destino asignado.");
            }

            // Si el portal no es persistente, se apaga tras usarse
            if (!persistente)
            {
                gameObject.SetActive(false);
                Debug.Log($"Portal {gameObject.name} desactivado tras usarse.");
            }
        }
    }

    // Llamado por Palabra.cs al recoger una palabra
    public void ActivarPortal()
    {
        gameObject.SetActive(true);
        Debug.Log($"Portal activado manualmente: {gameObject.name}");
        uiEstado.ActualizarEstado("Portal Activado");
    }
}
