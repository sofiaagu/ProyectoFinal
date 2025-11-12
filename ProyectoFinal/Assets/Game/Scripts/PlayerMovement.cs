using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    public float fuerzaSalto = 8.0f;
    public float gravedad = 20.0f;

    private CharacterController controller;
    private Animator anim;

    private Vector3 movimiento;
    private float x, y;
    private bool puedoSaltar = false;
    private bool estaCayendo = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", y);

        transform.Rotate(0, x * Time.deltaTime * velocidadRotacion, 0);

        Vector3 direccion = transform.forward * y * velocidadMovimiento;
        movimiento.x = direccion.x;
        movimiento.z = direccion.z;

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
    }

    public void EstoyCayendo()
    {
        // Cambia animaciones al caer
        if (!estaCayendo)
        {
            estaCayendo = true;
            anim.SetBool("tocoSuelo", false);
            anim.SetBool("salte", false);
        }
    }

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
}
