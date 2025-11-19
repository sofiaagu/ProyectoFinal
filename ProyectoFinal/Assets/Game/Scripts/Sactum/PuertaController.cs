using System.Collections;
using UnityEngine;

public class PuertaRompible : MonoBehaviour
{
    public int golpesNecesarios = 5;
    private int golpesRecibidos = 0;
    private bool puedeRecibirGolpe = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("arma2") && puedeRecibirGolpe)
        {
            golpesRecibidos++;
            Debug.Log($"Puerta golpeada: {golpesRecibidos}/{golpesNecesarios}");

            puedeRecibirGolpe = false;
            StartCoroutine(DelayGolpe());

            if (golpesRecibidos >= golpesNecesarios)
            {
                Debug.Log("¡Puerta destruida!");
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator DelayGolpe()
    {
        yield return new WaitForSeconds(0.5f);
        puedeRecibirGolpe = true;
    }
}