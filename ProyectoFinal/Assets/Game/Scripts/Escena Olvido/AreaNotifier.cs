using UnityEngine;

public class AreaNotifier : MonoBehaviour
{
    public EnemyContoller targetAgent; // Asignar enemigo

    private void OnTriggerEnter(Collider other)
    {
        // Si lo que entra al área tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            // Inicia la persecución enviando la posición donde el jugador entró
            targetAgent.StartChase(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si el jugador sale del área
        if (other.CompareTag("Player"))
        {
            // Se detiene la persecución

            targetAgent.StopChase();
        }
    }
}
