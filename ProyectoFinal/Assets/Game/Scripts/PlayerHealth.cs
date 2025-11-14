using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 5;
    private int currentLives;

    public int CurrentLives => currentLives;

    public PlayerHealthUI playerHealthUI; // Asignado o buscado automáticamente

    void Start()
    {
        currentLives = maxLives;

        // 🔥 AUTOASIGNACIÓN DEL UI (si no está asignado a mano)
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

        if (playerHealthUI == null)
            playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

        if (playerHealthUI != null)
            playerHealthUI.UpdateHearts();

        if (currentLives <= 0)
            Die();
    }

    void Die()
    {
        if (GameManager.instance != null)
            GameManager.instance.MostrarGameOver();

        gameObject.SetActive(false);
    }
}
