using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaCollision : MonoBehaviour
{
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

            if (GameManager.instance != null)
            {
                GameManager.instance.EnemigoEliminado();
            }

            Destroy(other.gameObject);
        }
    }
}