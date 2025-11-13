using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller2 : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI palabrasRecolectadasText;

    [Header("Referencias de escena")]
    public AltarMemoria altarMemoria;
    public Timer timer; // ← referencia al temporizador de la escena

    private List<string> palabrasRecolectadas = new List<string>();
    [SerializeField] private int totalPalabras = 4;

    void Start()
    {
        ActualizarUI();
        if (altarMemoria != null)
            altarMemoria.gameObject.SetActive(false);
    }

    public void AgregarPalabra(string palabra)
    {
        if (!palabrasRecolectadas.Contains(palabra))
        {
            palabrasRecolectadas.Add(palabra);
            ActualizarUI();

            if (palabrasRecolectadas.Count >= totalPalabras)
            {
                ActivarAltar();
            }
        }
    }

    void ActualizarUI()
    {
        if (palabrasRecolectadasText != null)
            palabrasRecolectadasText.text = "Palabras: " + string.Join(" - ", palabrasRecolectadas);
    }

    void ActivarAltar()
    {
        if (altarMemoria != null)
        {
            altarMemoria.gameObject.SetActive(true);
            Debug.Log("Todas las palabras recolectadas. Altar activado.");
        }
    }

    // 👉 NUEVO MÉTODO
    public void CompletarEscena()
    {
        Debug.Log("Escena completada. Registrando tiempo y abriendo portal.");

        if (timer != null)
        {
            timer.TimerStop(); // Detiene el cronómetro
            if (GameManager.instance != null)
            {
                GameManager.instance.RegistrarTiempo(timer.StopTime);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el Timer en esta escena.");
        }

        // Aquí puedes cargar la siguiente escena
        // SceneManager.LoadScene("NombreDeLaSiguienteEscena");
    }

    public List<string> ObtenerPalabrasRecolectadas()
    {
        return new List<string>(palabrasRecolectadas);
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.ReiniciarJuego();
        }
        else
        {
            Debug.LogWarning("No se encontró el GameManager persistente.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
