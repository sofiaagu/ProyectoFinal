using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaCollision : MonoBehaviour
{
    // Se ejecuta automáticamente cuando otro collider entra en el trigger del arma
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el BOSS
        BossVida boss = other.GetComponent<BossVida>();
        if (boss != null)
        {
            Debug.Log("¡Golpeaste al BOSS!");
            boss.RecibirDaño(20f); // 20 de daño al boss
            return; // No destruir, solo hacer daño
        }

        // Si NO es el boss, entonces es un enemigo normal
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("¡Golpeaste al enemigo!");
            // Si existe el GameManager global, actualizar contador de enemigos eliminados
            if (GameManager.instance != null)
            {
                GameManager.instance.EnemigoEliminado();
            }
            // Destruye el enemigo de la escena
            Destroy(other.gameObject);
        }
    }
}