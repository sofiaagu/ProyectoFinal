using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // ============================================================
    // =========================== VIDAS ===========================
    // ============================================================

    [Header("❤️ Vidas del jugador")]
    public int maxVidas = 3;
    public int vidasActuales;
    public GameObject[] corazones;   // Iconos de vida

    // ============================================================
    // =========================== SCORE ===========================
    // ============================================================

    [Header("💰 Score Total (monedas globales)")]
    public int score = 0;
    public TextMeshProUGUI textoScore;

    // ============================================================
    // =========================== TIEMPO ==========================
    // ============================================================

    private Dictionary<string, float> tiemposPorEscena = new Dictionary<string, float>();
    private float tiempoTotal = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            vidasActuales = maxVidas;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar el texto del score en cada escena que tenga uno
        if (textoScore == null)
        {
            GameObject s = GameObject.FindWithTag("TextoScore");
            if (s != null)
                textoScore = s.GetComponent<TextMeshProUGUI>();
        }

        ActualizarUI();
    }

    // ============================================================
    // ======================= SCORE / MONEDAS =====================
    // ============================================================

    public void AgregarMoneda(int cantidad = 1)
    {
        score += cantidad;
        ActualizarUI();
    }

    // ============================================================
    // =========================== VIDAS ===========================
    // ============================================================

    public void PerderVida()
    {
        vidasActuales--;

        if (vidasActuales <= 0)
        {
            vidasActuales = 0;
            GameOver();
        }

        ActualizarUI();
    }

    public void GanarVida()
    {
        if (vidasActuales < maxVidas)
            vidasActuales++;

        ActualizarUI();
    }

    private void ActualizarUI()
    {
        // Actualizar corazones
        if (corazones != null && corazones.Length > 0)
        {
            for (int i = 0; i < corazones.Length; i++)
                corazones[i].SetActive(i < vidasActuales);
        }

        // Actualizar score si existe texto
        if (textoScore != null)
            textoScore.text = "x " + score;
    }

    // ============================================================
    // =========================== TIEMPO ==========================
    // ============================================================

    public void RegistrarTiempo(float tiempoEscena)
    {
        string escena = SceneManager.GetActiveScene().name;

        tiemposPorEscena[escena] = tiempoEscena;
        RecalcularTiempoTotal();
    }

    private void RecalcularTiempoTotal()
    {
        tiempoTotal = 0f;

        foreach (float t in tiemposPorEscena.Values)
            tiempoTotal += t;
    }

    public float ObtenerTiempoTotal() => tiempoTotal;

    // ============================================================
    // =========================== GAME OVER =======================
    // ============================================================

    private void GameOver()
    {
        Debug.Log("💀 GAME OVER");
        // Aquí puedes poner un panel de Game Over si tienes uno
        Time.timeScale = 0f;
    }

    // ============================================================
    // =========================== RESET ===========================
    // ============================================================

    public void ReiniciarJuego()
    {
        vidasActuales = maxVidas;
        score = 0;
        tiempoTotal = 0f;
        tiemposPorEscena.Clear();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ActualizarUI();
    }
}
