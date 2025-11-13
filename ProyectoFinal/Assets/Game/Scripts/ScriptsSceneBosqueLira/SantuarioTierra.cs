using System.Collections;
using UnityEngine;

// SantuarioTierra: controla el minijuego de fuegos sobre tumbas.
// Compatible con tu script genérico "Semilla" (la semilla debe tener tipo "Tierra").
public class SantuarioTierra : MonoBehaviour
{
    [Header("Configuración del juego")]
    public GameObject[] tumbas;              // Las 5 tumbas (IDs deben ser 0..n-1)
    public ParticleSystem[] fuegos;          // Partículas de fuego asociadas (mismo tamaño que tumbas)
    public GameObject semillaTierra;         // Semilla (inactiva hasta ganar)
    public GameObject portalTierra;          // Portal que se activa cuando se recoge la semilla
    public int fuegosNecesarios = 5;         // Número de aciertos necesarios (aquí 5)
    public float duracionFuego = 2f;         // Cuánto dura cada fuego encendido
    public float esperaEntreFuegos = 1f;     // Espera entre fuegos

    private int aciertos = 0;
    private bool jugando = false;
    private bool completado = false;
    private ControllerSceneLira controller;
    private int indiceActivo = -1;           // índice de la tumba que actualmente tiene fuego

    void Start()
    {
        controller = FindObjectOfType<ControllerSceneLira>();

        // Seguridad: arrays deben coincidir
        if (tumbas == null || fuegos == null || tumbas.Length != fuegos.Length)
        {
            Debug.LogError("SantuarioTierra: Asegúrate de asignar 'tumbas' y 'fuegos' y que tengan la misma longitud.");
            return;
        }

        // Apagar todos los fuegos al iniciar
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

        // Mensaje inicial (aparece al entrar y luego empieza el juego automáticamente)
        controller?.MostrarMensaje(
            "Bienvenido al Santuario de la Tierra.\n\nHaz clic en las tumbas encendidas para despertar los fuegos sagrados y liberar la semilla del santuario.",
            6f
        );

        // Iniciar el juego automáticamente después del mensaje
        StartCoroutine(IniciarJuegoDespuesDe(6f));
    }

    private IEnumerator IniciarJuegoDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        // seguridad extra: si hubo error en Start (arrays), no iniciar
        if (tumbas == null || fuegos == null || tumbas.Length != fuegos.Length) yield break;
        jugando = true;
        StartCoroutine(CicloFuegos());
    }

    // Ciclo que enciende fuegos aleatoriamente mientras el juego esté activo
    private IEnumerator CicloFuegos()
    {
        while (jugando && !completado)
        {
            // Elegir tumba aleatoria (diferente a la anterior si hay más de 1)
            int nuevoIndice;
            do
            {
                nuevoIndice = Random.Range(0, tumbas.Length);
            } while (tumbas.Length > 1 && nuevoIndice == indiceActivo);

            indiceActivo = nuevoIndice;

            // Encender fuego (asegura que el objeto esté activo para poder reproducir)
            if (fuegos[indiceActivo] != null)
            {
                fuegos[indiceActivo].gameObject.SetActive(true);
                fuegos[indiceActivo].Play();
            }

            // Espera durante la duración del fuego (duracionFuego), permitiendo clicks
            float tiempo = 0f;
            while (tiempo < duracionFuego)
            {
                if (!jugando) yield break;
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Si el fuego sigue activo (no fue apagado por click), se apaga automáticamente
            if (fuegos[indiceActivo] != null)
            {
                fuegos[indiceActivo].Stop();
                // opcional: dejar inactivo el GameObject de partículas para limpieza visual
                // fuegos[indiceActivo].gameObject.SetActive(false);
            }

            // Espera entre fuegos
            yield return new WaitForSeconds(esperaEntreFuegos);
        }
    }

    // Método público llamado por cada tumba cuando el jugador hace click (Tumba.cs)
    public void ClickEnTumba(int id)
    {
        if (!jugando || completado) return;

        // Validar id dentro de rango
        if (id < 0 || id >= fuegos.Length)
        {
            Debug.LogWarning($"ClickEnTumba: id fuera de rango: {id}");
            return;
        }

        bool fuegoActivo = fuegos[id] != null && fuegos[id].isPlaying;

        if (fuegoActivo && id == indiceActivo)
        {
            // Acierto
            aciertos++;
            controller?.MostrarMensaje($"¡Fuego despertado! ({aciertos}/{fuegosNecesarios})", 2f);

            // Apagar fuego inmediatamente
            fuegos[id].Stop();
            // opcional dejar activo el objeto si lo prefieres visible:
            // fuegos[id].gameObject.SetActive(false);

            // Comprobar victoria
            if (aciertos >= fuegosNecesarios)
            {
                Completado();
            }
        }
        else
        {
            // Clic en tumba sin fuego -> penalización
            aciertos = Mathf.Max(0, aciertos - 1);
            controller?.MostrarMensaje($"Tumba incorrecta ({aciertos}/{fuegosNecesarios})", 2f);
            // No hacemos nada más; el ciclo continuará encendiendo nuevos fuegos
        }
    }

    private void Completado()
    {
        completado = true;
        jugando = false;

        controller?.MostrarMensaje("¡Has despertado los cinco fuegos sagrados!\n\nRecolecta la semilla de tierra.", 4f);

        // Apagar todos los fuegos por seguridad
        for (int i = 0; i < fuegos.Length; i++)
        {
            if (fuegos[i] != null)
            {
                fuegos[i].Stop();
                // fuegos[i].gameObject.SetActive(false);
            }
        }

        // Mostrar la semilla y asignarle tipo = "Tierra" si tiene el componente Semilla
        if (semillaTierra != null)
        {
            semillaTierra.SetActive(true);
            var sem = semillaTierra.GetComponent<Semilla>();
            if (sem != null)
            {
                sem.tipoSemilla = "Tierra";
            }
        }
    }

    // Este método puede llamarlo la semilla (tu script genérico) al ser recogida,
    // o lo llamas desde otra parte para activar el portal.
    public void ActivarPortal()
    {
        if (portalTierra != null)
        {
            portalTierra.SetActive(true);
            controller?.MostrarMensaje("🌀 El portal de la Tierra ha sido activado...", 3f);
            Debug.Log("Portal de Tierra activado");
        }
    }

    // (opcional) para depuración desde el editor:
    private void OnValidate()
    {
        // Mantener consistencia: si sólo asignas fuegos, crea tumbas con el mismo tamaño, y viceversa.
        if (tumbas != null && fuegos != null && tumbas.Length != fuegos.Length)
        {
            // No hacemos modificaciones automáticas, solo aviso
            // Debug.LogWarning("SantuarioTierra: 'tumbas' y 'fuegos' deberían tener la misma longitud.");
        }
    }
}
