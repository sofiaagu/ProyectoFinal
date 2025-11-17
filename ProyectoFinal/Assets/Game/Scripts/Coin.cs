using UnityEngine;

public class Coin : MonoBehaviour
{
    public int valorMoneda = 2;
    public AudioClip sonidoMoneda;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                // Si existe Controller2
                if (GameManager.instance.controllerActual != null)
                    GameManager.instance.controllerActual.RegistrarMoneda(valorMoneda);

                // Si existe ControllerSceneLira
                if (GameManager.instance.controllerActual2 != null)
                    GameManager.instance.controllerActual2.RegistrarMoneda(valorMoneda);

                if (GameManager.instance.controllerActual3 != null)
                    GameManager.instance.controllerActual3.RegistrarMoneda(valorMoneda);
            }

            if (sonidoMoneda != null)
                AudioSource.PlayClipAtPoint(sonidoMoneda, transform.position);

            Destroy(gameObject);
        }
    }
}
