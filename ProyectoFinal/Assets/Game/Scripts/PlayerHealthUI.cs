using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject[] hearts;

    public void UpdateHearts()
    {
        if (playerHealth == null) return;

        for (int i = 0; i < hearts.Length; i++)
            hearts[i].SetActive(i < playerHealth.CurrentLives);
    }
}
