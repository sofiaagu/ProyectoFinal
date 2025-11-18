using TMPro;
using UnityEngine;

public class UIestado : MonoBehaviour
{
    public TextMeshProUGUI textoEstado; 

    // Método para actualizar el texto
    public void ActualizarEstado(string nuevoTexto)
    {
        textoEstado.text = nuevoTexto;
    }
    void Start()
    {
        ActualizarEstado("¡Bienvenido! Prepárate para jugar...");
    }
}
