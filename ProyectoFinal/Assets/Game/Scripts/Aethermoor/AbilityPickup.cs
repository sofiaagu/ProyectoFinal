using UnityEngine;
using TMPro;
using System.Collections;

public class AbilityPickup : MonoBehaviour
{
    public enum AbilityType { Luminiscencia, Equilibrio }

    [Header("Configuración")]
    public AbilityType habilidad;

    [Header("UI")]
    public GameObject notificacionUI;
    public TextMeshProUGUI textoHabilidad;
    public float duracionNotificacion = 3f;

    [Header("Efectos")]
    public GameObject efectoRecoleccion;

    // Singleton para mantener la referencia al UI Manager
    private static AbilityUIManager uiManager;

    private void Start()
    {
        if (notificacionUI != null)
            notificacionUI.SetActive(false);

        // Crear UI Manager si no existe
        if (uiManager == null)
        {
            GameObject managerObj = new GameObject("AbilityUIManager");
            uiManager = managerObj.AddComponent<AbilityUIManager>();
            uiManager.notificacionUI = notificacionUI;
            uiManager.textoHabilidad = textoHabilidad;
            DontDestroyOnLoad(managerObj);
        }
    }

    private void Update()
    {
        // Rotación constante del objeto
        transform.Rotate(Vector3.up, 50f * Time.deltaTime);

        // Flotación
        float nuevaY = transform.position.y + Mathf.Sin(Time.time * 2f) * 0.01f;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OtorgarHabilidad(other.gameObject);
        }
    }

    private void OtorgarHabilidad(GameObject player)
    {
        PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();
        if (abilities == null)
        {
            abilities = player.AddComponent<PlayerAbilities>();
        }

        string mensaje = "";

        if (habilidad == AbilityType.Luminiscencia)
        {
            abilities.tieneLuminiscencia = true;
            mensaje = "¡Habilidad Adquirida!\nLuminiscencia\n[Click en cristales]";
        }
        else if (habilidad == AbilityType.Equilibrio)
        {
            abilities.tieneEquilibrio = true;
            FindFirstObjectByType<GameController4>()?.ObtenerHabilidadEquilibrio();
            mensaje = "¡Habilidad Adquirida!\nEquilibrio\n[Presiona R para cambiar plano]";
        }

        // Mostrar notificación ANTES de destruir
        if (uiManager != null)
        {
            uiManager.MostrarNotificacion(mensaje, duracionNotificacion);
        }

        // Efecto visual
        if (efectoRecoleccion != null)
        {
            Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);
        }

        Debug.Log("Habilidad " + habilidad + " adquirida!");

        // Destruir el pickup
        Destroy(gameObject);
    }
}

// Clase separada para manejar el UI persistente
public class AbilityUIManager : MonoBehaviour
{
    public GameObject notificacionUI;
    public TextMeshProUGUI textoHabilidad;

    public void MostrarNotificacion(string mensaje, float duracion)
    {
        if (notificacionUI != null && textoHabilidad != null)
        {
            textoHabilidad.text = mensaje;
            notificacionUI.SetActive(true);
            Debug.Log("Panel mostrado: " + mensaje);

            // Usar Coroutine en lugar de Invoke
            StopAllCoroutines();
            StartCoroutine(OcultarDespuesDe(duracion));
        }
    }

    private IEnumerator OcultarDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        if (notificacionUI != null)
        {
            notificacionUI.SetActive(false);
            Debug.Log("Panel ocultado");
        }
    }
}