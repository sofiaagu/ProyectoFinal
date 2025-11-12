using UnityEngine;
using UnityEngine.AI;

public class Caida : MonoBehaviour
{
    public Transform playerTarget;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        agent.enabled = false;
    }

    void Update()
    {
        
        if (agent.enabled && playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    public void StartChase(Vector3 playerPos)
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
            agent.SetDestination(playerPos);
            Debug.Log(gameObject.name + " activado. Iniciando persecución.");
        }
    }

    public void StopChase()
    {
        agent.enabled = false;
        Debug.Log(gameObject.name + " detenido.");
    }
}
