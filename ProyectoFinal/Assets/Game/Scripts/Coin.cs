using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("💰 Valor de la moneda")]
    public int valorMoneda = 1;

    [Header("🔊 Sonido (opcional)")]
    public AudioClip sonidoMoneda;
    private AudioSource audioSource;

    private void Start()
    {
        // Crear un AudioSource si el objeto tiene un sonido
        if (sonidoMoneda != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sonidoMoneda;
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el jugador toca la moneda
        if (other.CompareTag("Player"))
        {
            // Reproduce sonido
            if (audioSource != null && sonidoMoneda != null)
                audioSource.PlayOneShot(sonidoMoneda);

            // Registrar en el GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.AgregarMoneda(valorMoneda);
            }

            // Desactivar el objeto después de recogerlo (espera un momento si tiene sonido)
            Destroy(gameObject, sonidoMoneda != null ? sonidoMoneda.length : 0f);
        }
    }
}
