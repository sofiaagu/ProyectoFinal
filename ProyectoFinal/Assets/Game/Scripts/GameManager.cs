using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    // ============================================================
    // CONTROLLER DE LA ESCENA
    // ============================================================
    public Controller2 controllerActual;
    public ControllerSceneLira controllerActual2;
    public GameController4 controllerActual3;
    public SceneController controllerActual4;
    // ============================================================
    // SCORE (MONEDAS)
    // ============================================================
    [Header("💰 Score Total (monedas globales)")]
    public int score = 0;
    public TextMeshProUGUI textoScore;

    [Header("⚔️ Enemigos")]
    public int enemigosEliminados = 0;        // Contador
    public TextMeshProUGUI textoEnemigos;     // Opcional si quieres mostrarlo en UI

    public int vidasPersistentes = -1; // -1 significa "no inicializado"


    // ============================================================
    // GAME OVER
    // ============================================================
    [Header("💀 Game Over")]
    public GameObject panelGameOver;

    // ============================================================
    // TIEMPO
    // ============================================================
    private Dictionary<string, float> tiemposPorEscena = new Dictionary<string, float>();
    private float tiempoTotal = 0f;

    // ============================================================
    // INICIO
    // ============================================================
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Score
        GameObject s = GameObject.FindWithTag("TextoScore");
        if (s != null)
            textoScore = s.GetComponent<TextMeshProUGUI>();

        // GameOver Panel
        GameObject panel = GameObject.FindWithTag("PanelGameOver");
        if (panel != null)
        {
            panelGameOver = panel;
            panelGameOver.SetActive(false);
        }

        // Controller
        controllerActual = FindAnyObjectByType<Controller2>();
        controllerActual2 = FindAnyObjectByType<ControllerSceneLira>();
        controllerActual3 = FindAnyObjectByType<GameController4>();
        controllerActual4 = FindAnyObjectByType<SceneController>();
        ActualizarUI();
    }

    // ============================================================
    // SCORE
    // ============================================================
    public void AgregarMoneda(int cantidad = 1)
    {
        score += cantidad;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoScore != null)
            textoScore.text = "x " + score;
    }
    public void EnemigoEliminado()
    {
        enemigosEliminados++;              // Suma 1 al contador
        ActualizarUI();                    // Refresca el texto del score

        ActualizarUIEnemigos();            // Refresca el contador (si lo usas)

        Debug.Log("✔ Enemigo eliminado. Total: " + enemigosEliminados);
    }
    private void ActualizarUIEnemigos()
    {
        if (textoEnemigos != null)
            textoEnemigos.text = "Enemigos: " + enemigosEliminados;
    }

    // ============================================================
    // TIEMPO
    // ============================================================
    public void RegistrarTiempo(float tiempoEscena)
    {
        string escena = SceneManager.GetActiveScene().name;
        tiemposPorEscena[escena] = tiempoEscena;
        RecalcularTiempoTotal();
    }

    private void RecalcularTiempoTotal()
    {
        tiempoTotal = 0;
        foreach (float t in tiemposPorEscena.Values) tiempoTotal += t;
    }

    public float ObtenerTiempoTotal() => tiempoTotal;

    // ============================================================
    // GAME OVER
    // ============================================================
    public void MostrarGameOver()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        Time.timeScale = 0f;
    }

    // ============================================================
    // REINICIAR JUEGO
    // ============================================================
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        ResetDatos();
        SceneManager.LoadScene("Sactum");
    }

    public void VolverAlMenuYResetear()
    {
        Time.timeScale = 1f;

        ResetDatos(); // Reinicia score, tiempo y UI

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Apagar panel game over si está presente
        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        // Cargar escena del menú
        SceneManager.LoadScene("Menu");

        Debug.Log("🏠 Volviendo al menú principal con reset global.");
    }
    public void ResetDatos()
    {
        score = 0;
        vidasPersistentes = -1;

        // Reset de tiempo
        tiemposPorEscena.Clear();
        tiempoTotal = 0f;

        ActualizarUI();
    }


}
