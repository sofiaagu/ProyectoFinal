using UnityEngine;
using UnityEngine.Audio;

public enum TipoSantuario
{
    Agua,
    Tierra,
    Luz
}

public class SantuarioClick : MonoBehaviour
{
    [Header("Tipo de Santuario")]
    public TipoSantuario tipo;

    [Header("Duración del mensaje")]
    public float duracion = 6f;

    [Header("Sonido al hacer clic")]
    public AudioClip sonidoClick;

    private AudioSource audioSource;

    [Header("Animación del Objeto")]
    public float alturaMovimiento = 0.2f;
    public float velocidadMovimiento = 2f;
    public float velocidadRotacion = 45f;

    private Vector3 posicionInicial;
    private float progresoAnimacion = 0f;

    private ControllerSceneLira controller;

    void Start()
    {
        posicionInicial = transform.position;

        controller = FindObjectOfType<ControllerSceneLira>();

        if (controller == null)
            Debug.LogError("ERROR: No se encontró ControllerSceneLira en la escena.");

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        AnimarObjeto();
    }

    void AnimarObjeto()
    {
        // Rebote
        progresoAnimacion += Time.deltaTime * velocidadMovimiento;
        float offsetY = Mathf.Sin(progresoAnimacion) * alturaMovimiento;
        transform.position = posicionInicial + Vector3.up * offsetY;

        // Rotación
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
    }

    void OnMouseDown()
    {
       
        if (sonidoClick != null)
            audioSource.PlayOneShot(sonidoClick);

     
        if (controller != null)
        {
            controller.MostrarMensaje(ObtenerMensajePorTipo(), duracion);
        }
    }

    string ObtenerMensajePorTipo()
    {
        switch (tipo)
        {
            case TipoSantuario.Agua:
                return "Bienvenido al Santuario de Agua.\n\nActiva las gemas en el orden correcto para liberar la semilla de Agua.";

            case TipoSantuario.Tierra:
                return "Bienvenido al Santuario de la Tierra.\n\nLibera la semilla de Tierra activando las tumbas encendidas de los cinco fuegos sagrados sin equivocarte.";

            case TipoSantuario.Luz:
                return "Bienvenido al Santuario de la Luz.\n\nBusca, encuentra e ilumina las antorchas sagradas para liberar la semilla de Luz.";

            default:
                return "Bienvenido al Santuario.";
        }
    }
}
