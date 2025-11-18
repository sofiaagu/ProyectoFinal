using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoColeccionable : MonoBehaviour
{
    [Header("Referencias")]
    public BossEnemy jefe; // Asigna el jefe en el Inspector
    public GameObject portal; // Asigna el portal que se activará
    public GameObject efectoRecoleccion; // Opcional: partículas al recoger

    [Header("Visual")]
    public float velocidadRotacion = 50f;
    public bool rotarObjeto = true;

    void Start()
    {
        // Asegurarse de que el portal esté desactivado al inicio
        if (portal != null)
        {
            portal.SetActive(false);
        }
    }

    void Update()
    {
        // Hacer que el objeto rote para que sea más visible
        if (rotarObjeto)
        {
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("🔍 Algo tocó el objeto: " + other.gameObject.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Es el jugador, recogiendo...");
            RecogerObjeto(other.gameObject);
        }
        else
        {
            Debug.LogWarning("⚠️ No es el jugador. Tag encontrado: " + other.tag);
        }
    }

    void RecogerObjeto(GameObject jugador)
    {
        Debug.Log("✅ ¡Objeto recogido! El jefe ha sido derrotado.");

        // Matar al jefe automáticamente
        if (jefe != null)
        {
            jefe.MorirPorObjeto();
        }
        else
        {
            Debug.LogWarning("⚠️ No hay referencia al jefe en el ObjetoColeccionable");
        }

        // Activar el portal
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("🌀 ¡Portal desbloqueado!");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay referencia al portal en el ObjetoColeccionable");
        }

        // Efecto visual opcional
        if (efectoRecoleccion != null)
        {
            Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);
        }

        // Desactivar el objeto
        gameObject.SetActive(false);
    }
}