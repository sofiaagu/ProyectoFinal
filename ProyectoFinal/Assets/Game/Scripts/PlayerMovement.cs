using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Cámara")]
    [Tooltip("Pivot o punto donde está la cámara (hijo del jugador).")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalClamp = 80f;

    private CharacterController controller;
    private Animator anim;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private Vector3 currentHorizontalVelocity;

    private bool isGroundedByTag = false;
    private bool rightClickHeld = false;
    private float xRotation = 0f;

    // Parámetros del Animator
    private static readonly int VelX = Animator.StringToHash("velX");
    private static readonly int VelY = Animator.StringToHash("velY");
    private static readonly int JumpTrigger = Animator.StringToHash("jump");

    [SerializeField] private float animDamp = 0.05f;
    private float velXCur, velYCur;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Cursor libre al iniciar
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // === NUEVO INPUT SYSTEM ===
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnRightClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            rightClickHeld = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (ctx.canceled)
        {
            rightClickHeld = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        HandleMovement();

        // Solo mirar si el clic derecho está presionado
        if (rightClickHeld)
            HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        // Movimiento relativo a la cámara
        Vector3 moveWorld;
        if (cameraPivot != null)
        {
            Vector3 camFwd = cameraPivot.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = cameraPivot.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveWorld = camRight * input.x + camFwd * input.z;
        }
        else
        {
            moveWorld = transform.right * input.x + transform.forward * input.z;
        }

        // Movimiento directo
        Vector3 targetVelocity = moveWorld * moveSpeed;
        currentHorizontalVelocity = targetVelocity;
        controller.Move(currentHorizontalVelocity * Time.deltaTime);

        // Rotación suave hacia la dirección de movimiento
        Vector3 lookDir = new Vector3(currentHorizontalVelocity.x, 0f, currentHorizontalVelocity.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Salto
        if (isGroundedByTag)
        {
            if (velocity.y < 0f)
                velocity.y = -2f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (anim != null)
                    anim.SetTrigger(JumpTrigger);
            }
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animaciones
        velXCur = Mathf.Lerp(velXCur, moveInput.x, animDamp);
        velYCur = Mathf.Lerp(velYCur, moveInput.y, animDamp);
        if (anim != null)
        {
            anim.SetFloat(VelX, velXCur);
            anim.SetFloat(VelY, velYCur);
        }
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Rotación vertical de la cámara (eje X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal del jugador (eje Y)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ground"))
            isGroundedByTag = true;
        else
            isGroundedByTag = false;
    }
}
