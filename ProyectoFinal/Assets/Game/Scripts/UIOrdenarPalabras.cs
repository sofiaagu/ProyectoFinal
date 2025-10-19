using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIOrdenarPalabras : MonoBehaviour
{
    public List<Button> botonesPalabras;
    public Button botonVerificar;

    private List<string> palabrasActuales = new List<string>();
    private int indiceSeleccionado = -1;
    private AltarMemoria altar;

    public void Configurar(List<string> palabras, AltarMemoria altarMemoria)
    {
        altar = altarMemoria;
        palabrasActuales = new List<string>(palabras);

        for (int i = 0; i < botonesPalabras.Count; i++)
        {
            if (i < palabras.Count)
            {
                botonesPalabras[i].gameObject.SetActive(true);
                botonesPalabras[i].GetComponentInChildren<TMP_Text>().text = palabras[i];
                int index = i;
                botonesPalabras[i].onClick.RemoveAllListeners();
                botonesPalabras[i].onClick.AddListener(() => SeleccionarBoton(index));
            }
            else
            {
                botonesPalabras[i].gameObject.SetActive(false);
            }
        }

        botonVerificar.onClick.RemoveAllListeners();
        botonVerificar.onClick.AddListener(VerificarOrden);
    }

    void SeleccionarBoton(int index)
    {
        if (indiceSeleccionado == -1)
        {
            indiceSeleccionado = index;
            botonesPalabras[index].image.color = Color.yellow;
        }
        else
        {
            string temp = palabrasActuales[indiceSeleccionado];
            palabrasActuales[indiceSeleccionado] = palabrasActuales[index];
            palabrasActuales[index] = temp;
            ActualizarBotones();

            botonesPalabras[indiceSeleccionado].image.color = Color.white;
            botonesPalabras[index].image.color = Color.white;

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
        altar.VerificarOrdenManual(palabrasActuales);
    }
}
