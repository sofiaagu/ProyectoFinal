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
                // Si existe ControllerOlvido
                if (GameManager.instance.controllerActual != null)
                    GameManager.instance.controllerActual.RegistrarMoneda(valorMoneda);

                // Si existe ControllerSceneLira
                if (GameManager.instance.controllerActual2 != null)
                    GameManager.instance.controllerActual2.RegistrarMoneda(valorMoneda);

                // Si existe ControllerAerthemoor
                if (GameManager.instance.controllerActual3 != null)
                    GameManager.instance.controllerActual3.RegistrarMoneda(valorMoneda);

                // Si existe ControllerSactum
                if (GameManager.instance.controllerActual4 != null)
                    GameManager.instance.controllerActual4.RegistrarMoneda(valorMoneda);
            }

            if (sonidoMoneda != null)
                AudioSource.PlayClipAtPoint(sonidoMoneda, transform.position);

            Destroy(gameObject);
        }
    }
}
