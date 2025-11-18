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
    public TextMeshProUGUI textoMensajeFinal;

    [Header("Visual del Portal")]
    public float velocidadRotacion = 100f;
    public GameObject efectoParticulasPortal;

    [Header("Configuración")]
    public string escenaSiguiente = "Menu"; // O la escena que quieras cargar
    public float tiempoParaMostrarPanel = 1f;

    private bool jugadorEnPortal = false;

    void Start()
    {
        // Asegurar que el panel esté oculto al inicio
        if (panelEstadisticas != null)
        {
            panelEstadisticas.SetActive(false);
        }
    }

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

        // Tiempo total
        if (textoTiempo != null)
        {
            float tiempoTotal = GameManager.instance.ObtenerTiempoTotal();
            int minutos = Mathf.FloorToInt(tiempoTotal / 60f);
            int segundos = Mathf.FloorToInt(tiempoTotal % 60f);
            textoTiempo.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);
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
}