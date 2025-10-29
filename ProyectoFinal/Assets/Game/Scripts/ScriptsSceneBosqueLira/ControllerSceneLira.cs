using UnityEngine;
using TMPro;

public class ControllerSceneLira : MonoBehaviour
{
    public int semillasTotales = 3;
    private int semillasRecolectadas = 0;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoSemillas;
    public TextMeshProUGUI mensajeUI;

    [Header("Referencias de Escena")]
    public GameObject arbolCentral;
    public ParticleSystem efectoRestauracion;

    private bool bosqueRestaurado = false;

    void Start()
    {
        textoSemillas.text = "Semillas recolectadas: 0/" + semillasTotales;
        mensajeUI.text = "";
    }

    public void RecogerSemilla()
    {
        semillasRecolectadas++;
        textoSemillas.text = "Semillas recolectadas: " + semillasRecolectadas + "/" + semillasTotales;
    }

    public bool TieneTodasLasSemillas()
    {
        return semillasRecolectadas >= semillasTotales;
    }

    public void RestaurarBosque()
    {
        if (bosqueRestaurado) return;

        bosqueRestaurado = true;
        mensajeUI.text = "¡Has restaurado el Bosque de Lira!";

        if (efectoRestauracion != null) efectoRestauracion.Play();

        if (arbolCentral != null)
            arbolCentral.GetComponent<Animator>()?.SetTrigger("Renacer");

        RenderSettings.ambientLight = new Color(0.6f, 0.9f, 0.6f);
        Light sol = FindObjectOfType<Light>();
        if (sol != null)
        {
            sol.color = new Color(1f, 0.95f, 0.8f);
            sol.intensity = 1.2f;
        }

        RenderSettings.fog = false;
    }
}
