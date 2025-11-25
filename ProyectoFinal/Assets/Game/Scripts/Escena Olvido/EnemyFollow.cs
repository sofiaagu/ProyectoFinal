using UnityEngine;
using UnityEngine.AI;

public class EnemyContoller : MonoBehaviour
{
    public Transform playerTarget;// Referencia al transform del jugador 
    private NavMeshAgent agent;  // Componente que permite mover al enemigo usando el NavMesh

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();// Obtiene el NavMeshAgent que controla el movimiento
        // Inicia deshabilitado
        agent.enabled = false;
    }

    void Update()
    {
        // Si el agente está encendido y tiene un objetivo, lo persigue
        if (agent.enabled && playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);// Actualiza la ruta hacia la posición actual del jugador cada frame
        }
    }

    public void StartChase(Vector3 playerPos)
    {
        // Método que se llama cuando el jugador entra al área de detección

        if (!agent.enabled)
        {
            agent.enabled = true; // Activa el navmesh agent (desde este punto empieza a moverse)
            agent.SetDestination(playerPos);// Le damos una primera posición del jugador (aunque luego en Update lo sigue dinámicamente)
            Debug.Log(gameObject.name + " activado. Iniciando persecución.");
        }
    }

    public void StopChase()
    {
        // Método llamado cuando el jugador sale del área de detección
        agent.enabled = false;
        // Se apaga el navmesh agent, deteniendo totalmente su movimiento y cálculo de rutas
        Debug.Log(gameObject.name + " detenido.");
    }
}
