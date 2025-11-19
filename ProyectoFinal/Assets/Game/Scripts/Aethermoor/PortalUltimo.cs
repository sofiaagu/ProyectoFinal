using System.Collections;
using UnityEngine;
using TMPro;

public class PortalUltimo : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelEstadisticas;
    public TextMeshProUGUI textoScore;
    public TextMeshProUGUI textoEnemigos;
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoVidas; // NUEVO: Para mostrar corazones
    public TextMeshProUGUI textoMensajeFinal;

    [Header("Timer")]
    public Timer timerScript; // Arrastra tu script Timer aquí

    void Start()
    {
        // Buscar el Timer automáticamente si no está asignado
        if (timerScript == null)
        {
            timerScript = FindObjectOfType<Timer>();
            if (timerScript != null)
            {
                Debug.Log("✅ Timer encontrado automáticamente");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró el script Timer en la escena");
            }
        }

        // Buscar el GameDataSaver automáticamente
        if (dataSaver == null)
        {
            dataSaver = FindObjectOfType<GameDataSaver>();
            if (dataSaver == null)
            {
                Debug.LogWarning("⚠️ No se encontró GameDataSaver - El botón de guardar no funcionará");
            }
        }

        // Asegurar que el panel esté oculto al inicio
        if (panelEstadisticas != null)
        {
            panelEstadisticas.SetActive(false);
        }
    }

    [Header("Visual del Portal")]
    public float velocidadRotacion = 100f;
    public GameObject efectoParticulasPortal;

    [Header("Configuración")]
    public string escenaSiguiente = "Menu"; // O la escena que quieras cargar
    public float tiempoParaMostrarPanel = 1f;

    [Header("Sistema de Guardado")]
    public GameDataSaver dataSaver; // Referencia al sistema de guardado

    private bool jugadorEnPortal = false;


    void Update()
    {
        // Rotar el portal para efecto visual
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !jugadorEnPortal)
        {
            jugadorEnPortal = true;
            StartCoroutine(MostrarPanelFinal());
        }
    }

    IEnumerator MostrarPanelFinal()
    {
        Debug.Log("🌟 ¡Jugador entró al portal!");

        // Esperar un momento
        yield return new WaitForSeconds(tiempoParaMostrarPanel);

        // Pausar el juego
        Time.timeScale = 0f;

        // Mostrar panel
        if (panelEstadisticas != null)
        {
            panelEstadisticas.SetActive(true);
            ActualizarEstadisticas();
        }

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ActualizarEstadisticas()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("❌ No se encontró GameManager");
            return;
        }

        // Score total
        if (textoScore != null)
        {
            textoScore.text = "Score Total: " + GameManager.instance.score;
        }

        // Enemigos eliminados
        if (textoEnemigos != null)
        {
            textoEnemigos.text = "Enemigos Eliminados: " + GameManager.instance.enemigosEliminados;
        }

        // Tiempo - Tomar directamente del Timer
        if (textoTiempo != null && timerScript != null)
        {
            // Detener el timer
            timerScript.TimerStop();

            // Obtener el tiempo final desde StopTime
            float tiempoFinal = timerScript.StopTime;

            // Formatear el tiempo igual que en el Timer
            int minutos = Mathf.FloorToInt(tiempoFinal / 60f);
            int segundos = Mathf.FloorToInt(tiempoFinal % 60f);
            int centesimas = Mathf.FloorToInt((tiempoFinal - (segundos + minutos * 60)) * 100f);

            textoTiempo.text = string.Format("Tiempo: {0:00}:{1:00}:{2:00}", minutos, segundos, centesimas);

            Debug.Log($"⏱️ Tiempo mostrado: {minutos:00}:{segundos:00}:{centesimas:00} (StopTime: {tiempoFinal})");
        }
        else
        {
            if (textoTiempo == null)
                Debug.LogWarning("⚠️ No hay TextoTiempo asignado");
            if (timerScript == null)
                Debug.LogWarning("⚠️ No hay Timer asignado");
        }

        // NUEVO: Vidas restantes (corazones)
        if (textoVidas != null)
        {
            Debug.Log("🔍 Buscando vidas del jugador...");

            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                Debug.Log("✅ Jugador encontrado: " + jugador.name);

                PlayerHealth playerHealth = jugador.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    int vidasRestantes = playerHealth.CurrentLives;
                    Debug.Log($"✅ PlayerHealth encontrado. Vidas: {vidasRestantes}");

                    // Opción 1: Mostrar con emojis de corazones
                    string corazones = "";
                    for (int i = 0; i < vidasRestantes; i++)
                    {
                        corazones += "❤️ ";
                    }
                    textoVidas.text = "Vidas Restantes: " + corazones + "(" + vidasRestantes + ")";
                    Debug.Log($"💖 Texto actualizado: {textoVidas.text}");

                    // Opción 2 (comentada): Solo número
                    // textoVidas.text = "Vidas Restantes: " + vidasRestantes;
                }
                else
                {
                    Debug.LogError("❌ El jugador NO tiene el componente PlayerHealth");
                    textoVidas.text = "Vidas: N/A (Sin PlayerHealth)";
                }
            }
            else
            {
                Debug.LogError("❌ No se encontró el jugador con tag 'Player'");
                textoVidas.text = "Vidas: N/A (Sin Jugador)";
            }
        }
        else
        {
            Debug.LogError("❌ textoVidas es NULL - No está asignado en el Inspector");
        }

        // Mensaje final personalizado
        if (textoMensajeFinal != null)
        {
            int score = GameManager.instance.score;
            string mensaje = "";

            if (score >= 100)
                mensaje = "🏆 ¡LEYENDA! Desempeño perfecto";
            else if (score >= 50)
                mensaje = "⭐ ¡Excelente trabajo!";
            else if (score >= 25)
                mensaje = "👍 Buen desempeño";
            else
                mensaje = "✅ ¡Misión completada!";

            textoMensajeFinal.text = mensaje;
        }
    }

    // Método para botón de continuar/salir
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.VolverAlMenuYResetear();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
        }
    }

    // Método para reiniciar nivel
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.ReiniciarJuego();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    // Método para el botón de guardar
    public void GuardarPartida()
    {
        if (dataSaver != null)
        {
            dataSaver.GuardarPartida();
        }
        else
        {
            Debug.LogError("❌ No hay GameDataSaver asignado");
        }
    }
}