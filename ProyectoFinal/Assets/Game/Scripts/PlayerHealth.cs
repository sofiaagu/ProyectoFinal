using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vidas")]
    public int maxLives = 5;
    private int currentLives;
    public int CurrentLives => currentLives;

    [Header("UI")]
    public PlayerHealthUI playerHealthUI;

    [Header("Sonidos")]
    public AudioClip sonidoDaño;
    public AudioClip sonidoMuerte;
    private AudioSource audioSource;


    void Start()
    {
        // Crear AudioSource si no existe
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Cargar vidas guardadas o iniciales
        if (GameManager.instance != null && GameManager.instance.vidasPersistentes >= 0)
            currentLives = GameManager.instance.vidasPersistentes;
        else
            currentLives = maxLives;

        // Autoasignar UI si no se ha puesto
        if (playerHealthUI == null)
            playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

        if (playerHealthUI != null)
            playerHealthUI.UpdateHearts();
    }


    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        if (currentLives < 0)
            currentLives = 0;

        // 🔊 Sonido de daño
        if (sonidoDaño != null)
            audioSource.PlayOneShot(sonidoDaño);

        // Guardar vidas en GameManager
        if (GameManager.instance != null)
            GameManager.instance.vidasPersistentes = currentLives;

        // Actualizar UI
        if (playerHealthUI == null)
            playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

        if (playerHealthUI != null)
            playerHealthUI.UpdateHearts();

        if (currentLives <= 0)
            Die();
    }


    void Die()
    {
        // 🔊 Sonido de muerte
        if (sonidoMuerte != null)
            audioSource.PlayOneShot(sonidoMuerte);

        // Llamar Game Over
        if (GameManager.instance != null)
            GameManager.instance.MostrarGameOver();

        // Desactivar jugador tras un pequeño delay (para que suene bien el audio)
        StartCoroutine(DesactivarJugador());
    }


    private System.Collections.IEnumerator DesactivarJugador()
    {
        yield return new WaitForSeconds(0.2f); // Espera mínima para que suene
        gameObject.SetActive(false);
    }
}
