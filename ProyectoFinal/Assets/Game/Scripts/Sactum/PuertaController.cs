using System.Collections;
using UnityEngine;

public class PuertaRompible : MonoBehaviour
{
    // Configuración de la puerta
    public int golpesNecesarios = 5; // Cuántos golpes necesita la puerta para romperse
    private int golpesRecibidos = 0; // Contador de golpes recibidos
    private bool puedeRecibirGolpe = true; // Para evitar contar golpes demasiado rápido

    // Detectar colisión con el arma
    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionar si el collider tiene tag "arma2" y puede recibir golpe
        if (other.CompareTag("arma2") && puedeRecibirGolpe)
        {
            golpesRecibidos++;
            Debug.Log($"Puerta golpeada: {golpesRecibidos}/{golpesNecesarios}");

            // Bloquear temporalmente para evitar golpes múltiples consecutivos
            puedeRecibirGolpe = false;
            StartCoroutine(DelayGolpe());

            // Si se alcanza la cantidad necesaria de golpes → destruir puerta
            if (golpesRecibidos >= golpesNecesarios)
            {
                Debug.Log("¡Puerta destruida!");
                gameObject.SetActive(false);
            }
        }
    }

    // Coroutine para retrasar el siguiente golpe
    private IEnumerator DelayGolpe()
    {
        yield return new WaitForSeconds(0.5f);// Espera 0.5 segundos
        puedeRecibirGolpe = true;// Permite recibir nuevo golpe
    }
}