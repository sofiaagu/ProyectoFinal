using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaDeteccion = 15f;
    public float distanciaAtaque = 2.5f;
    public float velocidadCaminar = 1.5f;
    public float velocidadCorrer = 3.5f;

    [Header("Nombres de Parámetros del Animator")]
    public string paramWalk = "walk";
    public string paramRun = "run";
    public string paramAttack = "attack";

    private NavMeshAgent agent;
    private GameObject target;
    private Animator ani;
    private bool atacando;
    private float tiempoEntreAtaques = 1.5f;
    private float siguienteAtaque = 0f;

    void Start()
    {
        Debug.Log("🟢 Boss Start() ejecutado");

        ani = GetComponent<Animator>();
        if (ani == null)
            Debug.LogError("❌ No hay Animator!");
        else
        {
            Debug.Log("✅ Animator encontrado");
            // Mostrar parámetros disponibles
            foreach (AnimatorControllerParameter param in ani.parameters)
            {
                Debug.Log($"   Parámetro: {param.name} (tipo: {param.type})");
            }
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("❌ No hay NavMeshAgent!");
        else
        {
            Debug.Log("✅ NavMeshAgent encontrado");
            agent.speed = velocidadCorrer;
            agent.stoppingDistance = distanciaAtaque;
            agent.updateRotation = true;
        }

        target = GameObject.Find("Player");
        if (target == null)
            Debug.LogError("❌ No se encontró Player!");
        else
            Debug.Log("✅ Player encontrado");
    }

    void Update()
    {
        if (agent == null || target == null || ani == null) return;

        float distancia = Vector3.Distance(transform.position, target.transform.position);

        // Si está atacando
        if (atacando)
        {
            agent.isStopped = true;
            ani.SetBool(paramWalk, false);
            ani.SetBool(paramRun, false);
            ani.SetBool(paramAttack, true);
            return;
        }

        // Jugador detectado
        if (distancia <= distanciaDeteccion)
        {
            agent.isStopped = false;

            // Muy cerca - Atacar
            if (distancia <= distanciaAtaque)
            {
                agent.isStopped = true;

                ani.SetBool(paramWalk, false);
                ani.SetBool(paramRun, false);
                ani.SetBool(paramAttack, false);

                if (Time.time >= siguienteAtaque)
                {
                    ani.SetBool(paramAttack, true);
                    atacando = true;
                    Invoke("FinalizarAtaque", 1.5f);
                }
            }
            // Perseguir
            else
            {
                agent.SetDestination(target.transform.position);
                agent.speed = velocidadCorrer;

                // FORZAR animación de correr
                ani.SetBool(paramAttack, false);
                ani.SetBool(paramWalk, false);
                ani.SetBool(paramRun, true);

                Debug.Log("🏃 Corriendo hacia el jugador - run=true");
            }
        }
        // Jugador lejos - idle
        else
        {
            agent.isStopped = true;
            ani.SetBool(paramWalk, false);
            ani.SetBool(paramRun, false);
            ani.SetBool(paramAttack, false);
        }

        // Debug
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"walk={ani.GetBool(paramWalk)} | run={ani.GetBool(paramRun)} | attack={ani.GetBool(paramAttack)}");
        }
    }

    void SetAnimBool(string param, bool value)
    {
        if (ani == null) return;

        // Intentar setear el parámetro si existe
        try
        {
            ani.SetBool(param, value);
        }
        catch
        {
            // El parámetro no existe, no hacer nada
        }
    }

    void FinalizarAtaque()
    {
        atacando = false;
        siguienteAtaque = Time.time + tiempoEntreAtaques;
        SetAnimBool(paramAttack, false);
    }
}