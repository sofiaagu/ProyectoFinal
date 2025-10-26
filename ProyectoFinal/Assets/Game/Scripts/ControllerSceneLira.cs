using UnityEngine;
using UnityEngine.UI;

public class ControllerSceneLira : MonoBehaviour
{
    public int semillasTotales = 3;
    private int semillasRecolectadas = 0;

    public Text mensajeUI;
    public GameObject arbolCentral;
    public ParticleSystem efectoRestauracion;

    public void RecogerSemilla()
    {
        semillasRecolectadas++;
        mensajeUI.text = "Semillas recolectadas: " + semillasRecolectadas + "/" + semillasTotales;

        if (semillasRecolectadas >= semillasTotales)
        {
            RestaurarBosque();
        }
    }

    void RestaurarBosque()
    {
        mensajeUI.text = "¡Has restaurado el Bosque de Lira!";
        if (efectoRestauracion != null) efectoRestauracion.Play();
        if (arbolCentral != null)
            arbolCentral.GetComponent<Animator>().SetTrigger("Renacer");
    }
}
