using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    [Header("❤️ Vidas del jugador")]
    public int maxVidas = 3;
    public int vidasActuales;
    public GameObject panelGameOver;

    [Header("💰 Monedas")]
    public int monedasTotales = 0;                 // Monedas acumuladas en todas las escenas
    public TextMeshProUGUI textoMonedas;           // Texto UI opcional para mostrar monedas

    [Header("⏱️ Tiempo")]
    private Dictionary<string, float> tiemposPorEscena = new Dictionary<string, float>();
    private float tiempoTotal = 0f;
    public TextMeshProUGUI textoTiempoTotal;       // (Opcional) Mostrar tiempo total

    [Header("🖥️ UI de vidas")]
    public TextMeshProUGUI textoVidas;
    public GameObject[] corazones;

    private void Awake()
    {
        // Configurar Singleton persistente
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
        // Reasignar referencias de UI cuando cambia de escena
        if (textoVidas == null)
        {
            GameObject t = GameObject.FindWithTag("TextoVidas");
            if (t != null) textoVidas = t.GetComponent<TextMeshProUGUI>();
        }

        if (textoMonedas == null)
        {
            GameObject m = GameObject.FindWithTag("TextoMonedas");
            if (m != null) textoMonedas = m.GetComponent<TextMeshProUGUI>();
        }

        ActualizarUI();
    }

    // ============================================================
    // ===================== SISTEMA DE MONEDAS ===================
    // ============================================================
    public void AgregarMoneda(int cantidad = 1)
    {
        monedasTotales += cantidad;
        ActualizarUI();
        Debug.Log($"💰 Monedas totales: {monedasTotales}");
    }

    public void QuitarMoneda(int cantidad = 1)
    {
        monedasTotales = Mathf.Max(0, monedasTotales - cantidad);
        ActualizarUI();
    }

    // ============================================================
    // ===================== SISTEMA DE VIDAS =====================
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

    // ============================================================
    // ===================== CONTROL DE TIEMPO ====================
    // ============================================================
    public void RegistrarTiempo(float tiempoEscena)
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (!tiemposPorEscena.ContainsKey(escenaActual))
        {
            tiemposPorEscena.Add(escenaActual, tiempoEscena);
        }
        else
        {
            tiemposPorEscena[escenaActual] = tiempoEscena;
        }

        CalcularTiempoTotal();
        Debug.Log($"⏱️ Tiempo registrado en {escenaActual}: {tiempoEscena:F2}s | Total: {tiempoTotal:F2}s");
    }

    private void CalcularTiempoTotal()
    {
        tiempoTotal = 0f;
        foreach (float t in tiemposPorEscena.Values)
            tiempoTotal += t;
    }

    public float ObtenerTiempoTotal() => tiempoTotal;

    // ============================================================
    // ===================== ACTUALIZAR UI ========================
    // ============================================================
    private void ActualizarUI()
    {
        if (textoVidas != null)
            textoVidas.text = $"Vidas: {vidasActuales}";

        if (corazones != null && corazones.Length > 0)
        {
            for (int i = 0; i < corazones.Length; i++)
                corazones[i].SetActive(i < vidasActuales);
        }

        if (textoMonedas != null)
            textoMonedas.text = $"Monedas: {monedasTotales}";

        if (textoTiempoTotal != null)
            textoTiempoTotal.text = $"Tiempo total: {tiempoTotal:F2}s";
    }

    // ============================================================
    // ===================== GAME OVER ============================
    // ============================================================
    private void GameOver()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        if (textoTiempoTotal != null)
            textoTiempoTotal.text = $"Tiempo total: {tiempoTotal:F2}s";

        Time.timeScale = 0f;
        Debug.Log("💀 GAME OVER");
    }

    // ============================================================
    // ===================== REINICIAR JUEGO ======================
    // ============================================================
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        vidasActuales = maxVidas;
        tiemposPorEscena.Clear();
        tiempoTotal = 0f;
        monedasTotales = 0;
        ActualizarUI();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("🔄 Juego reiniciado.");
    }
}
