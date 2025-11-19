using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Controller2 : MonoBehaviour
{
    // REFERENCIAS

    [Header("UI")]
    public TextMeshProUGUI palabrasRecolectadasText;
    public TextMeshProUGUI textoMonedas;

    [Header("Referencias de escena")]
    public AltarMemoria altarMemoria;
    public Timer timer;

    
    // VARIABLES INTERNAS

    private Stack<string> pilaPalabras = new Stack<string>();
    private int monedasEscena = 0;

    [SerializeField] private int totalPalabras = 4;

    //START

    private void Start()
    {
        if (timer == null)
            timer = FindFirstObjectByType<Timer>();

        ActualizarUI();

        if (altarMemoria != null)
            altarMemoria.gameObject.SetActive(false);
    }


    // SISTEMA DE PALABRAS
    public void AgregarPalabra(string palabra)
{
    pilaPalabras.Push(palabra); 
    ActualizarUI();

    if (pilaPalabras.Count >= totalPalabras)
        ActivarAltar();
}
    private void ActivarAltar()
    {
        if (altarMemoria == null)
            return;

        altarMemoria.gameObject.SetActive(true);
        Debug.Log("Todas las palabras recolectadas. Altar activado.");
    }


    // MONEDAS

    public void RegistrarMoneda(int cantidad)
    {
        monedasEscena += cantidad;
        ActualizarUI();

        if (GameManager.instance != null)
            GameManager.instance.AgregarMoneda(cantidad);

        Debug.Log($"Moneda recogida | Escena: {monedasEscena}");
    }

    // FINAL DE ESCENA

    public void CompletarEscena()
    {
        Debug.Log("Escena completada. Registrando tiempo…");

        if (timer != null)
        {
            timer.TimerStop();

            if (GameManager.instance != null)
                GameManager.instance.RegistrarTiempo(timer.StopTime);
        }
    }

    // UI UPDATE

    private void ActualizarUI()
    {
        if (palabrasRecolectadasText != null)
            palabrasRecolectadasText.text = "Palabras: " + string.Join(" - ", pilaPalabras);

        if (textoMonedas != null)
            textoMonedas.text = "Monedas: " + monedasEscena;
    }


    // MÉTODOS AUXILIARES

    public List<string> ObtenerPalabrasRecolectadas()
    {
        return new List<string>(pilaPalabras); // se convierte a lista para la UI
    }
   
}
