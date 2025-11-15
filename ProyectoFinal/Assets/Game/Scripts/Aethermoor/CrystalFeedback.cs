using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sistema de feedback mejorado para los cristales y el templo.
/// VERSIÓN MULTI-COLOR: Cada cristal tiene su propio color único.
/// </summary>
public class CrystalFeedbackSystem : MonoBehaviour
{
    [Header("Colores de los Cristales")]
    [Tooltip("Define el color único de cada cristal (5 colores)")]
    public Color[] coloresCristales = new Color[5]
    {
        new Color(0.5f, 0f, 1f),    // 0: Morado
        new Color(0f, 0.8f, 1f),    // 1: Celeste
        new Color(1f, 0.6f, 0f),    // 2: Naranja
        new Color(1f, 0f, 0f),      // 3: Rojo
        new Color(0f, 0.2f, 0.8f)   // 4: Azul Oscuro
    };

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

    [Header("Partículas (Prefabs)")]
    public GameObject particulasHelixPrefab;
    public GameObject particulasOndaExpansivaPrefab;
    public GameObject particulasActivacionPilarPrefab;
    public GameObject particulasConvergenciaPrefab;

    private AudioSource audioSource;
    private List<CristalNode> cristalesActivos = new List<CristalNode>();
    private List<int> indicesCristalesActivos = new List<int>();
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
        {
            // Guardar la posición Y ROTACIÓN original
            posicionCamaraOriginal = mainCamera.transform.position;
        }

