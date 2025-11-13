using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    [Header("Vidas del jugador")]
    public int maxVidas = 3;
    public int vidasActuales;
    public GameObject panelGameOver;
    [Header("UI opcional")]
    public TextMeshPro textoVidas; // Asigna si tienes un texto en pantalla
    public GameObject[] corazones; // Si usas imágenes de corazones

    void Awake()
    {
        // Singleton persistente
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            vidasActuales = maxVidas;

            // Escucha cuando cambia de escena, para actualizar UI
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Busca de nuevo el texto o corazones en la nueva escena (si existen)
        if (textoVidas == null)
        {
            GameObject t = GameObject.FindWithTag("TextoVidas");
            if (t != null)
                textoVidas = t.GetComponent<TextMeshPro>();
        }

        ActualizarUI();
    }

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
        {
            vidasActuales++;
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidasActuales;

        if (corazones.Length > 0)
        {
            for (int i = 0; i < corazones.Length; i++)
            {
                corazones[i].SetActive(i < vidasActuales);
            }
        }
    }

    void GameOver()
    {
        // Activa el panel Game Over
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        // Si quieres, puedes pausar el juego
        Time.timeScale = 0f;
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f; // reanudar el tiempo
        vidasActuales = maxVidas;
        ActualizarUI();

        // Puedes recargar la escena actual si quieres reiniciar todo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
