using UnityEngine;

public class CuboClickeable : MonoBehaviour
{
    [Header("Configuración de Mensajes")]
    [TextArea(3, 10)]
    public string[] mensajes = new string[]
    {
        "¡Hola! Has clickeado el cubo",
        "Este es el segundo mensaje",
        "Y este es el tercero",
        "Puedes agregar los mensajes que quieras"
    };

    [Header("Configuración Visual")]
    public float tiempoEntreMensajes = 2f; // Tiempo que dura cada mensaje
    public int tamañoFuente = 24;
    public Color colorTexto = Color.white;
    public Color colorFondo = new Color(0, 0, 0, 0.7f); // Negro semi-transparente

    [Header("Configuración de Animación del Cubo")]
    public float alturaMovimiento = 2f; // Qué tan alto sube el cubo
    public float velocidadMovimiento = 2f; // Velocidad de subida/bajada
    public float velocidadRotacion = 180f; // Grados por segundo de rotación

    [Header("Configuración de Sonido")]
    public AudioClip sonidoClick; // Sonido al hacer click en el cubo
    public AudioClip sonidoMensaje; // Sonido al aparecer cada mensaje
    [Range(0f, 1f)]
    public float volumen = 0.5f;

    private int indiceMensajeActual = 0;
    private bool mostrandoMensajes = false;
    private float tiempoTranscurrido = 0f;
    private string mensajeActual = "";
    private AudioSource audioSource;

    // Variables para la animación del cubo
    private Vector3 posicionInicial;
    private bool animandoCubo = false;
    private float progresoAnimacion = 0f;

    void Start()
    {
        // Guardar posición inicial del cubo
        posicionInicial = transform.position;

        // Crear o obtener AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = volumen;
    }

    void Update()
    {
        // Detectar click en el cubo
        if (Input.GetMouseButtonDown(0) && !mostrandoMensajes)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    IniciarMensajes();
                    ReproducirSonido(sonidoClick);
                }
            }
        }

        // Gestionar la muestra de mensajes
        if (mostrandoMensajes)
        {
            tiempoTranscurrido += Time.deltaTime;

            if (tiempoTranscurrido >= tiempoEntreMensajes)
            {
                indiceMensajeActual++;
                tiempoTranscurrido = 0f;

                if (indiceMensajeActual >= mensajes.Length)
                {
                    FinalizarMensajes();
                }
                else
                {
                    mensajeActual = mensajes[indiceMensajeActual];
                    ReproducirSonido(sonidoMensaje);
                }
            }
        }

        // Animar el cubo
        AnimarCubo();
    }

    void AnimarCubo()
    {
        if (animandoCubo)
        {
            // Movimiento de arriba hacia abajo (efecto de rebote)
            progresoAnimacion += Time.deltaTime * velocidadMovimiento;
            float offsetY = Mathf.Sin(progresoAnimacion) * alturaMovimiento;
            transform.position = posicionInicial + Vector3.up * offsetY;

            // Rotación continua
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }
        else
        {
            // Volver suavemente a la posición original
            transform.position = Vector3.Lerp(transform.position, posicionInicial, Time.deltaTime * 5f);
        }
    }

    void IniciarMensajes()
    {
        mostrandoMensajes = true;
        animandoCubo = true;
        indiceMensajeActual = 0;
        tiempoTranscurrido = 0f;
        progresoAnimacion = 0f;
        mensajeActual = mensajes[0];
        ReproducirSonido(sonidoMensaje);
    }

    void FinalizarMensajes()
    {
        mostrandoMensajes = false;
        animandoCubo = false;
        mensajeActual = "";
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volumen);
        }
    }

    void OnGUI()
    {
        if (mostrandoMensajes && !string.IsNullOrEmpty(mensajeActual))
        {
            // Configurar estilo del texto
            GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
            estiloTexto.fontSize = tamañoFuente;
            estiloTexto.normal.textColor = colorTexto;
            estiloTexto.alignment = TextAnchor.MiddleCenter;
            estiloTexto.wordWrap = true;

            // Calcular dimensiones
            float anchoPanel = Screen.width * 0.8f;
            float altoPanel = 100f;
            float x = (Screen.width - anchoPanel) / 2;
            float y = Screen.height / 2 - altoPanel / 2;

            // Dibujar fondo
            Texture2D fondoTextura = new Texture2D(1, 1);
            fondoTextura.SetPixel(0, 0, colorFondo);
            fondoTextura.Apply();
            GUI.DrawTexture(new Rect(x, y, anchoPanel, altoPanel), fondoTextura);

            // Dibujar texto
            GUI.Label(new Rect(x, y, anchoPanel, altoPanel), mensajeActual, estiloTexto);

            // Limpiar textura
            Destroy(fondoTextura);
        }
    }
}