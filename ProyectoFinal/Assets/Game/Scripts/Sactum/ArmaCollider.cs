using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("¡Golpeaste al enemigo!");

            // ⭐ AGREGAR ESTAS 4 LÍNEAS ⭐
            if (GameManager.instance != null)
            {
                GameManager.instance.EnemigoEliminado();
            }
            // ⭐ FIN DE LO NUEVO ⭐

            Destroy(other.gameObject); // Esta línea ya la tenías
        }
    }
}