using UnityEngine;

public class Coin : MonoBehaviour
{
    public int valorMoneda = 1;
    public AudioClip sonidoMoneda;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Busca el controller de la escena actual
            if (GameManager.instance != null && GameManager.instance.controllerActual != null)
            {
                GameManager.instance.controllerActual.RegistrarMoneda(valorMoneda);
            }

            // Reproduce sonido (opcional)
            if (sonidoMoneda != null)
                AudioSource.PlayClipAtPoint(sonidoMoneda, transform.position);

            // Destruye la moneda
            Destroy(gameObject);
        }
    }
}
