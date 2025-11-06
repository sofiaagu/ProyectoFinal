using UnityEngine;
using TMPro;

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

    private void Start()
    {
        if (notificacionUI != null)
            notificacionUI.SetActive(false);
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

        if (habilidad == AbilityType.Luminiscencia)
        {
            abilities.tieneLuminiscencia = true;
            /*MostrarNotificacion("¡Habilidad Adquirida!\nLuminiscencia\n[Click en cristales]")*/;
        }
        else if (habilidad == AbilityType.Equilibrio)
        {
            abilities.tieneEquilibrio = true;
            //MostrarNotificacion("¡Habilidad Adquirida!\nEquilibrio\n[Presiona E para cam  biar plano]");
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

    //private void MostrarNotificacion(string mensaje)
    //{
    //    if (notificacionUI != null && textoHabilidad != null)
    //    {
    //        textoHabilidad.text = mensaje;
    //        notificacionUI.SetActive(true);

    //        // Cancelar invocación anterior si existe
    //        CancelInvoke("OcultarNotificacion");

    //        // Programar ocultación
    //        Invoke("OcultarNotificacion", duracionNotificacion);
    //    }
    //}

    private void OcultarNotificacion()
    {
        if (notificacionUI != null)
        {
            notificacionUI.SetActive(false);
            Debug.Log("Panel ocultado");
        }
    }
}