using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sistema de feedback mejorado para los cristales y el templo.
/// Maneja resonancia, ceremonias de activación, pilares del templo y secuencia final.
/// </summary>
public class CrystalFeedbackSystem : MonoBehaviour
{
    [Header("Referencias del Templo")]
    [Tooltip("GameObject raíz del templo")]
    public GameObject templo;

    [Tooltip("Pilares que se iluminan progresivamente (5 pilares)")]
    public GameObject[] pilaresTemplo;

    [Tooltip("Luces de los pilares")]
    public Light[] lucesPilares;

    [Tooltip("Transform del centro del templo (para efectos)")]
    public Transform centroTemplo;

    [Header("Referencias de Cristales")]
    [Tooltip("Todos los cristales interactuables del puzzle")]
    public CristalNode[] cristales;

    [Tooltip("Manager de secuencia (para escuchar eventos)")]
    public CrystalSequenceManager sequenceManager;

    [Header("Torre Final")]
    [Tooltip("Torre distante que se activa al completar")]
    public GameObject torreFinal;

    [Tooltip("Luz/Beacon de la torre")]
    public Light luzTorre;

    [Tooltip("Partículas de la torre")]
    public ParticleSystem particulasTorre;

    [Header("Configuración de Resonancia")]
    [Tooltip("Cada cuántos segundos pulsan los cristales activos")]
    public float intervaloPulso = 3f;

    [Tooltip("Intensidad del pulso de búsqueda")]
    public float intensidadPulsoBusqueda = 2f;

    [Tooltip("Radio del pulso de búsqueda")]
    public float radioPulsoBusqueda = 50f;

    [Header("Configuración de Ceremonia")]
    [Tooltip("Duración de la ceremonia de activación")]
    public float duracionCeremonia = 4f;

    [Tooltip("Intensidad del shake de cámara")]
    public float intensidadShake = 0.3f;

    [Header("Audio Mejorado")]
    public AudioClip sonidoPulsoResonancia;
    public AudioClip sonidoPilarActivado;
    public AudioClip sonidoRayoCreciendo;
    public AudioClip sonidoPuenteMaterializando;
    public AudioClip sonidoConvergenciaFinal;
    public AudioClip sonidoTorreActivada;

    [Header("Partículas")]
    public GameObject particulasHelix;
    public GameObject particulasOndaExpansiva;
    public GameObject particulasActivacionPilar;
    public GameObject particulasConvergencia;

    private AudioSource audioSource;
    private List<CristalNode> cristalesActivos = new List<CristalNode>();
    private Coroutine coroutineResonancia;
    private int cristalesActivadosCount = 0;
    private bool puzzleCompletado = false;
    private Camera mainCamera;
    private Vector3 posicionCamaraOriginal;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        mainCamera = Camera.main;
        if (mainCamera != null)
            posicionCamaraOriginal = mainCamera.transform.position;

        // Desactivar todos los pilares al inicio
        DesactivarPilares();

