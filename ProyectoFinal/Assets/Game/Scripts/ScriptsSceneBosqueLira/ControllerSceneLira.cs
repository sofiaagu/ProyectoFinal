using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerSceneLira : MonoBehaviour
{
    public int semillasTotales = 3;
    private int semillasRecolectadas = 0;
    private int monedasEscena = 0;

    public Timer timer;

    [Header("Portal")]
    public GameObject portal;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoSemillas;
    public TextMeshProUGUI mensajeUI;
    public TextMeshProUGUI textoMonedas;

    [Header("Referencias de Escena")]
    public GameObject arbolCentral;
    public ParticleSystem efectoRestauracion;

    private bool bosqueRestaurado = false;

    // corutina activa del mensaje
    private Coroutine mensajeCoroutine;

    void Start()
    {
        textoSemillas.text = "Semillas recolectadas: 0/" + semillasTotales;
        mensajeUI.text = "";

        MostrarMensaje("Bienvenido al Bosque de Lira\n\nDeberás completar un minijuego en cada Santuario para recolectar las tres semillas y así restaurar por completo el bosque.", 6f);

        if (timer == null)
            timer = FindFirstObjectByType<Timer>();

        ActualizarUI();
    }

    // Llamado por Semilla al ser recogida
    public void RecogerSemilla()
    {
        semillasRecolectadas++;
        textoSemillas.text = $"Semillas recolectadas: {semillasRecolectadas}/{semillasTotales}";

        MostrarMensaje($"Has recolectado una semilla ({semillasRecolectadas}/{semillasTotales})", 3f);

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
            MostrarMensaje("¡El portal del bosque ha aparecido!", 3f);
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
        MostrarMensaje("¡Has restaurado el Bosque de Lira! \n\n Busca el Portal para pasar al siguiente nivel", 3f);

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

        if (mensajeCoroutine != null)
            StopCoroutine(mensajeCoroutine);

        mensajeCoroutine = StartCoroutine(MostrarMensajeTemporal(texto, duracion));
    }

    private System.Collections.IEnumerator MostrarMensajeTemporal(string texto, float duracion)
    {
        mensajeUI.text = texto;
        yield return new WaitForSeconds(duracion);
        mensajeUI.text = "";
    }
    private void ActualizarUI()
    {

        if (textoMonedas != null)
            textoMonedas.text = "Monedas: " + monedasEscena;
    }
    public void CompletarEscena()
    {
        Debug.Log("Escena completada. Registrando tiempo…");

        if (timer != null)
        {
            timer.TimerStop();

            if (GameManager.instance != null)
                GameManager.instance.RegistrarTiempo(timer.StopTime);
        }

    }
    public void RegistrarMoneda(int cantidad)
    {
        monedasEscena += cantidad;
        ActualizarUI();

        if (GameManager.instance != null)
            GameManager.instance.AgregarMoneda(cantidad);

        Debug.Log($"Moneda recogida | Escena: {monedasEscena}");
    }
}
