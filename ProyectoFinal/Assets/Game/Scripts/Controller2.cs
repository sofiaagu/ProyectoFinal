using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Controller2 : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI palabrasRecolectadasText;

    [Header("Referencias de escena")]
    public AltarMemoria altarMemoria;

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

    public void CompletarEscena()
    {
        Debug.Log("Escena completada. Portal abierto.");
        // Aquí puedes llamar al siguiente nivel con SceneManager.LoadScene()
    }

    public List<string> ObtenerPalabrasRecolectadas()
    {
        return new List<string>(palabrasRecolectadas);
    }
}
