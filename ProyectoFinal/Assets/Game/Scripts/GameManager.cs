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

    // ============================================================
    // SCORE (MONEDAS)
    // ============================================================
    [Header("💰 Score Total (monedas globales)")]
    public int score = 0;
    public TextMeshProUGUI textoScore;

    [Header("⚔️ Enemigos")]
    public int enemigosEliminados = 0;
    public TextMeshProUGUI textoEnemigos;

    public int vidasPersistentes = -1;

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
    // 🎵 MUSICA GLOBAL (UN SOLO AUDIOSOURCE)
    // ============================================================
    [Header("🎵 Música")]
    public AudioSource audioSource;

    public AudioClip musicaMenu;
    public AudioClip musicaSactum;
    public AudioClip musicaOlvido;
    public AudioClip musicaBosqueLira;
    public AudioClip musicaAethermoor;

    [Range(0f, 1f)]
    public float volumenMaximo = 0.8f;

    public float fadeDuration = 1.5f;

    private bool isFading = false;

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

        ActualizarUI();

        // ============================================================
        // 🎵 Música según la escena
        // ============================================================
        ReproducirMusicaPorEscena(scene.name);
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
        enemigosEliminados++;
        ActualizarUI();
        ActualizarUIEnemigos();

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ResetDatos();
    }

    public void VolverAlMenuYResetear()
    {
        Time.timeScale = 1f;

        ResetDatos();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        SceneManager.LoadScene("Menu");

        Debug.Log("🏠 Volviendo al menú principal con reset global.");
    }

    public void ResetDatos()
    {
        score = 0;
        vidasPersistentes = -1;

        tiemposPorEscena.Clear();
        tiempoTotal = 0f;

        ActualizarUI();
    }


    // ============================================================
    // 🎵 MUSICA SEGÚN ESCENA + FADE
    // ============================================================

    public void ReproducirMusicaPorEscena(string escena)
    {
        AudioClip nuevaMusica = null;

        switch (escena)
        {
            case "Menu":
                nuevaMusica = musicaMenu;
                break;

            case "Sactum":
                nuevaMusica = musicaSactum;
                break;

            case "EscenaOlvido":
                nuevaMusica = musicaOlvido;
                break;

            case "BosqueDeLira":
                nuevaMusica = musicaBosqueLira;
                break;

            case "Aethermoor":
                nuevaMusica = musicaAethermoor;
                break;
        }

        if (nuevaMusica != null)
            StartCoroutine(CambiarMusicaConFade(nuevaMusica));
    }

    private System.Collections.IEnumerator CambiarMusicaConFade(AudioClip nuevaMusica)
    {
        if (isFading) yield break;
        isFading = true;

        // 🔻 Fade Out
        float t = 0f;
        float volumenInicial = audioSource.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.Play();

        // 🔺 Fade In
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, volumenMaximo, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = volumenMaximo;
        isFading = false;
    }
}
