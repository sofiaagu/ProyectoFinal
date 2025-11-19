using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragmento : MonoBehaviour
{
    // Referencias a UI e icono del fragmento
    public GameObject panelUI; // Panel que se muestra al interactuar
    public Texture2D iconoFragmento; // Icono que se agregará al inventario

    void Start()
    {
        if (panelUI != null)
        {
            panelUI.SetActive(false);// Asegurarse que el panel no esté activo
        }
    }

    void Update()
    {
        if (panelUI != null && panelUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarPanel();
        }
    }

    // Mostrar panel del fragmento y pausar el juego
    public void MostrarPanel()
    {
        Debug.Log("Mostrando panel del fragmento: " + gameObject.name);
        if (panelUI != null)
        {
            panelUI.SetActive(true);// Activar panel
            Time.timeScale = 0f;// Pausar juego
        }
    }

    public void CerrarPanel()
    {
        if (panelUI != null)
        {
            panelUI.SetActive(false); // Ocultar panel
        }
        Time.timeScale = 1f;// Reanudar juego
        Debug.Log("Panel cerrado, juego reanudado");
    }

    // Recolectar fragmento: agregar al inventario y notificar
    public void RecolectarFragmento()
    {
        Debug.Log("RecolectarFragmento llamado");

        Time.timeScale = 1f;// Asegurarse de reanudar el juego

        if (panelUI != null)
        {
            panelUI.SetActive(false); // Cerrar panel si estaba abierto
        }

        // Verificar que exista el Inventory global
        if (Inventory.instance != null)
        {
            if (iconoFragmento != null)
            {
                // Intentar agregar el fragmento al inventario
                bool agregado = Inventory.instance.AgregarFragmento(iconoFragmento);

                if (agregado)
                {
                    Debug.Log("¡Fragmento recolectado y agregado al inventario!");

                    // Notificar al SceneController que se recogió un fragmento
                    SceneController.instance?.RegistrarFragmento();

                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("No se pudo agregar al inventario (inventario lleno)");
                }
            }
            else
            {
                Debug.LogError("No se asignó la textura del fragmento!");
            }
        }
        else
        {
            Debug.LogError("No se encontró el Inventory!");
        }
    }
}