        // Suscribirse a eventos del CrystalSequenceManager
        // (Modificaremos el manager para invocar eventos)
    }

    #region 1. SISTEMA DE RESONANCIA CRISTALINA

    /// <summary>
    /// Inicia el sistema de resonancia cuando se activa el primer cristal
    /// </summary>
    public void IniciarResonancia()
    {
        if (coroutineResonancia == null)
        {
            coroutineResonancia = StartCoroutine(SistemaResonancia());
        }
    }

    IEnumerator SistemaResonancia()
    {
        while (!puzzleCompletado)
        {
            yield return new WaitForSeconds(intervaloPulso);

            if (cristalesActivos.Count > 0)
            {
                // Todos los cristales activos pulsan sincronizados
                foreach (CristalNode cristal in cristalesActivos)
                {
                    if (cristal != null)
                    {
                        StartCoroutine(PulsarCristalResonante(cristal));
                    }
                }

                // Reproducir sonido de resonancia
                if (sonidoPulsoResonancia != null)
                    audioSource.PlayOneShot(sonidoPulsoResonancia, 0.3f);

                // Emitir pulso de búsqueda que ilumina el siguiente cristal
                yield return new WaitForSeconds(0.5f);
                EmitirPulsoBusqueda();
            }
        }
    }

    IEnumerator PulsarCristalResonante(CristalNode cristal)
    {
        Light luz = cristal.luzCristal;
        if (luz != null)
        {
            float intensidadOriginal = luz.intensity;
            float tiempo = 0f;
            float duracion = 0.4f;

            // Aumentar intensidad
            while (tiempo < duracion / 2)
            {
                tiempo += Time.deltaTime;
                luz.intensity = Mathf.Lerp(intensidadOriginal, intensidadOriginal * 2f, tiempo / (duracion / 2));
                yield return null;
            }

            // Disminuir intensidad
            tiempo = 0f;
            while (tiempo < duracion / 2)
            {
                tiempo += Time.deltaTime;
                luz.intensity = Mathf.Lerp(intensidadOriginal * 2f, intensidadOriginal, tiempo / (duracion / 2));
                yield return null;
            }

            luz.intensity = intensidadOriginal;
        }
    }

    void EmitirPulsoBusqueda()
    {
        // Encuentra el índice del siguiente cristal correcto
        int siguienteIndice = sequenceManager != null ? ObtenerSiguienteCristalCorrecto() : -1;

        if (siguienteIndice >= 0 && siguienteIndice < cristales.Length)
        {
            CristalNode siguienteCristal = cristales[siguienteIndice];

            if (siguienteCristal != null && siguienteCristal.luzCristal != null)
            {
                StartCoroutine(IluminarBrevementeSiguienteCristal(siguienteCristal));
            }
        }
    }

    IEnumerator IluminarBrevementeSiguienteCristal(CristalNode cristal)
    {
        Light luz = cristal.luzCristal;
        Color colorOriginal = luz.color;
        float intensidadOriginal = luz.intensity;

        // Pulsar con color amarillo (guía)
        luz.color = Color.yellow;
        luz.intensity = intensidadPulsoBusqueda * 3f;

        // Crear efecto de partículas de "ping"
        if (particulasHelix != null)
        {
            Instantiate(particulasHelix, cristal.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.8f);

        // Restaurar
        luz.color = colorOriginal;
        luz.intensity = intensidadOriginal;
    }

    int ObtenerSiguienteCristalCorrecto()
    {
        if (sequenceManager != null)
        {
            return sequenceManager.ObtenerSiguienteCristalCorrecto();
        }
        return -1;
    }

    #endregion

    #region 2. PILARES DEL TEMPLO

    void DesactivarPilares()
    {
        if (pilaresTemplo == null) return;

        foreach (GameObject pilar in pilaresTemplo)
        {
            if (pilar != null)
            {
                // Desactivar renderer o luz
                Renderer[] renderers = pilar.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    Material mat = r.material;
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        if (lucesPilares != null)
        {
            foreach (Light luz in lucesPilares)
            {
                if (luz != null)
                    luz.enabled = false;
            }
        }
    }

    public void ActivarPilar(int index)
    {
        if (pilaresTemplo == null || index >= pilaresTemplo.Length) return;

        StartCoroutine(AnimarActivacionPilar(index));
    }

    IEnumerator AnimarActivacionPilar(int index)
    {
        GameObject pilar = pilaresTemplo[index];
        Light luz = lucesPilares != null && index < lucesPilares.Length ? lucesPilares[index] : null;

        // Sonido
        if (sonidoPilarActivado != null)
            audioSource.PlayOneShot(sonidoPilarActivado);

        // Partículas
        if (particulasActivacionPilar != null && pilar != null)
        {
            Instantiate(particulasActivacionPilar, pilar.transform.position, Quaternion.identity);
        }

        // Animar emisión del material
        if (pilar != null)
        {
            Renderer[] renderers = pilar.GetComponentsInChildren<Renderer>();
            float tiempo = 0f;
            float duracion = 1f;

            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float intensidad = Mathf.Lerp(0f, 3f, tiempo / duracion);

                foreach (Renderer r in renderers)
                {
                    Material mat = r.material;
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.SetColor("_EmissionColor", Color.cyan * intensidad);
                    }
                }

                yield return null;
            }
        }

        // Activar luz
        if (luz != null)
        {
            luz.enabled = true;
            luz.color = Color.cyan;
            luz.intensity = 0f;

            float tiempo = 0f;
            float duracion = 0.5f;

            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                luz.intensity = Mathf.Lerp(0f, 5f, tiempo / duracion);
                yield return null;
            }
        }

        Debug.Log($"Pilar {index + 1}/5 activado en el templo");
    }

    #endregion

    #region 3. CEREMONIA DE ACTIVACIÓN

    public IEnumerator CeremoniaActivacionCristal(CristalNode cristal, GameObject puente, int indiceCristal)
    {
        Debug.Log($"Iniciando ceremonia de activación para cristal {indiceCristal}");

        // 1. CRISTAL PULSA (0.5s)
        yield return StartCoroutine(PulsoCristalInicial(cristal));

        // 2. HELIX DE PARTÍCULAS (1s)
        yield return StartCoroutine(EmitirHelixParticulas(cristal));

        // 3. RAYO CRECE HACIA PUENTE (1s)
        yield return StartCoroutine(CrecerRayoEnergia(cristal, puente));

        // 4. PUENTE SE MATERIALIZA (1.5s)
        yield return StartCoroutine(MaterializarPuente(puente));

        // 5. ONDA EXPANSIVA (0.5s)
        yield return StartCoroutine(OndaExpansiva(puente));

        // 6. ACTIVAR PILAR DEL TEMPLO
        ActivarPilar(indiceCristal);

        // Añadir cristal a la lista de activos
        if (!cristalesActivos.Contains(cristal))
        {
            cristalesActivos.Add(cristal);
            cristalesActivadosCount++;
        }

        // Iniciar resonancia si es el primer cristal
        if (cristalesActivadosCount == 1)
        {
            IniciarResonancia();
        }
    }

    IEnumerator PulsoCristalInicial(CristalNode cristal)
    {
        Transform t = cristal.transform;
        Vector3 escalaOriginal = t.localScale;
        Light luz = cristal.luzCristal;
        float intensidadOriginal = luz != null ? luz.intensity : 1f;

        float tiempo = 0f;
        float duracion = 0.25f;

        // Expandir
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float escala = Mathf.Lerp(1f, 1.3f, tiempo / duracion);
            t.localScale = escalaOriginal * escala;

            if (luz != null)
                luz.intensity = Mathf.Lerp(intensidadOriginal, intensidadOriginal * 3f, tiempo / duracion);

            yield return null;
        }

        // Contraer
        tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float escala = Mathf.Lerp(1.3f, 1f, tiempo / duracion);
            t.localScale = escalaOriginal * escala;

            if (luz != null)
                luz.intensity = Mathf.Lerp(intensidadOriginal * 3f, intensidadOriginal, tiempo / duracion);

            yield return null;
        }

        t.localScale = escalaOriginal;
    }

    IEnumerator EmitirHelixParticulas(CristalNode cristal)
    {
        if (particulasHelix != null)
        {
            GameObject helix = Instantiate(particulasHelix, cristal.transform.position, Quaternion.identity);
            Destroy(helix, 2f);
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator CrecerRayoEnergia(CristalNode cristal, GameObject puente)
    {
        if (sonidoRayoCreciendo != null)
            audioSource.PlayOneShot(sonidoRayoCreciendo);

        LineRenderer line = cristal.lineRenderer;
        if (line != null && puente != null)
        {
            line.enabled = true;
            Vector3 inicio = cristal.transform.position;
            Vector3 fin = puente.transform.position;

            float tiempo = 0f;
            float duracion = 1f;

            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float progreso = tiempo / duracion;

                Vector3 puntoActual = Vector3.Lerp(inicio, fin, progreso);
                line.SetPosition(0, inicio);
                line.SetPosition(1, puntoActual);

                // Animar grosor
                line.startWidth = Mathf.Lerp(0f, 0.15f, progreso);
                line.endWidth = Mathf.Lerp(0f, 0.15f, progreso);

                yield return null;
            }

            line.SetPosition(1, fin);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator MaterializarPuente(GameObject puente)
    {
        if (puente == null) yield break;

        if (sonidoPuenteMaterializando != null)
            audioSource.PlayOneShot(sonidoPuenteMaterializando);

        puente.SetActive(true);

        // Animar materiales del puente (de transparente a opaco)
        Renderer[] renderers = puente.GetComponentsInChildren<Renderer>();

        float tiempo = 0f;
        float duracion = 1.5f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);

            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }

                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.SetColor("_EmissionColor", Color.cyan * (alpha * 2f));
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator OndaExpansiva(GameObject puente)
    {
        if (particulasOndaExpansiva != null && puente != null)
        {
            Instantiate(particulasOndaExpansiva, puente.transform.position, Quaternion.identity);
        }

        // Camera shake
        if (mainCamera != null)
        {
            StartCoroutine(CameraShake(intensidadShake, 0.3f));
        }

        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region 4. SECUENCIA FINAL ÉPICA

    public IEnumerator SecuenciaFinalCompletado()
    {
        puzzleCompletado = true;

        Debug.Log("¡Iniciando secuencia final épica!");

        // 1. TODOS LOS CRISTALES PULSAN AL UNÍSONO (2s)
        yield return StartCoroutine(PulsoSincronizadoFinal());

        // 2. CONVERGENCIA DE RAYOS HACIA EL TEMPLO (2s)
        yield return StartCoroutine(ConvergenciaRayosAlTemplo());

        // 3. TEMPLO ABSORBE ENERGÍA (1s)
        yield return StartCoroutine(TemploAbsorbeEnergia());

        // 4. EXPLOSIÓN DE LUZ (0.5s)
        yield return StartCoroutine(ExplosionLuzTemplo());

        // 5. PUENTE FINAL HACIA LA TORRE (3s)
        yield return StartCoroutine(MaterializarPuenteFinal());

        // 6. ACTIVAR TORRE (2s)
        yield return StartCoroutine(ActivarTorreFinal());

        Debug.Log("¡Secuencia final completada!");
    }

    IEnumerator PulsoSincronizadoFinal()
    {
        if (sonidoConvergenciaFinal != null)
            audioSource.PlayOneShot(sonidoConvergenciaFinal);

        float tiempo = 0f;
        float duracion = 2f;
        int pulsos = 4;

        for (int i = 0; i < pulsos; i++)
        {
            foreach (CristalNode cristal in cristalesActivos)
            {
                if (cristal != null && cristal.luzCristal != null)
                {
                    StartCoroutine(PulsarCristalResonante(cristal));
                }
            }

            yield return new WaitForSeconds(duracion / pulsos);
        }
    }

    IEnumerator ConvergenciaRayosAlTemplo()
    {
        if (centroTemplo == null) yield break;

        // Redirigir todos los rayos hacia el centro del templo
        float tiempo = 0f;
        float duracion = 2f;

        Dictionary<LineRenderer, Vector3> puntosFinalesOriginales = new Dictionary<LineRenderer, Vector3>();

        // Guardar posiciones finales originales
        foreach (CristalNode cristal in cristalesActivos)
        {
            if (cristal != null && cristal.lineRenderer != null)
            {
                LineRenderer line = cristal.lineRenderer;
                puntosFinalesOriginales[line] = line.GetPosition(1);
            }
        }

        // Animar hacia el centro del templo
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            foreach (CristalNode cristal in cristalesActivos)
            {
                if (cristal != null && cristal.lineRenderer != null)
                {
                    LineRenderer line = cristal.lineRenderer;
                    Vector3 puntoFinalOriginal = puntosFinalesOriginales[line];
                    Vector3 puntoNuevo = Vector3.Lerp(puntoFinalOriginal, centroTemplo.position, progreso);
                    line.SetPosition(1, puntoNuevo);

                    // Aumentar grosor
                    float grosor = Mathf.Lerp(0.15f, 0.3f, progreso);
                    line.startWidth = grosor;
                    line.endWidth = grosor;
                }
            }

            yield return null;
        }
    }

    IEnumerator TemploAbsorbeEnergia()
    {
        if (particulasConvergencia != null && centroTemplo != null)
        {
            GameObject convergencia = Instantiate(particulasConvergencia, centroTemplo.position, Quaternion.identity);
            Destroy(convergencia, 2f);
        }

        // Camera shake fuerte
        StartCoroutine(CameraShake(0.5f, 1f));

        yield return new WaitForSeconds(1f);

        // Ocultar todos los rayos
        foreach (CristalNode cristal in cristalesActivos)
        {
            if (cristal != null && cristal.lineRenderer != null)
            {
                cristal.lineRenderer.enabled = false;
            }
        }
    }

    IEnumerator ExplosionLuzTemplo()
    {
        if (centroTemplo == null) yield break;

        // Crear luz intensa en el centro
        GameObject lightObj = new GameObject("ExplosionLuz");
        lightObj.transform.position = centroTemplo.position;
        Light luz = lightObj.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.color = Color.white;
        luz.intensity = 0f;
        luz.range = 50f;

        float tiempo = 0f;
        float duracion = 0.5f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(0f, 20f, tiempo / duracion);
            yield return null;
        }

        // Desvanecer
        tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(20f, 0f, tiempo / duracion);
            yield return null;
        }

        Destroy(lightObj);
    }

    IEnumerator MaterializarPuenteFinal()
    {
        // Este método necesitaría referencia al puente final
        // Por ahora solo esperamos
        yield return new WaitForSeconds(3f);
    }

    IEnumerator ActivarTorreFinal()
    {
        if (torreFinal != null)
        {
            torreFinal.SetActive(true);
        }

        if (sonidoTorreActivada != null)
            audioSource.PlayOneShot(sonidoTorreActivada);

        if (luzTorre != null)
        {
            luzTorre.enabled = true;
            luzTorre.intensity = 0f;

            float tiempo = 0f;
            float duracion = 2f;

            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                luzTorre.intensity = Mathf.Lerp(0f, 10f, tiempo / duracion);
                yield return null;
            }
        }

        if (particulasTorre != null)
        {
            particulasTorre.Play();
        }

        yield return new WaitForSeconds(1f);
    }

    #endregion

    #region UTILIDADES

    IEnumerator CameraShake(float intensidad, float duracion)
    {
        if (mainCamera == null) yield break;

        Vector3 posicionOriginal = mainCamera.transform.position;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            float x = Random.Range(-1f, 1f) * intensidad;
            float y = Random.Range(-1f, 1f) * intensidad;

            mainCamera.transform.position = posicionOriginal + new Vector3(x, y, 0f);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = posicionOriginal;
    }

    #endregion
}