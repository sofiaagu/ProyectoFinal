using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIOrdenarPalabras : MonoBehaviour
{
    public List<Button> botonesPalabras;
    public Button botonVerificar;

    // Lista interna con el orden actual de las palabras
    private List<string> palabrasActuales = new List<string>();

    // Índice del botón seleccionado para intercambiar
    private int indiceSeleccionado = -1;
    private AltarMemoria altar;

    public void Configurar(List<string> palabras, AltarMemoria altarMemoria)
    {
        altar = altarMemoria;

        // Copiar palabras recibidas a la lista interna
        palabrasActuales = new List<string>(palabras);

        // Configurar cada botón según las palabras disponibles
        for (int i = 0; i < botonesPalabras.Count; i++)
        {
            
            if (i < palabras.Count)
            {
                // Activar botón y asignar texto
                botonesPalabras[i].gameObject.SetActive(true);
                botonesPalabras[i].GetComponentInChildren<TMP_Text>().text = palabras[i];
                // Guardar el índice para que el botón sepa cuál es
                int index = i;
                // Limpiar listeners previos por seguridad
                botonesPalabras[i].onClick.RemoveAllListeners();
                // Asignar función cuando se presiona
                botonesPalabras[i].onClick.AddListener(() => SeleccionarBoton(index));
            }
            else
            {
                // Ocultar botones extra
                botonesPalabras[i].gameObject.SetActive(false);
            }
        }

        botonVerificar.onClick.RemoveAllListeners();
        botonVerificar.onClick.AddListener(VerificarOrden);
    }

    void SeleccionarBoton(int index)
    {
        // Si no había un botón seleccionado, seleccionar este
        if (indiceSeleccionado == -1)
        {
            indiceSeleccionado = index;
            botonesPalabras[index].image.color = Color.yellow;
        }
        else
        {
            // Intercambiar palabras entre los dos botones
            string temp = palabrasActuales[indiceSeleccionado];
            palabrasActuales[indiceSeleccionado] = palabrasActuales[index];
            palabrasActuales[index] = temp;
            // Actualizar textos de los botones
            ActualizarBotones();
            
            // Restaurar colores originales
            botonesPalabras[indiceSeleccionado].image.color = Color.white;
            botonesPalabras[index].image.color = Color.white;

            // Resetear selección
            indiceSeleccionado = -1;
        }
    }

    void ActualizarBotones()
    {
        for (int i = 0; i < palabrasActuales.Count; i++)
        {
            botonesPalabras[i].GetComponentInChildren<TMP_Text>().text = palabrasActuales[i];
        }
    }

    void VerificarOrden()
    {
        // Enviar lista a AltarMemoria para comparación
        altar.VerificarOrdenManual(palabrasActuales);
    }
}
