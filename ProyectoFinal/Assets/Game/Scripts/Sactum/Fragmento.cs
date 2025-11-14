using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragmento : MonoBehaviour
{
    public GameObject panelUI; // Cada fragmento tiene su propio panel

    void Start()
    {
        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }
    }

    public void MostrarPanel()
    {
        Debug.Log("Mostrando panel del fragmento: " + gameObject.name);
        panelUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RecolectarFragmento()
    {
        panelUI.SetActive(false);
        Time.timeScale = 1f;
        Destroy(gameObject);
        Debug.Log("¡Fragmento recolectado!");
    }
}