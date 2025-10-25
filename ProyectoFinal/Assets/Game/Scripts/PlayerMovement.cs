using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float doubleJumpHeight = 1.5f;
    [SerializeField] private bool canDoubleJump = true;
    [Tooltip("Tiempo en el aire antes de poder saltar de nuevo (coyote time)")]
    [SerializeField] private float coyoteTime = 0.15f;
    [Tooltip("Tiempo para detectar input anticipado de salto (jump buffering)")]
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer = -1;
    [Tooltip("Offset desde el centro del CharacterController para el raycast")]
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0);

    [Header("Physics")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Optional")]
    [Tooltip("Si se asigna, el movimiento será relativo a esta cámara")]
    public Camera mouseOrbitCamera;

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip doubleJumpSound;
    [SerializeField] private AudioClip landSound;

    // Componentes
    private CharacterController controller;
    private Animator anim;
    private AudioSource audioSource;

    // Input
    private Vector2 moveInput;
    private bool jumpPressed;

    // Física
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;

    // Sistema de salto
    private int jumpsRemaining;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Animación
    private static readonly int VelX = Animator.StringToHash("velX");
    private static readonly int VelY = Animator.StringToHash("velY");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int VerticalVelocity = Animator.StringToHash("verticalVelocity");

    [SerializeField] private float animDamp = 0.05f;
    private float velXCur, velYCur;
    private float velXVelocity, velYVelocity; // Variables para SmoothDamp

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Si no hay AudioSource, agregarlo
        if (audioSource == null && (jumpSound != null || doubleJumpSound != null || landSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        jumpsRemaining = canDoubleJump ? 2 : 1;
    }

    // ===== INPUT CALLBACKS =====

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            jumpPressed = true;
            jumpBufferCounter = jumpBufferTime; // Activar buffer
        }
    }

    private void Update()
    {
        // 1. Detección de suelo mejorada
        CheckGroundStatus();

        // 2. Manejar Coyote Time y Jump Buffer
        HandleJumpTimers();

        // 3. Procesar salto
        HandleJump();

        // 4. Calcular y aplicar movimiento horizontal
        HandleMovement();

        // 5. Aplicar gravedad y movimiento vertical
        HandleGravity();

        // 6. Actualizar animaciones
        UpdateAnimations();

        // 7. Resetear input de salto
        jumpPressed = false;
    }

    /// <summary>
    /// Detección mejorada del suelo usando Raycast
    /// </summary>
    private void CheckGroundStatus()
    {
        wasGrounded = isGrounded;

        // Raycast desde el centro del personaje hacia abajo
        Vector3 rayOrigin = transform.position + groundCheckOffset;

        // Usar tanto el CharacterController como un Raycast para mayor precisión
        bool controllerGrounded = controller.isGrounded;
        bool raycastGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayer);

        isGrounded = controllerGrounded || raycastGrounded;

        // Debug visual (opcional - comentar en producción)
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);

        // Detectar aterrizaje
        if (isGrounded && !wasGrounded)
        {
            OnLanded();
        }

        // Resetear saltos al tocar el suelo
        if (isGrounded)
        {
            jumpsRemaining = canDoubleJump ? 2 : 1;
        }
    }

    /// <summary>
    /// Maneja los timers de Coyote Time y Jump Buffer
    /// </summary>
    private void HandleJumpTimers()
    {
        // Coyote Time: permite saltar brevemente después de dejar el suelo
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer: recuerda input de salto por un breve tiempo
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Procesa la lógica de salto y doble salto
    /// </summary>
    private void HandleJump()
    {
        // Condiciones para saltar:
        // 1. Input de salto (directo o en buffer)
        // 2. Está en el suelo O tiene coyote time O le quedan saltos en el aire
        bool canJump = (jumpPressed || jumpBufferCounter > 0) &&
                       (coyoteTimeCounter > 0 || jumpsRemaining > 0);

        if (canJump)
        {
            PerformJump();
            jumpBufferCounter = 0; // Consumir el buffer
        }
    }

    /// <summary>
    /// Ejecuta el salto
    /// </summary>
    private void PerformJump()
    {
        // Determinar altura del salto
        float jumpForce;
        bool isDoubleJump = false;

        if (jumpsRemaining == 2 || coyoteTimeCounter > 0)
        {
            // Primer salto
            jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlaySound(jumpSound);
        }
        else
        {
            // Doble salto
            jumpForce = Mathf.Sqrt(doubleJumpHeight * -2f * gravity);
            PlaySound(doubleJumpSound);
            isDoubleJump = true;
        }

        // Aplicar velocidad vertical
        velocity.y = jumpForce;

        // Reducir saltos disponibles
        jumpsRemaining--;
        coyoteTimeCounter = 0; // Consumir coyote time

        // Trigger de animación
        if (anim != null)
        {
            anim.SetTrigger(Jump);
        }

        Debug.Log(isDoubleJump ? "Doble Salto!" : "Salto!");
    }

    /// <summary>
    /// Maneja el movimiento horizontal
    /// </summary>
    private void HandleMovement()
    {
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 moveWorld;
        if (mouseOrbitCamera != null && mouseOrbitCamera.gameObject.activeInHierarchy)
        {
            Vector3 camFwd = mouseOrbitCamera.transform.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = mouseOrbitCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveWorld = camRight * input.x + camFwd * input.z;
        }
        else
        {
            moveWorld = transform.right * input.x + transform.forward * input.z;
        }

        // Rotar hacia la dirección de movimiento
        Vector3 lookDir = new Vector3(moveWorld.x, 0f, moveWorld.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Aplicar movimiento
        Vector3 horizontal = moveWorld * moveSpeed;
        controller.Move(horizontal * Time.deltaTime);
    }

    /// <summary>
    /// Aplica gravedad con modificador de caída
    /// </summary>
    private void HandleGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // Pequeño empuje para mantener grounded
        }
        else
        {
            // Aplicar gravedad
            float gravityMultiplier = velocity.y < 0 ? fallMultiplier : 1f;
            velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Actualiza parámetros del Animator
    /// </summary>
    private void UpdateAnimations()
    {
        if (anim == null) return;

        // Blend Tree (velX, velY) - Corregido: usar Lerp en lugar de SmoothDamp
        velXCur = Mathf.Lerp(velXCur, moveInput.x, animDamp);
        velYCur = Mathf.Lerp(velYCur, moveInput.y, animDamp);
        anim.SetFloat(VelX, velXCur);
        anim.SetFloat(VelY, velYCur);

        // Estado de suelo
        anim.SetBool(IsGrounded, isGrounded);

        // Velocidad vertical (para animación de caída)
        anim.SetFloat(VerticalVelocity, velocity.y);
    }

    /// <summary>
    /// Callback cuando el personaje aterriza
    /// </summary>
    private void OnLanded()
    {
        PlaySound(landSound);
        Debug.Log("Aterrizó!");

        // Aquí puedes agregar efectos de partículas, cámara shake, etc.
    }

    /// <summary>
    /// Reproduce un sonido si está asignado
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ===== MÉTODOS PÚBLICOS PARA EXTENSIÓN =====

    /// <summary>
    /// Fuerza un salto desde código (útil para jump pads, etc.)
    /// </summary>
    public void ForceJump(float customHeight)
    {
        float jumpForce = Mathf.Sqrt(customHeight * -2f * gravity);
        velocity.y = jumpForce;

        if (anim != null)
        {
            anim.SetTrigger(Jump);
        }
    }

    /// <summary>
    /// Desactiva temporalmente el movimiento
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    /// <summary>
    /// Obtiene si el jugador está en el suelo
    /// </summary>
    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Obtiene la velocidad vertical actual
    /// </summary>
    public float GetVerticalVelocity()
    {
        return velocity.y;
    }

    // ===== GIZMOS PARA DEBUG =====
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Visualizar el punto de detección de suelo
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(rayOrigin, 0.1f);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}