using System.Collections;
using UnityEngine;

// SantuarioTierra: controla el minijuego de fuegos sobre tumbas.
// Compatible con tu script genérico "Semilla" (la semilla debe tener tipo "Tierra").
public class SantuarioTierra : MonoBehaviour
{
    [Header("Configuración del juego")]
    public GameObject[] tumbas;
    public ParticleSystem[] fuegos;
    public GameObject semillaTierra;
    public GameObject portalTierra;
    public int fuegosNecesarios = 5;
    public float duracionFuego = 2f;
    public float esperaEntreFuegos = 1f;

    [Header("Prefab al aparecer la semilla")]
    public GameObject prefabAparecer;        // Prefab que aparecerá (simplemente se activa)

    [Header("Control de Planos")]
    public GameObject planoDesactivar1;
    public GameObject planoDesactivar2;

    public GameObject planoActivar1;
    public GameObject planoActivar2;

    private int aciertos = 0;
    private bool jugando = false;
    private bool completado = false;
    private ControllerSceneLira controller;
    private int indiceActivo = -1;

    void Start()
    {
        controller = FindObjectOfType<ControllerSceneLira>();

        if (tumbas == null || fuegos == null || tumbas.Length != fuegos.Length)
        {
            Debug.LogError("SantuarioTierra: Asegúrate de asignar 'tumbas' y 'fuegos' y que tengan la misma longitud.");
            return;
        }

        for (int i = 0; i < fuegos.Length; i++)
        {
            if (fuegos[i] != null)
            {
                fuegos[i].Stop();
                fuegos[i].gameObject.SetActive(false);
            }
        }

        if (semillaTierra != null)
            semillaTierra.SetActive(false);

        if (portalTierra != null)
            portalTierra.SetActive(false);

        // 🔥 El prefab debe iniciar apagado
        if (prefabAparecer != null)
            prefabAparecer.SetActive(false);

        StartCoroutine(IniciarJuegoDespuesDe(6f));
    }

    private IEnumerator IniciarJuegoDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (tumbas == null || fuegos == null || tumbas.Length != fuegos.Length) yield break;
        jugando = true;
        StartCoroutine(CicloFuegos());
    }

    private IEnumerator CicloFuegos()
    {
        while (jugando && !completado)
        {
            int nuevoIndice;
            do
            {
                nuevoIndice = Random.Range(0, tumbas.Length);
            } while (tumbas.Length > 1 && nuevoIndice == indiceActivo);

            indiceActivo = nuevoIndice;

            if (fuegos[indiceActivo] != null)
            {
                fuegos[indiceActivo].gameObject.SetActive(true);
                fuegos[indiceActivo].Play();
            }

            float tiempo = 0f;
            while (tiempo < duracionFuego)
            {
                if (!jugando) yield break;
                tiempo += Time.deltaTime;
                yield return null;
            }

            if (fuegos[indiceActivo] != null)
            {
                fuegos[indiceActivo].Stop();
            }

            yield return new WaitForSeconds(esperaEntreFuegos);
        }
    }

    public void ClickEnTumba(int id)
    {
        if (!jugando || completado) return;

        if (id < 0 || id >= fuegos.Length)
        {
            Debug.LogWarning($"ClickEnTumba: id fuera de rango: {id}");
            return;
        }

        bool fuegoActivo = fuegos[id] != null && fuegos[id].isPlaying;

        if (fuegoActivo && id == indiceActivo)
        {
            aciertos++;
            controller?.MostrarMensaje($"¡{aciertos} de {fuegosNecesarios} fuegos necesarios!", 2f);

            fuegos[id].Stop();

            if (aciertos >= fuegosNecesarios)
            {
                Completado();
            }
        }
        else
        {
            aciertos = Mathf.Max(0, aciertos - 1);
            controller?.MostrarMensaje($"Tumba incorrecta\n\n¡{aciertos} de {fuegosNecesarios} fuegos necesarios!", 2f);
        }
    }

    private void Completado()
    {
        completado = true;
        jugando = false;

        controller?.MostrarMensaje("¡Has despertado los cinco fuegos sagrados!\n\nRecolecta la semilla de tierra.", 4f);

        for (int i = 0; i < fuegos.Length; i++)
        {
            if (fuegos[i] != null)
            {
                fuegos[i].Stop();
            }
        }

        // 🌱 Mostrar la semilla
        if (semillaTierra != null)
        {
            semillaTierra.SetActive(true);

            var sem = semillaTierra.GetComponent<Semilla>();
            if (sem != null)
                sem.tipoSemilla = "Tierra";
        }
      
        // 🔥 CAMBIO DE PLANOS 🔥
        if (planoDesactivar1 != null) planoDesactivar1.SetActive(false);
        if (planoDesactivar2 != null) planoDesactivar2.SetActive(false);

        if (planoActivar1 != null) planoActivar1.SetActive(true);
        if (planoActivar2 != null) planoActivar2.SetActive(true);

        Debug.Log("Cambios de planos realizados.");
    }

    public void ActivarPortal()
    {
        if (portalTierra != null)
        {
            portalTierra.SetActive(true);
            Debug.Log("Portal de Tierra activado");
        }
    }

    public void ActivarPrefab()
    {
        if (prefabAparecer != null)
        {
            prefabAparecer.SetActive(true);
            Debug.Log("Prefab activado al hacer clic en la semilla.");
        }
    }
}
