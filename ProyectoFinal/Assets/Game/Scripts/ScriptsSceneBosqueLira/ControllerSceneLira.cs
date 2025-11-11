using TMPro;
using UnityEngine;

public class ControllerSceneLira : MonoBehaviour
{
    public int semillasTotales = 3;
    private int semillasRecolectadas = 0;

    [Header("Portal")]
    public GameObject portal;

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

        // 🌿 Mensaje inicial
        MostrarMensaje("Bienvenido al Santuario de Agua. \n\n Haz clic en las gemas en el orden correcto para activar la semilla del santuario.", 6f);
    }

    // 🔹 Llamado por Semilla al ser recogida
    public void RecogerSemilla()
    {
        semillasRecolectadas++;
        textoSemillas.text = $"Semillas recolectadas: {semillasRecolectadas}/{semillasTotales}";

        // Mensaje informativo
        MostrarMensaje($"🌱 Has recolectado una semilla ({semillasRecolectadas}/{semillasTotales})", 3f);

        if (TieneTodasLasSemillas())
        {
            ActivarPortel();
        }
    }

    private void ActivarPortel()
    {
        if (portal != null && !portal.activeSelf)
        {
            portal.SetActive(true);
            MostrarMensaje("🌌 ¡El portal del bosque ha aparecido!", 3f);
            Debug.Log("Portal activado");
        }
    }

    public bool TieneTodasLasSemillas()
    {
        return semillasRecolectadas >= semillasTotales;
    }

    public void RestaurarBosque()
    {
        if (bosqueRestaurado) return;

        bosqueRestaurado = true;
        MostrarMensaje("✨ ¡Has restaurado el Bosque de Lira! 🌿", 3f);

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

    public void MostrarMensaje(string texto, float duracion = 2f)
    {
        if (mensajeUI == null) return;
        StopAllCoroutines();
        StartCoroutine(MostrarMensajeTemporal(texto, duracion));
    }

    private System.Collections.IEnumerator MostrarMensajeTemporal(string texto, float duracion)
    {
        mensajeUI.text = texto;
        yield return new WaitForSeconds(duracion);
        mensajeUI.text = "";
    }
}
