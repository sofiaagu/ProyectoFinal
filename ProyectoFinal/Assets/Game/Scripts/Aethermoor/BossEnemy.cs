using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public Transform objetoADefender; // El objeto que el jefe defiende
    public Animator anim;

    [Header("Configuración de Combate")]
    public float rangoDeteccion = 15f;
    public float rangoAtaque1 = 3f; // Ataque cuerpo a cuerpo
    public float rangoAtaque2 = 8f; // Ataque a distancia
    public float tiempoEntreAtaques = 2f;

    [Header("Movimiento")]
    public float velocidadMovimiento = 3f;
    public float velocidadRotacion = 5f;
    public float distanciaMinima = 2f; // Distancia mínima al objeto

    [Header("Estado")]
    public bool estaVivo = true;

    private float tiempoUltimoAtaque;
    private bool estaAtacando = false;
    private CharacterController controller;

    void Start()
    {
        // Buscar referencias automáticamente si no están asignadas
        if (jugador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                jugador = player.transform;
        }

        if (anim == null)
            anim = GetComponent<Animator>();

        controller = GetComponent<CharacterController>();

        tiempoUltimoAtaque = Time.time;
    }

    void Update()
    {
        if (!estaVivo || jugador == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        // Defender el área del objeto
        DefenderObjeto(distanciaAlJugador);
    }

    void DefenderObjeto(float distanciaAlJugador)
    {
        // Si el jugador está cerca, atacar
        if (distanciaAlJugador <= rangoDeteccion)
        {
            MirarAlJugador();

            // Verificar si puede atacar
            if (Time.time - tiempoUltimoAtaque >= tiempoEntreAtaques && !estaAtacando)
            {
                // Elegir ataque según distancia
                if (distanciaAlJugador <= rangoAtaque1)
                {
                    StartCoroutine(RealizarAtaque1());
                }
                else if (distanciaAlJugador <= rangoAtaque2)
                {
                    StartCoroutine(RealizarAtaque2());
                }
                else if (distanciaAlJugador > rangoAtaque2)
                {
                    // Acercarse al jugador
                    MoverHaciaJugador();
                }
            }
        }
        else
        {
            // Patrullar cerca del objeto
            PatrullarCercaDelObjeto();
        }
    }

    void MoverHaciaJugador()
    {
        if (anim != null)
            anim.SetBool("corriendo", true);

        Vector3 direccion = (jugador.position - transform.position).normalized;

        if (controller != null)
        {
            controller.Move(direccion * velocidadMovimiento * Time.deltaTime);
        }
        else
        {
            transform.position += direccion * velocidadMovimiento * Time.deltaTime;
        }
    }

    void MirarAlJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0;

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }
    }

    void PatrullarCercaDelObjeto()
    {
        if (objetoADefender == null) return;

        float distanciaAlObjeto = Vector3.Distance(transform.position, objetoADefender.position);

        if (distanciaAlObjeto > distanciaMinima + 2f)
        {
            // Volver al objeto
            Vector3 direccion = (objetoADefender.position - transform.position).normalized;

            if (controller != null)
            {
                controller.Move(direccion * velocidadMovimiento * 0.5f * Time.deltaTime);
            }
            else
            {
                transform.position += direccion * velocidadMovimiento * 0.5f * Time.deltaTime;
            }

            if (anim != null)
                anim.SetBool("corriendo", true);
        }
        else
        {
            if (anim != null)
                anim.SetBool("corriendo", false);
        }
    }

    IEnumerator RealizarAtaque1()
    {
        estaAtacando = true;
        tiempoUltimoAtaque = Time.time;

        if (anim != null)
        {
            anim.SetBool("corriendo", false);
            anim.SetTrigger("ataque1");
        }

        // Esperar a que la animación llegue al momento del golpe
        yield return new WaitForSeconds(0.5f);

        // Verificar si el jugador está en rango
        float distancia = Vector3.Distance(transform.position, jugador.position);
        if (distancia <= rangoAtaque1)
        {
            MatarJugador();
        }

        yield return new WaitForSeconds(0.5f);
        estaAtacando = false;
    }

    IEnumerator RealizarAtaque2()
    {
        estaAtacando = true;
        tiempoUltimoAtaque = Time.time;

        if (anim != null)
        {
            anim.SetBool("corriendo", false);
            anim.SetTrigger("ataque2");
        }

        // Esperar a que la animación llegue al momento del ataque
        yield return new WaitForSeconds(0.7f);

        // Verificar si el jugador está en rango
        float distancia = Vector3.Distance(transform.position, jugador.position);
        if (distancia <= rangoAtaque2)
        {
            MatarJugador();
        }

        yield return new WaitForSeconds(0.8f);
        estaAtacando = false;
    }

    void MatarJugador()
    {
        PlayerHealth playerHealth = jugador.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Instakill: quitar todas las vidas
            playerHealth.TakeDamage(playerHealth.CurrentLives);
            Debug.Log("💀 ¡El jefe mató al jugador!");
        }
    }

    // Llamar este método cuando el jugador recoja el objeto
    public void MorirPorObjeto()
    {
        if (!estaVivo) return;

        estaVivo = false;

        if (anim != null)
            anim.SetTrigger("muerte");

        // Notificar al GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.EnemigoEliminado();
        }

        Debug.Log("💀 ¡El jefe ha sido derrotado por el objeto sagrado!");

        // Desactivar el jefe después de la animación
        StartCoroutine(DesactivarDespuesDeMorir());
    }

    IEnumerator DesactivarDespuesDeMorir()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    // Visualizar rangos en el editor
    void OnDrawGizmosSelected()
    {
        // Rango de detección (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        // Rango ataque 1 (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque1);

        // Rango ataque 2 (naranja)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, rangoAtaque2);

        // Línea al objeto a defender (verde)
        if (objetoADefender != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, objetoADefender.position);
        }
    }
}