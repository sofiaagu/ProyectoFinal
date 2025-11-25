using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //  CONFIGURACIÓN DE MOVIMIENTO
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    public float fuerzaSalto = 8.0f;
    public float gravedad = 20.0f;

    // CONTROL DE LA CÁMARA 
    [Header("Rotación de Cámara")]
    public Transform camaraTransform; // Cámara en primera/tercera persona
    public float sensibilidadRaton = 3.0f;// Sensibilidad del mouse
    public float limiteVerticalMin = -40f;  // Límite al mirar hacia abajo
    public float limiteVerticalMax = 80f; // Límite al mirar hacia arriba

 // SISTEMA DE VIDAS
    [Header("Vida del jugador")]
    public float tiempoInvulnerable = 2f;
    private bool invulnerable = false;

    private CharacterController controller;
    private Animator anim;
    private Vector3 movimiento;
    private float x, y;
    private bool puedoSaltar = false;
    private bool estaCayendo = false;

    private float rotacionVertical = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

      // Si no asignas la cámara manualmente, la busca automáticamente
        if (camaraTransform == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
                camaraTransform = cam.transform;
        }
    }

    void Update()
    {
        // Rotación de cámara con botón derecho
        if (Input.GetMouseButton(1)) // Botón derecho del mouse
        {
            float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

            // Rotar jugador horizontalmente
            transform.Rotate(0, mouseX, 0);

            // Rotar cámara verticalmente
            if (camaraTransform != null)
            {
                rotacionVertical -= mouseY;
                rotacionVertical = Mathf.Clamp(rotacionVertical, limiteVerticalMin, limiteVerticalMax);
                camaraTransform.localRotation = Quaternion.Euler(rotacionVertical, 0, 0);
            }
        }
        else
        {
            // Rotación con teclado (solo cuando NO se usa el mouse)
            x = Input.GetAxis("Horizontal");
            transform.Rotate(0, x * Time.deltaTime * velocidadRotacion, 0);
        }

        // Movimiento
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", y);

        Vector3 direccion = transform.forward * y * velocidadMovimiento;
        movimiento.x = direccion.x;
        movimiento.z = direccion.z;

        // Salto
        if (puedoSaltar)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                movimiento.y = fuerzaSalto;
                anim.SetBool("salte", true);
                anim.SetBool("tocoSuelo", false);
                puedoSaltar = false;
            }
        }
        else
        {
            EstoyCayendo();
        }

        // Aplicar gravedad
        movimiento.y -= gravedad * Time.deltaTime;
        controller.Move(movimiento * Time.deltaTime);

        // Ataque
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("attack");
        }
    }

    public void EstoyCayendo()
    {
        if (!estaCayendo)
        {
            estaCayendo = true;
            anim.SetBool("tocoSuelo", false);
            anim.SetBool("salte", false);
        }
    }

    // Detectar contacto con el suelo mediante el collider del CharacterController
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ground"))
        {
            puedoSaltar = true;
            estaCayendo = false;
            anim.SetBool("tocoSuelo", true);
            anim.SetBool("salte", false);
        }
    }

   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("arma") && !invulnerable)
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
                if (health.CurrentLives > 0)
                    StartCoroutine(InvulnerabilidadTemporal());
            }
        }
    }
    // Método para recibir daño desde otros scripts
    public void TomarDaño()
    {
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health == null)
        {
            Debug.LogError(" No se encontró PlayerHealth en el jugador");
            return;
        }

        health.TakeDamage(1);

        if (health.CurrentLives > 0)
        {
            StartCoroutine(InvulnerabilidadTemporal());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator InvulnerabilidadTemporal()
    {
        invulnerable = true;
        Renderer rend = GetComponentInChildren<Renderer>();
        float tiempo = 0;

        while (tiempo < tiempoInvulnerable)
        {
            if (rend != null)
                rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(0.2f);
            tiempo += 0.2f;
        }

        if (rend != null)
            rend.enabled = true;
        invulnerable = false;
    }
}