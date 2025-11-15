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
    public float tiempoEntreMensajes = 2f;
    public int tamañoFuente = 24;
    public Color colorTexto = Color.white;
    public Color colorFondo = new Color(0, 0, 0, 0.7f);

    [Header("Configuración de Animación del Cubo")]
    public float alturaMovimiento = 2f;
    public float velocidadMovimiento = 2f;
    public float velocidadRotacion = 180f;

    [Header("Configuración de Sonido")]
    public AudioClip sonidoClick;
    public AudioClip sonidoMensaje;
    [Range(0f, 1f)]
    public float volumen = 0.5f;

    [Header("Configuración de Respawn")]
    public Transform nuevoPuntoRespawn; // NUEVO: Arrastra aquí el punto de respawn para este cubo

    private int indiceMensajeActual = 0;
    private bool mostrandoMensajes = false;
    private float tiempoTranscurrido = 0f;
    private string mensajeActual = "";
    private AudioSource audioSource;

    private Vector3 posicionInicial;
    private bool animandoCubo = false;
    private float progresoAnimacion = 0f;

    void Start()
    {
        posicionInicial = transform.position;

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
                    CambiarPuntoRespawn(); // NUEVO: Cambia el respawn al hacer click
                }
            }
        }

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

        AnimarCubo();
    }

    void AnimarCubo()
    {
        if (animandoCubo)
        {
            progresoAnimacion += Time.deltaTime * velocidadMovimiento;
            float offsetY = Mathf.Sin(progresoAnimacion) * alturaMovimiento;
            transform.position = posicionInicial + Vector3.up * offsetY;
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }
        else
        {
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

    // NUEVO MÉTODO: Cambia el punto de respawn del jugador
    void CambiarPuntoRespawn()
    {
        if (nuevoPuntoRespawn != null)
        {
            PlayerRespawn playerRespawn = FindFirstObjectByType<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.puntoRespawn = nuevoPuntoRespawn;
                Debug.Log("Punto de respawn cambiado a: " + nuevoPuntoRespawn.name);
            }
        }
    }

    void OnGUI()
    {
        if (mostrandoMensajes && !string.IsNullOrEmpty(mensajeActual))
        {
            GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
            estiloTexto.fontSize = tamañoFuente;
            estiloTexto.normal.textColor = colorTexto;
            estiloTexto.alignment = TextAnchor.MiddleCenter;
            estiloTexto.wordWrap = true;

            float anchoPanel = Screen.width * 0.8f;
            float altoPanel = 100f;
            float x = (Screen.width - anchoPanel) / 2;
            float y = Screen.height / 2 - altoPanel / 2;

            Texture2D fondoTextura = new Texture2D(1, 1);
            fondoTextura.SetPixel(0, 0, colorFondo);
            fondoTextura.Apply();
            GUI.DrawTexture(new Rect(x, y, anchoPanel, altoPanel), fondoTextura);

            GUI.Label(new Rect(x, y, anchoPanel, altoPanel), mensajeActual, estiloTexto);

            Destroy(fondoTextura);
        }
    }
}