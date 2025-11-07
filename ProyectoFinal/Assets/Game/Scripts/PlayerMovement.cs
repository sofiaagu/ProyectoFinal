using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Cámara")]
    [SerializeField] private Transform cameraPivot; // Punto donde está la cámara (hijo del player)
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalClamp = 80f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool jumpPressed;
    private bool isGrounded;
    private bool rightClickHeld = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Cursor libre al inicio (puedes cambiarlo si quieres que empiece bloqueado)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- INPUT SYSTEM ---
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            jumpPressed = true;
    }

    // 🔹 Nuevo: se llama desde la acción "RightClick"
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

        // Solo mirar mientras el clic derecho esté presionado
        if (rightClickHeld)
            HandleLook();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        // Movimiento horizontal relativo a la cámara
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Salto
        if (isGrounded && jumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        jumpPressed = false;

        // Gravedad
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0)
            velocity.y = -2f;

        controller.Move(velocity * Time.deltaTime);
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
}