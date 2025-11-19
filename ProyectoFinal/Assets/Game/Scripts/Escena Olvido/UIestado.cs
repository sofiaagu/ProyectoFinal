using TMPro;
using UnityEngine;

public class UIestado : MonoBehaviour
{
    [Header("Referencias de Texto")]
    public TextMeshProUGUI textoEstado;        // Mensaje temporal principal
    public TextMeshProUGUI textoSecundario;    // Mensaje temporal secundario

    [Header("Configuración de tiempos")]
    public float duracionMensaje = 4f;         // Duración mensaje principal
    public float duracionMensajeSecundario = 4f; // Duración mensaje secundario

    private float contadorPrincipal = 0f;
    private float contadorSecundario = 0f;

    // --- MÉTODOS PARA ACTUALIZAR UI ---

    public void ActualizarEstado(string nuevoTexto)
    {
        if (textoEstado == null) return;

        textoEstado.text = nuevoTexto;
        textoEstado.gameObject.SetActive(true);
        contadorPrincipal = duracionMensaje;
    }

    public void ActualizarSecundario(string nuevoTexto)
    {
        if (textoSecundario == null) return;

        textoSecundario.text = nuevoTexto;
        textoSecundario.gameObject.SetActive(true);
        contadorSecundario = duracionMensajeSecundario;
    }

    // --- CICLO DE VIDA ---

    private void Start()
    {
        ActualizarEstado("¡Bienvenido! Prepárate para jugar...");
    }

    private void Update()
    {
        // --- Desaparición automática del mensaje principal ---
        if (contadorPrincipal > 0)
        {
            contadorPrincipal -= Time.deltaTime;

            if (contadorPrincipal <= 0 && textoEstado != null)
            {
                textoEstado.gameObject.SetActive(false);
            }
        }

        // --- Desaparición automática del mensaje secundario ---
        if (contadorSecundario > 0)
        {
            contadorSecundario -= Time.deltaTime;

            if (contadorSecundario <= 0 && textoSecundario != null)
            {
                textoSecundario.gameObject.SetActive(false);
            }
        }
    }
}
