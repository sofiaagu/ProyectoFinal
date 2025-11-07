using UnityEngine;

public class AreaNotifier : MonoBehaviour
{
    public EnemyContoller targetAgent; // tu enemigo asignado

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetAgent.StartChase(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetAgent.StopChase();
        }
    }
}
