using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragmento : MonoBehaviour
{
    public GameObject panelUI;
    public Texture2D iconoFragmento;

    [Header("Sonido")]
    public AudioClip sonidoRecoleccion;
    private AudioSource audioSource;

    void Start()
    {
        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (panelUI != null && panelUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarPanel();
        }
    }

    public void MostrarPanel()
    {
        Debug.Log("Mostrando panel del fragmento: " + gameObject.name);
        if (panelUI != null)
        {
            panelUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CerrarPanel()
    {
        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }
        Time.timeScale = 1f;
        Debug.Log("Panel cerrado, juego reanudado");
    }

    public void RecolectarFragmento()
    {
        Debug.Log("RecolectarFragmento llamado");

        Time.timeScale = 1f;

        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }

        if (Inventory.instance != null)
        {
            if (iconoFragmento != null)
            {
                bool agregado = Inventory.instance.AgregarFragmento(iconoFragmento);

                if (agregado)
                {
                    Debug.Log("Fragmento recolectado y agregado al inventario");

                    if (sonidoRecoleccion != null)
                        audioSource.PlayOneShot(sonidoRecoleccion);

                    SceneController.instance?.RegistrarFragmento();

                    Destroy(gameObject, sonidoRecoleccion != null ? sonidoRecoleccion.length : 0f);
                }
                else
                {
                    Debug.Log("No se pudo agregar al inventario (inventario lleno)");
                }
            }
            else
            {
                Debug.LogError("No se asignó la textura del fragmento");
            }
        }
        else
        {
            Debug.LogError("No se encontró el Inventory");
        }
    }
}