        DesactivarPilares();
        AplicarColoresIniciales();
    }

    void AplicarColoresIniciales()
    {
        // Aplicar colores a cada cristal según su índice
        for (int i = 0; i < cristales.Length && i < coloresCristales.Length; i++)
        {
            if (cristales[i] != null)
            {
                // Establecer el color del cristal
                cristales[i].colorActivo = coloresCristales[i];

                // Aplicar color a la luz si existe
                if (cristales[i].luzCristal != null)
                {
                    cristales[i].luzCristal.color = coloresCristales[i];
                }
            }
        }
    }

    Color ObtenerColorCristal(int indice)
    {
        if (indice >= 0 && indice < coloresCristales.Length)
            return coloresCristales[indice];
        return Color.white;
    }

    #region 1. SISTEMA DE RESONANCIA CRISTALINA

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
                for (int i = 0; i < cristalesActivos.Count; i++)
                {
                    CristalNode cristal = cristalesActivos[i];
                    int indiceCristal = indicesCristalesActivos[i];

                    if (cristal != null)
                    {
                        StartCoroutine(PulsarCristalResonante(cristal));

                        // NUEVO: Emitir helix periódico desde cristal hacia puente
                        StartCoroutine(EmitirHelixPeriodico(cristal, indiceCristal));
                    }
                }

                if (sonidoPulsoResonancia != null)
                    audioSource.PlayOneShot(sonidoPulsoResonancia, 0.3f);

                yield return new WaitForSeconds(0.5f);
                EmitirPulsoBusqueda();
            }
        }
    }

    IEnumerator EmitirHelixPeriodico(CristalNode cristal, int indice)
    {
        if (particulasHelixPrefab == null) yield break;

        Color colorCristal = ObtenerColorCristal(indice);

        // Crear helix MÁS GRANDE que viaja desde el cristal hacia el puente
        GameObject helix = Instantiate(particulasHelixPrefab, cristal.transform.position, Quaternion.identity);
        helix.transform.localScale = Vector3.one * 2f; // MÁS GRANDE
        AplicarColorAParticulas(helix, colorCristal);

        // Si hay LineRenderer, hacer que las partículas viajen por el rayo
        if (cristal.lineRenderer != null && cristal.lineRenderer.enabled)
        {
            Vector3 destino = cristal.lineRenderer.GetPosition(1);
            float duracion = 2f; // MÁS LENTO (antes 1.5s)
            float tiempo = 0f;

            while (tiempo < duracion && helix != null)
            {
                tiempo += Time.deltaTime;
                float progreso = tiempo / duracion;

                helix.transform.position = Vector3.Lerp(cristal.transform.position, destino, progreso);

                yield return null;
            }
        }

        Destroy(helix, 1.5f);
    }

    IEnumerator PulsarCristalResonante(CristalNode cristal)
    {
        Light luz = cristal.luzCristal;
        if (luz != null)
        {
            float intensidadOriginal = luz.intensity;
            float tiempo = 0f;
            float duracion = 0.4f;

            while (tiempo < duracion / 2)
            {
                tiempo += Time.deltaTime;
                luz.intensity = Mathf.Lerp(intensidadOriginal, intensidadOriginal * 2f, tiempo / (duracion / 2));
                yield return null;
            }

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
        int siguienteIndice = sequenceManager != null ? sequenceManager.ObtenerSiguienteCristalCorrecto() : -1;

        if (siguienteIndice >= 0 && siguienteIndice < cristales.Length)
        {
            CristalNode siguienteCristal = cristales[siguienteIndice];

            if (siguienteCristal != null && siguienteCristal.luzCristal != null)
            {
                // CRÍTICO: Usar el índice REAL del cristal para el color
                int indiceRealCristal = System.Array.IndexOf(cristales, siguienteCristal);
                StartCoroutine(IluminarBrevementeSiguienteCristal(siguienteCristal, indiceRealCristal));
            }
        }
    }

    IEnumerator IluminarBrevementeSiguienteCristal(CristalNode cristal, int indiceReal)
    {
        Light luz = cristal.luzCristal;
        Color colorOriginal = luz.color;
        float intensidadOriginal = luz.intensity;

        // Pulsar con su propio color (índice REAL) pero más brillante
        Color colorBrillante = ObtenerColorCristal(indiceReal) * 1.5f;
        luz.color = colorBrillante;
        luz.intensity = intensidadPulsoBusqueda * 3f;

        if (particulasHelixPrefab != null)
        {
            GameObject particulas = Instantiate(particulasHelixPrefab, cristal.transform.position, Quaternion.identity);
            particulas.transform.localScale = Vector3.one * 2f; // MÁS GRANDE
            AplicarColorAParticulas(particulas, ObtenerColorCristal(indiceReal));
            Destroy(particulas, 2.5f);
        }

        yield return new WaitForSeconds(0.8f);

        luz.color = colorOriginal;
        luz.intensity = intensidadOriginal;
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

        // Obtener el color del cristal correspondiente
        Color colorPilar = ObtenerColorCristal(index);

        if (sonidoPilarActivado != null)
            audioSource.PlayOneShot(sonidoPilarActivado);

        if (particulasActivacionPilarPrefab != null && pilar != null)
        {
            GameObject particulas = Instantiate(particulasActivacionPilarPrefab, pilar.transform.position, Quaternion.identity);
            AplicarColorAParticulas(particulas, colorPilar);
            Destroy(particulas, 3f);
        }

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
                        mat.SetColor("_EmissionColor", colorPilar * intensidad);
                    }
                }

                yield return null;
            }
        }

        if (luz != null)
        {
            luz.enabled = true;
            luz.color = colorPilar;
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

        Debug.Log($"Pilar {index + 1}/5 activado con color {colorPilar}");
    }

    #endregion

    #region 3. CEREMONIA DE ACTIVACIÓN

    public IEnumerator CeremoniaActivacionCristal(CristalNode cristal, GameObject puente, int indiceCristal)
    {
        // CRÍTICO: Obtener el índice REAL del cristal en el array, no el índice de la secuencia
        int indiceRealCristal = System.Array.IndexOf(cristales, cristal);

        Debug.Log($"Iniciando ceremonia - Índice secuencia: {indiceCristal}, Índice real cristal: {indiceRealCristal}");

        Color colorCristal = ObtenerColorCristal(indiceRealCristal);

        // 1. CRISTAL PULSA (0.5s)
        yield return StartCoroutine(PulsoCristalInicial(cristal));

        // 2. HELIX DE PARTÍCULAS (1s)
        yield return StartCoroutine(EmitirHelixParticulas(cristal, colorCristal));

        // 3. RAYO CRECE HACIA PUENTE (1s)
        yield return StartCoroutine(CrecerRayoEnergia(cristal, puente, colorCristal));

        // 4. PUENTE SE MATERIALIZA (1.5s)
        yield return StartCoroutine(MaterializarPuente(puente, colorCristal));

        // 5. ONDA EXPANSIVA (0.5s)
        yield return StartCoroutine(OndaExpansiva(puente, colorCristal));

        // 6. ACTIVAR PILAR DEL TEMPLO (usar índice REAL del cristal)
        ActivarPilar(indiceRealCristal);

        // Añadir cristal a la lista de activos con su índice REAL
        if (!cristalesActivos.Contains(cristal))
        {
            cristalesActivos.Add(cristal);
            indicesCristalesActivos.Add(indiceRealCristal); // Guardar índice REAL
            cristalesActivadosCount++;
        }

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

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float escala = Mathf.Lerp(1f, 1.3f, tiempo / duracion);
            t.localScale = escalaOriginal * escala;

            if (luz != null)
                luz.intensity = Mathf.Lerp(intensidadOriginal, intensidadOriginal * 3f, tiempo / duracion);

            yield return null;
        }

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

    IEnumerator EmitirHelixParticulas(CristalNode cristal, Color color)
    {
        if (particulasHelixPrefab != null)
        {
            GameObject helix = Instantiate(particulasHelixPrefab, cristal.transform.position, Quaternion.identity);

            // Hacer el helix MÁS GRANDE
            helix.transform.localScale = Vector3.one * 2f;

            AplicarColorAParticulas(helix, color);

            // Destruir después de MÁS TIEMPO
            Destroy(helix, 3f);
        }

        // Esperar MÁS tiempo antes de continuar
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator CrecerRayoEnergia(CristalNode cristal, GameObject puente, Color color)
    {
        if (sonidoRayoCreciendo != null)
            audioSource.PlayOneShot(sonidoRayoCreciendo);

        LineRenderer line = cristal.lineRenderer;
        if (line != null && puente != null)
        {
            line.enabled = true;
            line.startColor = color;
            line.endColor = color;

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

    IEnumerator MaterializarPuente(GameObject puente, Color color)
    {
        if (puente == null) yield break;

        if (sonidoPuenteMaterializando != null)
            audioSource.PlayOneShot(sonidoPuenteMaterializando);

        puente.SetActive(true);

        Renderer[] renderers = puente.GetComponentsInChildren<Renderer>();

        // FASE 1: Cristalización progresiva (partículas recorren el puente)
        float longitudPuente = 10f; // Ajusta según tu puente
        int numParticulas = 20;

        for (int i = 0; i < numParticulas; i++)
        {
            if (particulasOndaExpansivaPrefab != null)
            {
                float progreso = (float)i / numParticulas;
                Vector3 posicion = puente.transform.position + puente.transform.forward * (longitudPuente * progreso);

                GameObject particula = Instantiate(particulasOndaExpansivaPrefab, posicion, Quaternion.identity);
                AplicarColorAParticulas(particula, color);
                Destroy(particula, 1f);
            }

            yield return new WaitForSeconds(0.05f); // Rápido pero visible
        }

        // FASE 2: Materialización del material con ondas
        float tiempo = 0f;
        float duracion = 1.2f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);
            float emissionIntensity = Mathf.Sin(tiempo * 10f) * 2f + 2f; // Pulsos

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
                        mat.SetColor("_EmissionColor", color * emissionIntensity);
                    }
                }
            }

            yield return null;
        }

        // FASE 3: Estabilización final
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor("_EmissionColor", color * 2f);
                }
            }
        }
    }

    IEnumerator OndaExpansiva(GameObject puente, Color color)
    {
        if (particulasOndaExpansivaPrefab != null && puente != null)
        {
            GameObject onda = Instantiate(particulasOndaExpansivaPrefab, puente.transform.position, Quaternion.identity);
            AplicarColorAParticulas(onda, color);
            Destroy(onda, 2f);
        }

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

        yield return StartCoroutine(PulsoSincronizadoFinal());
        yield return StartCoroutine(ConvergenciaRayosAlTemplo());
        yield return StartCoroutine(TemploAbsorbeEnergia());
        yield return StartCoroutine(ExplosionLuzTemplo());
        yield return StartCoroutine(MaterializarPuenteFinal());
        yield return StartCoroutine(ActivarTorreFinal());

        Debug.Log("¡Secuencia final completada!");
    }

    IEnumerator PulsoSincronizadoFinal()
    {
        if (sonidoConvergenciaFinal != null)
            audioSource.PlayOneShot(sonidoConvergenciaFinal);

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

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ConvergenciaRayosAlTemplo()
    {
        if (centroTemplo == null) yield break;

        float tiempo = 0f;
        float duracion = 2f;

        Dictionary<LineRenderer, Vector3> puntosFinalesOriginales = new Dictionary<LineRenderer, Vector3>();

        foreach (CristalNode cristal in cristalesActivos)
        {
            if (cristal != null && cristal.lineRenderer != null)
            {
                LineRenderer line = cristal.lineRenderer;
                puntosFinalesOriginales[line] = line.GetPosition(1);
            }
        }

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
        if (centroTemplo == null) yield break;

        // FASE 1: Crear esfera de absorción que crece
        GameObject esferaObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esferaObj.name = "EsferaConvergencia";
        esferaObj.transform.position = centroTemplo.position;
        esferaObj.transform.localScale = Vector3.zero;

        // Material brillante para la esfera
        Renderer esferaRenderer = esferaObj.GetComponent<Renderer>();
        Material matEsfera = new Material(Shader.Find("Standard"));
        matEsfera.EnableKeyword("_EMISSION");
        matEsfera.SetColor("_EmissionColor", Color.white * 5f);
        matEsfera.SetFloat("_Metallic", 1f);
        matEsfera.SetFloat("_Glossiness", 1f);
        esferaRenderer.material = matEsfera;

        // Luz de la esfera
        Light luzEsfera = esferaObj.AddComponent<Light>();
        luzEsfera.type = LightType.Point;
        luzEsfera.color = Color.white;
        luzEsfera.intensity = 0f;
        luzEsfera.range = 30f;

        // FASE 2: Emitir partículas desde cada cristal hacia el centro
        List<GameObject> particulasConvergentes = new List<GameObject>();

        for (int i = 0; i < cristalesActivos.Count; i++)
        {
            CristalNode cristal = cristalesActivos[i];
            int indiceCristal = indicesCristalesActivos[i];
            Color colorCristal = ObtenerColorCristal(indiceCristal);

            if (cristal != null && particulasConvergenciaPrefab != null)
            {
                // Crear múltiples ráfagas de partículas desde cada cristal
                for (int j = 0; j < 5; j++)
                {
                    GameObject particulas = Instantiate(particulasConvergenciaPrefab, cristal.transform.position, Quaternion.identity);
                    AplicarColorAParticulas(particulas, colorCristal);

                    // Mover partículas hacia el centro
                    StartCoroutine(MoverParticulasHaciaCentro(particulas, cristal.transform.position, centroTemplo.position, 1.5f));

                    particulasConvergentes.Add(particulas);

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        // FASE 3: Esfera crece mientras absorbe
        float tiempo = 0f;
        float duracionAbsorcion = 1.5f;

        while (tiempo < duracionAbsorcion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionAbsorcion;

            // Esfera crece
            esferaObj.transform.localScale = Vector3.one * Mathf.Lerp(0f, 3f, progreso);

            // Luz aumenta
            luzEsfera.intensity = Mathf.Lerp(0f, 15f, progreso);

            // Colores de la esfera ciclan entre los 5 colores
            Color colorCiclico = Color.Lerp(
                coloresCristales[(int)(Time.time * 5f) % 5],
                coloresCristales[((int)(Time.time * 5f) + 1) % 5],
                (Time.time * 5f) % 1f
            );
            matEsfera.SetColor("_EmissionColor", colorCiclico * 5f);
            luzEsfera.color = colorCiclico;

            yield return null;
        }

        // FASE 4: Shake intenso mientras absorbe
        StartCoroutine(CameraShake(0.5f, 1f));

        // FASE 5: Todas las partículas llegan, esfera pulsa violentamente
        tiempo = 0f;
        float duracionPulso = 0.5f;

        while (tiempo < duracionPulso)
        {
            tiempo += Time.deltaTime;

            // Pulso de escala
            float pulso = Mathf.Sin(tiempo * 30f) * 0.5f + 3f;
            esferaObj.transform.localScale = Vector3.one * pulso;

            // Pulso de luz
            luzEsfera.intensity = 15f + Mathf.Sin(tiempo * 30f) * 10f;

            yield return null;
        }

        // FASE 6: La esfera se estabiliza antes de explotar
        esferaObj.transform.localScale = Vector3.one * 2.5f;
        luzEsfera.intensity = 20f;
        matEsfera.SetColor("_EmissionColor", Color.white * 10f);
        luzEsfera.color = Color.white;

        yield return new WaitForSeconds(0.3f);

        // Limpiar partículas convergentes
        foreach (GameObject p in particulasConvergentes)
        {
            if (p != null) Destroy(p);
        }

        // Ocultar los rayos DESPUÉS de la absorción
        foreach (CristalNode cristal in cristalesActivos)
        {
            if (cristal != null && cristal.lineRenderer != null)
            {
                cristal.lineRenderer.enabled = false;
            }
        }

        // Guardar referencia para la explosión
        StartCoroutine(ExplotarEsfera(esferaObj, luzEsfera));
    }

    IEnumerator MoverParticulasHaciaCentro(GameObject particulas, Vector3 inicio, Vector3 destino, float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion && particulas != null)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            // Movimiento con aceleración (empieza lento, termina rápido)
            float progressionCurve = progreso * progreso;
            particulas.transform.position = Vector3.Lerp(inicio, destino, progressionCurve);

            yield return null;
        }
    }

    IEnumerator ExplotarEsfera(GameObject esfera, Light luz)
    {
        // La esfera "implosiona" rápidamente
        float tiempo = 0f;
        float duracion = 0.2f;
        Vector3 escalaInicial = esfera.transform.localScale;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            // Implosión
            esfera.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, progreso);
            luz.intensity = Mathf.Lerp(20f, 0f, progreso);

            yield return null;
        }

        Destroy(esfera);
        // La explosión de luz viene después en ExplosionLuzTemplo()
    }

    IEnumerator ExplosionLuzTemplo()
    {
        if (centroTemplo == null) yield break;

        // MEGA EXPLOSIÓN con múltiples ondas
        GameObject lightObj = new GameObject("ExplosionLuz");
        lightObj.transform.position = centroTemplo.position;
        Light luz = lightObj.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.color = Color.white;
        luz.intensity = 0f;
        luz.range = 100f; // MUCHO más grande

        // Crear múltiples ondas expansivas de partículas
        for (int i = 0; i < 3; i++)
        {
            if (particulasOndaExpansivaPrefab != null)
            {
                GameObject onda = Instantiate(particulasOndaExpansivaPrefab, centroTemplo.position, Quaternion.identity);

                // Escalar el sistema de partículas para que sea gigante
                ParticleSystem ps = onda.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startSpeed = 25f * (i + 1); // Cada onda más rápida
                    main.startSize = 1f * (i + 1);
                }

                Destroy(onda, 2f);
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Explosión de luz súper intensa
        float tiempo = 0f;
        float duracion = 0.3f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(0f, 30f, tiempo / duracion);
            luz.range = Mathf.Lerp(50f, 100f, tiempo / duracion);
            yield return null;
        }

        // Camera shake FUERTE
        StartCoroutine(CameraShake(0.8f, 0.5f));

        // Mantener luz máxima brevemente
        yield return new WaitForSeconds(0.2f);

        // Desvanecer lentamente (más dramático)
        tiempo = 0f;
        duracion = 1f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(30f, 0f, tiempo / duracion);
            yield return null;
        }

        Destroy(lightObj);
    }

    IEnumerator MaterializarPuenteFinal()
    {
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
            luzTorre.color = new Color(1f, 0.84f, 0f); // Dorado
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

    void AplicarColorAParticulas(GameObject particulasObj, Color color)
    {
        ParticleSystem ps = particulasObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = color;

            var colorOverLifetime = ps.colorOverLifetime;
            if (colorOverLifetime.enabled)
            {
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(color, 0f),
                        new GradientColorKey(Color.white, 0.5f),
                        new GradientColorKey(color, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(1f, 0.7f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );
                colorOverLifetime.color = grad;
            }
        }
    }

    IEnumerator CameraShake(float intensidad, float duracion)
    {
        if (mainCamera == null) yield break;

        // CRÍTICO: Guardar posición ACTUAL antes de cada shake
        Vector3 posicionInicial = mainCamera.transform.position;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            float x = Random.Range(-1f, 1f) * intensidad;
            float y = Random.Range(-1f, 1f) * intensidad;

            mainCamera.transform.position = posicionInicial + new Vector3(x, y, 0f);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // SIEMPRE volver a la posición inicial
        mainCamera.transform.position = posicionInicial;
    }

    #endregion
}