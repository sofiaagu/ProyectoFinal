using UnityEngine;

public class PuertaRompible : MonoBehaviour
{
    public int golpesNecesarios = 5;
    private int golpesRecibidos = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("arma"))
        {
            golpesRecibidos++;
            Debug.Log($"Puerta golpeada: {golpesRecibidos}/{golpesNecesarios}");

            if (golpesRecibidos >= golpesNecesarios)
            {
                Debug.Log("¡Puerta destruida!");
                gameObject.SetActive(false);
            }
        }
    }
}