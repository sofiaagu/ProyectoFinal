using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSC : MonoBehaviour
{
    [Header("Destino del portal")]
    [Tooltip("Lugar al que el jugador será transportado.")]
    public Transform destino;

    [Header("Configuración del portal")]
    [Tooltip("Si está marcado, el portal estará activo desde el inicio.")]
    public bool siempreActivo = false;

    [Tooltip("Si está marcado, el portal no se desactiva tras usarse.")]
    public bool persistente = false;

    [Header("Portal de salida (nuevo nivel)")]
    [Tooltip("Si está marcado, este portal carga una nueva escena en lugar de teletransportar.")]
    public bool esPortalDeSalida = false;

    [Tooltip("Nombre de la escena a cargar (solo si esPortalDeSalida = true).")]
    public string nombreEscenaSiguiente = "";

    private void Start()
    {
        // Solo el primer portal estará activo desde el inicio
        if (!siempreActivo)
            gameObject.SetActive(false);
    }

    // Detectar colisión con el jugador
    private void OnTriggerEnter(Collider other)
    {
        // Ignorar si el portal está desactivado
        if (!gameObject.activeSelf) return;

        // Solo reacciona si el collider es el Player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"El jugador ha entrado al portal: {gameObject.name}");

            // Si es portal de salida → cargar nueva escena
            if (esPortalDeSalida)
            {
                CargarSiguienteNivel();
                return;
            }

            // Teletransporte dentro de la misma escena
            if (destino != null)
            {
                CharacterController cc = other.GetComponent<CharacterController>();

                // Desactivar CharacterController antes de mover
                if (cc != null) cc.enabled = false;

                // Cambiar posición y rotación del jugador
                other.transform.position = destino.position;
                other.transform.rotation = destino.rotation;

                Debug.Log($"Jugador transportado a: {destino.position}");

                // Reactivar CharacterController
                if (cc != null) cc.enabled = true;
            }
            else
            {
                Debug.LogWarning($"El portal {gameObject.name} no tiene destino asignado.");
            }

            // Si el portal no es persistente, desactivarlo
            if (!persistente)
            {
                gameObject.SetActive(false);
                Debug.Log($"Portal {gameObject.name} desactivado tras usarse.");
            }
        }
    }

    // Activar portal manualmente desde SceneController u otro script
    public void ActivarPortal()
    {
        gameObject.SetActive(true);
        Debug.Log($"¡Portal {gameObject.name} ACTIVADO!");
    }

    // Método privado para cargar la siguiente escena
    private void CargarSiguienteNivel()
    {
        if (string.IsNullOrEmpty(nombreEscenaSiguiente))
        {
            Debug.LogError("No se asignó el nombre de la siguiente escena!");
            return;
        }

        Debug.Log($"Cargando siguiente nivel: {nombreEscenaSiguiente}");

        // Si hay SceneController → usarlo para carga de escena
        if (SceneController.instance != null)
        {
            SceneController.instance.CargarSiguienteNivel(nombreEscenaSiguiente);
        }
        else
        {
            // Si no hay SceneController → cargar directamente con SceneManager
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
    }
}