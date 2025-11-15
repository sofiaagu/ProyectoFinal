using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaController : MonoBehaviour
{
    public int enemigosRequeridosParaAbrir = 10;
    private bool puertaAbierta = false;

    void Start()
    {
        Debug.Log("🚪 PuertaController iniciado en: " + gameObject.name);

        if (GameManager.instance == null)
        {
            Debug.LogError("❌ GameManager.instance es NULL!");
        }
        else
        {
            Debug.Log("✅ GameManager encontrado. Enemigos eliminados: " + GameManager.instance.enemigosEliminados);
        }
    }

    void Update()
    {
        if (puertaAbierta) return; // Ya se abrió, no verificar más

        if (GameManager.instance != null)
        {
            // Verificar si ya llegó al número requerido
            if (GameManager.instance.enemigosEliminados >= enemigosRequeridosParaAbrir)
            {
                AbrirPuerta();
            }
        }
    }

    void AbrirPuerta()
    {
        puertaAbierta = true;
        Debug.Log("🎉 ¡Puerta abierta! Enemigos eliminados: " + GameManager.instance.enemigosEliminados);
        gameObject.SetActive(false); // Desaparecer la puerta
    }
}