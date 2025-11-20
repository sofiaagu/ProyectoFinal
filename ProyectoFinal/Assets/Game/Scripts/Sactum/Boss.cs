using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator ani;
    public Quaternion angulo;
    public float grado;
    public GameObject target;
    public bool atacando;
    private float tiempoEntreAtaques = 0.5f;
    private float siguienteAtaque = 0f;

    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("Player");
    }

    void Update()
    {
        Comportamiento_Enemigo();
    }

    public void Comportamiento_Enemigo()
    {
        // SI ESTÁ ATACANDO, NO HACER NADA - mantener posición fija
        if (atacando)
        {
            return; // Sale inmediatamente, SIN rotar
        }

        float distancia = Vector3.Distance(transform.position, target.transform.position);

        if (distancia > 10)
        {
            // Lejos del jugador - comportamiento aleatorio
            ani.SetBool("run", false);
            ani.SetBool("attack", false);

            cronometro += 1 * Time.deltaTime;
            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    ani.SetBool("walk", false);
                    break;

                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;

                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    ani.SetBool("walk", true);
                    break;
            }
        }
        else if (distancia > 5)
        {
            // Cerca del jugador - perseguir
            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);

            ani.SetBool("walk", false);
            ani.SetBool("run", true);
            ani.SetBool("attack", false);

            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }
        else
        {
            // Muy cerca del jugador - zona de ataque
            ani.SetBool("walk", false);
            ani.SetBool("run", false);

            // Mirar hacia el jugador SOLO antes de atacar
            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 5); // Velocidad aumentada
            }

            // Iniciar ataque si es posible
            if (Time.time >= siguienteAtaque)
            {
                ani.SetBool("attack", true);
                atacando = true;
            }
        }
    }

    // Este método es llamado por el Animation Event al final de la animación
    public void Final_Ani()
    {
        ani.SetBool("attack", false);
        atacando = false;
        siguienteAtaque = Time.time + tiempoEntreAtaques;
    }
}