using UnityEngine;

public class Caida : MonoBehaviour
{
    public Transform puntoRespawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (puntoRespawn != null)
            {
                other.transform.position = puntoRespawn.position;
                Debug.Log("¡Jugador teletransportado!");
            }
            else
            {
                Debug.LogError("No se asignó un punto de respawn");
            }
        }
    }
}