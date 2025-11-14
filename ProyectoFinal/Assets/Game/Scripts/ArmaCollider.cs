using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si chocó con un enemigo
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("¡Golpeaste al enemigo!");
            Destroy(other.gameObject);
        }
    }
}