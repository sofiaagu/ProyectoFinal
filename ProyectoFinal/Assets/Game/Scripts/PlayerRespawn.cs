using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // 🔹 PÚBLICO para que los cubos lo puedan cambiar

    [Header("Vida")]
    public int vidasIniciales = 3;
    private int vidasActuales;

    [Header("Death Zone")]
    public float alturaMinima = -10f; // 🔹 Altura por debajo de la cual el jugador muere

    [Header("UI")]
    public GameObject panelPerdiste;
    public PlayerHealthUI healthUI;
    private PlayerHealth playerHealth;

    [Header("Efectos opcionales")]
    public AudioClip sonidoMuerte;
    public AudioClip sonidoVidaPerdida;

    private CharacterController controller;
    private AudioSource audioSource;
    private bool estaMuriendo = false; // 🔹 Evita muerte múltiple


    public int CurrentLives => vidasActuales;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();


        Time.timeScale = 1f;
        vidasActuales = vidasIniciales;

        if (respawnPoint == null)
        {
            GameObject spawn = new GameObject("SpawnPoint");
            spawn.transform.position = transform.position;
            respawnPoint = spawn.transform;
        }
    }

    private void Start()
    {
        if (panelPerdiste != null)
            panelPerdiste.SetActive(false);

        if (healthUI != null)
            healthUI.UpdateHearts();
    }

    private void Update()
    {
        // 🔹 DETECTAR CAÍDA (Death Zone por altura)
        if (!estaMuriendo && transform.position.y < alturaMinima)
        {
            PerderVida();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 🔹 DETECTAR COLISIÓN con objetos etiquetados como "DeathZone"
        if (!estaMuriendo && hit.gameObject.CompareTag("DeathZone"))
        {
            PerderVida();
        }
    }

    private void PerderVida()
    {
        if (estaMuriendo) return;

        estaMuriendo = true;

        // 💥 Restar vida desde PlayerHealth
        if (playerHealth != null)
            playerHealth.TakeDamage(1);

        Debug.Log("Vida perdida. Vidas restantes: " + playerHealth.CurrentLives);

        if (sonidoVidaPerdida != null && audioSource != null)
            audioSource.PlayOneShot(sonidoVidaPerdida);

        if (playerHealth.CurrentLives <= 0)
        {
            GameOver();
        }
        else
        {
            Respawn();
        }
    }


    private void Respawn()
    {
        controller.enabled = false;

        // 🔹 Resetear velocidad si hay Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = respawnPoint.position;
        controller.enabled = true;

        estaMuriendo = false; // 🔹 Permitir detectar muerte nuevamente
        Debug.Log("Respawn en: " + respawnPoint.position);
    }

    private void GameOver()
    {
        Debug.Log("¡Game Over!");

        // 🔊 Reproducir sonido de muerte
        if (sonidoMuerte != null && audioSource != null)
            audioSource.PlayOneShot(sonidoMuerte);

        if (panelPerdiste != null)
            panelPerdiste.SetActive(true);

        // 🔹 Pausa del juego
        Time.timeScale = 0f;

        // 🔹 Desactivar controles del jugador (opcional)
        controller.enabled = false;
    }

    // 🔹 MÉTODO PÚBLICO para reiniciar el juego
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        vidasActuales = vidasIniciales;

        if (panelPerdiste != null)
            panelPerdiste.SetActive(false);

        if (healthUI != null)
            healthUI.UpdateHearts();

        controller.enabled = true;
        estaMuriendo = false;
        Respawn();
    }
}