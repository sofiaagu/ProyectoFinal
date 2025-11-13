using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CrystalSequenceManager : MonoBehaviour
{
    [Header("Configuración de Cristales")]
    [Tooltip("Cristales de pista (los que muestran la secuencia)")]
    public CristalNode[] cristalesPista;

    [Tooltip("Cristales interactuables (los que el jugador debe activar)")]
    public CristalNode[] cristalesInteractuables;

    [Header("Configuración de Secuencia")]
    [Tooltip("Tiempo que cada cristal permanece encendido en la pista")]
    public float tiempoPorCristal = 1.5f;

    [Tooltip("Tecla para solicitar siguiente pista")]
    public KeyCode teclaPista = KeyCode.E;

    [Tooltip("Distancia mínima para poder ver las pistas")]
    public float rangoProximidad = 5f;

    [Header("Puentes")]
    [Tooltip("Puentes que se activan progresivamente (uno por cada cristal correcto)")]
    public GameObject[] puentes;

    [Header("UI")]
    public GameObject textoPresionaE;
    public TMPro.TextMeshProUGUI textoSecuencia;

    [Header("Audio")]
    public AudioClip sonidoPista;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoError;
    public AudioClip sonidoCompletado;

    [Header("Sistema de Feedback")]
    [Tooltip("Sistema mejorado de feedback visual y auditivo")]
    public CrystalFeedbackSystem feedbackSystem;

    [Header("Debug")]
    [Tooltip("Muestra la secuencia generada en consola al iniciar")]
    public bool mostrarSecuenciaEnConsola = true;

    private AudioSource audioSource;
    private int pasoActual = 0;
    private List<int> secuenciaCorrecta = new List<int>();
    private List<int> secuenciaJugador = new List<int>();
    private bool puzzleCompletado = false;
    private bool mostrandoPista = false;
    private bool jugadorEnRango = false;
    private Transform jugador;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Desactivar todos los puentes al inicio
        if (puentes != null)
        {
            foreach (GameObject puente in puentes)
            {
                if (puente != null)
                    puente.SetActive(false);
            }
        }

        if (textoPresionaE != null)
            textoPresionaE.SetActive(false);

        // Ocultar texto de secuencia al inicio
        if (textoSecuencia != null)
            textoSecuencia.gameObject.SetActive(false);

        GenerarSecuenciaAleatoria();

        // Suscribir eventos de cristales
        for (int i = 0; i < cristalesInteractuables.Length; i++)
        {
            int index = i;
            cristalesInteractuables[i].OnCristalClickado += () => OnCristalActivado(index);
        }

        // Encontrar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
    }

    void GenerarSecuenciaAleatoria()
    {
        List<int> indices = new List<int>();
        int cantidadCristales = Mathf.Min(cristalesPista.Length, cristalesInteractuables.Length);

        for (int i = 0; i < cantidadCristales; i++)
        {
            indices.Add(i);
        }

        secuenciaCorrecta = MezclarLista(indices);

        if (mostrarSecuenciaEnConsola)
        {
            string secuenciaTexto = "Secuencia generada: ";
            for (int i = 0; i < secuenciaCorrecta.Count; i++)
            {
                secuenciaTexto += (secuenciaCorrecta[i] + 1);
                if (i < secuenciaCorrecta.Count - 1)
                    secuenciaTexto += " → ";
            }
            Debug.Log(secuenciaTexto);
        }
    }

    List<int> MezclarLista(List<int> lista)
    {
        List<int> listaMezclada = new List<int>(lista);
        int n = listaMezclada.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = listaMezclada[i];
            listaMezclada[i] = listaMezclada[j];
            listaMezclada[j] = temp;
        }

        return listaMezclada;
    }

    void Update()
    {
        if (puzzleCompletado || mostrandoPista) return;

        if (jugador != null)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);
            bool enRangoAhora = distancia <= rangoProximidad;

            if (enRangoAhora && !jugadorEnRango)
            {
                jugadorEnRango = true;
                if (textoPresionaE != null)
                {
                    textoPresionaE.SetActive(true);
                }
            }
            else if (!enRangoAhora && jugadorEnRango)
            {
                jugadorEnRango = false;
                if (textoPresionaE != null)
                    textoPresionaE.SetActive(false);
            }
        }

        if (jugadorEnRango && Input.GetKeyDown(teclaPista))
        {
            StartCoroutine(MostrarPistaSecuencia());
        }
    }

    IEnumerator MostrarPistaSecuencia()
    {
        if (mostrandoPista) yield break;

        mostrandoPista = true;

        // Activar texto de secuencia la primera vez
        if (textoSecuencia != null && !textoSecuencia.gameObject.activeSelf)
        {
            textoSecuencia.gameObject.SetActive(true);
            ActualizarTextoSecuencia();
        }

        if (textoPresionaE != null)
            textoPresionaE.SetActive(false);

        if (sonidoPista != null)
            audioSource.PlayOneShot(sonidoPista);

        int pistasAMostrar = Mathf.Min(pasoActual + 1, secuenciaCorrecta.Count);

        for (int i = 0; i < pistasAMostrar; i++)
        {
            int indiceCristal = secuenciaCorrecta[i];

            if (indiceCristal < cristalesPista.Length)
            {
                cristalesPista[indiceCristal].MostrarPista(tiempoPorCristal);
                yield return new WaitForSeconds(tiempoPorCristal + 0.3f);
            }
        }

        mostrandoPista = false;

        if (jugadorEnRango && textoPresionaE != null)
        {
            textoPresionaE.SetActive(true);
        }
    }

    void OnCristalActivado(int indiceCristal)
    {
        if (puzzleCompletado) return;

        if (indiceCristal == secuenciaCorrecta[pasoActual])
        {
            secuenciaJugador.Add(indiceCristal);

            // === CEREMONIA DE ACTIVACIÓN MEJORADA ===
            Transform puenteDestino = null;
            GameObject puenteObj = null;

            if (puentes != null && pasoActual < puentes.Length && puentes[pasoActual] != null)
            {
                puenteObj = puentes[pasoActual];
                puenteDestino = puenteObj.transform;
            }

            // Activar cristal (prepara el LineRenderer y configuración básica)
            cristalesInteractuables[indiceCristal].ActivarPermanente(puenteDestino);

            // INICIAR CEREMONIA (esto maneja todos los efectos visuales épicos)
            if (feedbackSystem != null)
            {
                StartCoroutine(feedbackSystem.CeremoniaActivacionCristal(
                    cristalesInteractuables[indiceCristal],
                    puenteObj,
                    pasoActual
                ));
            }
            else
            {
                // Fallback: comportamiento original si no hay feedback system
                if (sonidoCorrecto != null)
                    audioSource.PlayOneShot(sonidoCorrecto);

                // Activar puente manualmente
                if (puenteObj != null)
                    puenteObj.SetActive(true);
            }

            pasoActual++;
            ActualizarTextoSecuencia();

            Debug.Log($"¡Correcto! Cristal {indiceCristal + 1} activado. Paso {pasoActual}/{secuenciaCorrecta.Count}");

            if (pasoActual >= secuenciaCorrecta.Count)
            {
                CompletarPuzzle();
            }
        }
        else
        {
            // Error: cristal incorrecto
            if (sonidoError != null)
                audioSource.PlayOneShot(sonidoError);

            Debug.Log($"¡Error! Activaste el cristal {indiceCristal + 1} pero debías activar el cristal {secuenciaCorrecta[pasoActual] + 1}");

            ReiniciarSecuencia();
        }
    }

    void ReiniciarSecuencia()
    {
        Debug.Log("¡Secuencia incorrecta! Reiniciando...");

        // Efecto visual de error mejorado
        if (feedbackSystem != null)
        {
            // Los cristales pierden brillo temporalmente
            foreach (var cristal in cristalesInteractuables)
            {
                if (cristal.luzCristal != null)
                {
                    StartCoroutine(ReducirBrilloTemporal(cristal.luzCristal));
                }
            }
        }

        // Desactivar todos los cristales
        foreach (var cristal in cristalesInteractuables)
        {
            cristal.Desactivar();
        }

        // Desactivar todos los puentes
        if (puentes != null)
        {
            foreach (GameObject puente in puentes)
            {
                if (puente != null)
                    puente.SetActive(false);
            }
        }

        secuenciaJugador.Clear();
        pasoActual = 0;
        ActualizarTextoSecuencia();
    }

    IEnumerator ReducirBrilloTemporal(Light luz)
    {
        float intensidadOriginal = luz.intensity;
        float tiempo = 0f;
        float duracion = 0.5f;

        // Reducir a 30%
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(intensidadOriginal, intensidadOriginal * 0.3f, tiempo / duracion);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        // Restaurar
        tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(intensidadOriginal * 0.3f, intensidadOriginal, tiempo / duracion);
            yield return null;
        }

        luz.intensity = intensidadOriginal;
    }

    void CompletarPuzzle()
    {
        puzzleCompletado = true;

        Debug.Log("¡Puzzle completado! Iniciando secuencia final épica...");

        // SECUENCIA FINAL ÉPICA
        if (feedbackSystem != null)
        {
            StartCoroutine(feedbackSystem.SecuenciaFinalCompletado());
        }
        else
        {
            // Fallback: comportamiento original
            if (sonidoCompletado != null)
                audioSource.PlayOneShot(sonidoCompletado);

            if (puentes != null)
            {
                foreach (GameObject puente in puentes)
                {
                    if (puente != null)
                        puente.SetActive(true);
                }
            }

            StartCoroutine(EfectoCompletado());
        }

        if (textoPresionaE != null)
            textoPresionaE.SetActive(false);

        if (textoSecuencia != null)
            textoSecuencia.text = "¡COMPLETADO!";
    }

    IEnumerator EfectoCompletado()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var cristal in cristalesInteractuables)
            {
                cristal.PulsarLuz();
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void ActualizarTextoSecuencia()
    {
        if (textoSecuencia == null) return;

        textoSecuencia.text = $"Secuencia: {pasoActual}/{secuenciaCorrecta.Count}";
    }

    /// <summary>
    /// Método público para que el FeedbackSystem pueda obtener el siguiente cristal correcto
    /// </summary>
    public int ObtenerSiguienteCristalCorrecto()
    {
        if (pasoActual < secuenciaCorrecta.Count)
        {
            return secuenciaCorrecta[pasoActual];
        }
        return -1;
    }

    /// <summary>
    /// Método público para obtener el progreso actual
    /// </summary>
    public int ObtenerProgresoActual()
    {
        return pasoActual;
    }

    /// <summary>
    /// Método público para obtener el total de pasos
    /// </summary>
    public int ObtenerTotalPasos()
    {
        return secuenciaCorrecta.Count;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoProximidad);

        if (cristalesPista != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var cristal in cristalesPista)
            {
                if (cristal != null)
                {
                    Gizmos.DrawLine(transform.position, cristal.transform.position);
                }
            }
        }
    }
}