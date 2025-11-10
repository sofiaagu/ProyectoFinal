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

    [Header("Puente Final")]
    public GameObject puenteFinal;

    [Header("UI")]
    public GameObject textoPresionaE;
    public TMPro.TextMeshProUGUI textoSecuencia;

    [Header("Audio")]
    public AudioClip sonidoPista;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoError;
    public AudioClip sonidoCompletado;

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

        if (puenteFinal != null)
            puenteFinal.SetActive(false);

        if (textoPresionaE != null)
            textoPresionaE.SetActive(false);

        // Generar secuencia aleatoria
        GenerarSecuenciaAleatoria();

        // Suscribir eventos de cristales interactuables
        for (int i = 0; i < cristalesInteractuables.Length; i++)
        {
            int index = i; // Capturar índice para el closure
            cristalesInteractuables[i].OnCristalClickado += () => OnCristalActivado(index);
        }

        // Encontrar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;

        ActualizarTextoSecuencia();
    }

    /// <summary>
    /// Genera una secuencia aleatoria única para este juego
    /// </summary>
    void GenerarSecuenciaAleatoria()
    {
        // Crear lista con todos los índices disponibles
        List<int> indices = new List<int>();
        int cantidadCristales = Mathf.Min(cristalesPista.Length, cristalesInteractuables.Length);

        for (int i = 0; i < cantidadCristales; i++)
        {
            indices.Add(i);
        }

        // Mezclar la lista usando algoritmo Fisher-Yates
        secuenciaCorrecta = MezclarLista(indices);

        // Debug: Mostrar secuencia generada
        if (mostrarSecuenciaEnConsola)
        {
            string secuenciaTexto = "Secuencia generada: ";
            for (int i = 0; i < secuenciaCorrecta.Count; i++)
            {
                secuenciaTexto += (secuenciaCorrecta[i] + 1); // +1 para mostrar 1-5 en vez de 0-4
                if (i < secuenciaCorrecta.Count - 1)
                    secuenciaTexto += " → ";
            }
            Debug.Log(secuenciaTexto);
        }
    }

    /// <summary>
    /// Mezcla una lista usando el algoritmo Fisher-Yates
    /// </summary>
    List<int> MezclarLista(List<int> lista)
    {
        List<int> listaMezclada = new List<int>(lista);
        int n = listaMezclada.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // Intercambiar
            int temp = listaMezclada[i];
            listaMezclada[i] = listaMezclada[j];
            listaMezclada[j] = temp;
        }

        return listaMezclada;
    }

    void Update()
    {
        if (puzzleCompletado || mostrandoPista) return;

        // Verificar proximidad del jugador
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

        // Input para mostrar pista (solo si está en rango)
        if (jugadorEnRango && Input.GetKeyDown(teclaPista))
        {
            StartCoroutine(MostrarPistaSecuencia());
        }
    }

    IEnumerator MostrarPistaSecuencia()
    {
        if (mostrandoPista) yield break;

        mostrandoPista = true;

        if (textoPresionaE != null)
            textoPresionaE.SetActive(false);

        // Reproducir sonido de pista
        if (sonidoPista != null)
            audioSource.PlayOneShot(sonidoPista);

        // Mostrar la secuencia hasta el paso actual + 1
        int pistasAMostrar = Mathf.Min(pasoActual + 1, secuenciaCorrecta.Count);

        for (int i = 0; i < pistasAMostrar; i++)
        {
            int indiceCristal = secuenciaCorrecta[i];

            if (indiceCristal < cristalesPista.Length)
            {
                // Encender cristal de pista temporalmente
                cristalesPista[indiceCristal].MostrarPista(tiempoPorCristal);
                yield return new WaitForSeconds(tiempoPorCristal + 0.3f);
            }
        }

        mostrandoPista = false;

        // Mostrar texto de nuevo si está en rango
        if (jugadorEnRango && textoPresionaE != null)
        {
            textoPresionaE.SetActive(true);
        }
    }

    void OnCristalActivado(int indiceCristal)
    {
        if (puzzleCompletado) return;

        // Verificar si es el cristal correcto en la secuencia
        if (indiceCristal == secuenciaCorrecta[pasoActual])
        {
            // ¡Correcto!
            secuenciaJugador.Add(indiceCristal);
            cristalesInteractuables[indiceCristal].ActivarPermanente();

            if (sonidoCorrecto != null)
                audioSource.PlayOneShot(sonidoCorrecto);

            pasoActual++;
            ActualizarTextoSecuencia();

            Debug.Log($"¡Correcto! Cristal {indiceCristal + 1} activado. Paso {pasoActual}/{secuenciaCorrecta.Count}");

            // Verificar si completó la secuencia
            if (pasoActual >= secuenciaCorrecta.Count)
            {
                CompletarPuzzle();
            }
        }
        else
        {
            // ¡Error! Reiniciar secuencia
            if (sonidoError != null)
                audioSource.PlayOneShot(sonidoError);

            Debug.Log($"¡Error! Activaste el cristal {indiceCristal + 1} pero debías activar el cristal {secuenciaCorrecta[pasoActual] + 1}");

            ReiniciarSecuencia();
        }
    }

    void ReiniciarSecuencia()
    {
        Debug.Log("¡Secuencia incorrecta! Reiniciando...");

        // Desactivar todos los cristales interactuables
        foreach (var cristal in cristalesInteractuables)
        {
            cristal.Desactivar();
        }

        secuenciaJugador.Clear();
        pasoActual = 0;
        ActualizarTextoSecuencia();
    }

    void CompletarPuzzle()
    {
        puzzleCompletado = true;

        Debug.Log("¡Puzzle completado! Puente activado.");

        if (sonidoCompletado != null)
            audioSource.PlayOneShot(sonidoCompletado);

        // Activar puente final
        if (puenteFinal != null)
        {
            puenteFinal.SetActive(true);
        }

        // Ocultar UI
        if (textoPresionaE != null)
            textoPresionaE.SetActive(false);

        if (textoSecuencia != null)
            textoSecuencia.text = "¡COMPLETADO!";

        // Efecto visual en todos los cristales
        StartCoroutine(EfectoCompletado());
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

    void OnDrawGizmosSelected()
    {
        // Dibujar rango de proximidad
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoProximidad);

        // Mostrar conexiones con cristales de pista
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