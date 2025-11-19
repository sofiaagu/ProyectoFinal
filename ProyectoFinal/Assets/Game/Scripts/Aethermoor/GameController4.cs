using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController4 : MonoBehaviour
{
    // ============================================================
    // ======================= REFERENCIAS =========================
    // ============================================================

    [Header("📜 UI - Monedas y Tiempo")]
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoTiempo;

    [Header("📜 UI - Cristales")]
    [Tooltip("Panel que contiene los 5 nombres de cristales")]
    public GameObject panelNombresCristales;
    
    [Tooltip("Los 5 textos de nombres de cristales (en orden: Morado, Celeste, Naranja, Rojo, Azul)")]
    public TextMeshProUGUI[] nombresCristales = new TextMeshProUGUI[5];
    
    [Tooltip("Textos iniciales (opcional, si no se asigna usa nombres genéricos)")]
    public string[] nombresIniciales = new string[5] 
    { 
        "🟣 Cristal Morado", 
        "🔵 Cristal Celeste", 
        "🟠 Cristal Naranja", 
        "🔴 Cristal Rojo", 
        "🔵 Cristal Azul Oscuro" 
    };

    [Header("📜 UI - Habilidad de Equilibrio")]
    [Tooltip("Panel que se muestra SOLO cuando el jugador tiene la habilidad")]
    public GameObject panelEquilibrio;
    public TextMeshProUGUI textoEquilibrio;
    public Image imagenEstadoEquilibrio;
    public Color colorActivo = new Color(0.5f, 0f, 1f); // Morado para plano etéreo
    public Color colorInactivo = new Color(0f, 0.5f, 1f); // Azul para plano físico

    [Header("📌 Referencias de escena")]
    public Timer timer;
    public CrystalSequenceManager crystalManager;

    [Header("⚙️ Configuración")]
    public int totalCristales = 5;
    public string nombreHabilidadEquilibrio = "Equilibrio Dimensional";

    // ============================================================
    // ==================== VARIABLES INTERNAS =====================
    // ============================================================

    private int monedasEscena = 0;
    private List<int> cristalesActivados = new List<int>();
    private bool tieneHabilidadEquilibrio = false;
    private bool equilibrioActivo = false; // Si está en plano etéreo
    private bool escenaCompletada = false;

    // ============================================================
    // ========================= START =============================
    // ============================================================

    private void Start()
    {
        // Buscar referencias automáticamente si no están asignadas
        if (timer == null)
            timer = FindFirstObjectByType<Timer>();

        if (crystalManager == null)
            crystalManager = FindFirstObjectByType<CrystalSequenceManager>();

        InicializarUI();
        ActualizarUI();
    }

    private void Update()
    {
        // Actualizar tiempo en cada frame
        if (timer != null && !escenaCompletada)
        {
            ActualizarUITiempo();
        }
    }

    // ============================================================
    // =================== INICIALIZACIÓN UI =======================
    // ============================================================

    private void InicializarUI()
    {
        // Configurar panel de cristales
        if (panelNombresCristales != null)
        {
            panelNombresCristales.SetActive(true);
        }

        // Asignar nombres iniciales a los textos
        for (int i = 0; i < nombresCristales.Length && i < nombresIniciales.Length; i++)
        {
            if (nombresCristales[i] != null)
            {
                nombresCristales[i].text = nombresIniciales[i];
                nombresCristales[i].gameObject.SetActive(true);
            }
        }

        // IMPORTANTE: Panel de equilibrio OCULTO hasta que recoja la habilidad
        if (panelEquilibrio != null)
        {
            panelEquilibrio.SetActive(false);
        }
    }

    // ============================================================
    // =================== SISTEMA DE CRISTALES ====================
    // ============================================================

    /// <summary>
    /// Se llama cuando un cristal es activado correctamente
    /// </summary>
    /// <param name="indiceCristal">Índice del cristal en el array (0-4)</param>
    public void CristalActivado(int indiceCristal)
    {
        if (indiceCristal < 0 || indiceCristal >= totalCristales)
        {
            Debug.LogWarning($"⚠️ Índice de cristal inválido: {indiceCristal}");
            return;
        }

        if (cristalesActivados.Contains(indiceCristal))
        {
            Debug.LogWarning($"⚠️ Cristal {indiceCristal} ya estaba activado");
            return;
        }

        cristalesActivados.Add(indiceCristal);
        OcultarNombreCristal(indiceCristal);

        Debug.Log($"✅ Cristal {indiceCristal} activado | Progreso: {cristalesActivados.Count}/{totalCristales}");

        // Verificar si se completó el puzzle
        if (cristalesActivados.Count >= totalCristales)
        {
            OnPuzzleCompletado();
        }
    }

    private void OcultarNombreCristal(int indice)
    {
        if (indice >= 0 && indice < nombresCristales.Length && nombresCristales[indice] != null)
        {
            // Opción 1: Tachar el texto
            nombresCristales[indice].text = "<s>" + nombresIniciales[indice] + "</s>";
            nombresCristales[indice].color = new Color(1f, 1f, 1f, 0.3f); // Semi-transparente

        }
    }


    private void OnPuzzleCompletado()
    {
        Debug.Log("🎉 ¡Todos los cristales activados! Puzzle completado.");
    }

    // ============================================================
    // ================ SISTEMA DE HABILIDAD EQUILIBRIO ============
    // ============================================================

    /// <summary>
    /// Llama este método cuando el jugador RECOJA la habilidad de equilibrio
    /// (por ejemplo, desde un AbilityPickup)
    /// </summary>
    public void ObtenerHabilidadEquilibrio()
    {
        if (tieneHabilidadEquilibrio)
        {
            Debug.LogWarning("El jugador ya tiene la habilidad de equilibrio");
            return;
        }

        tieneHabilidadEquilibrio = true;
        
        // MOSTRAR el panel de equilibrio
        if (panelEquilibrio != null)
        {
            panelEquilibrio.SetActive(true);
        }

        ActualizarUIEquilibrio();
        
        Debug.Log("✨ Habilidad de Equilibrio Dimensional OBTENIDA - Panel ahora visible");
    }

    /// <summary>
    /// Cambia el estado visual del panel de equilibrio (físico/etéreo)
    /// Solo funciona si ya tiene la habilidad
    /// </summary>
    /// <param name="enPlanoEtereo">True si está en plano etéreo, False si está en físico</param>
    public void CambiarEstadoEquilibrio(bool enPlanoEtereo)
    {
        if (!tieneHabilidadEquilibrio)
        {
            Debug.LogWarning("El jugador aún no tiene la habilidad de equilibrio");
            return;
        }

        equilibrioActivo = enPlanoEtereo;
        ActualizarUIEquilibrio();

        string estadoActual = enPlanoEtereo ? "ETÉREO" : "FÍSICO";
        Debug.Log($"⚖️ Cambio de plano → {estadoActual}");
    }

    public bool TieneHabilidadEquilibrio()
    {
        return tieneHabilidadEquilibrio;
    }

    public bool EstaEnPlanoEtereo()
    {
        return equilibrioActivo;
    }

    private void ActualizarUIEquilibrio()
    {
        if (textoEquilibrio != null)
        {
            string estado = equilibrioActivo ? "PLANO ETÉREO" : "PLANO FÍSICO";
            string emoji = equilibrioActivo ? "👻" : "🌍";
            textoEquilibrio.text = $"{emoji} {nombreHabilidadEquilibrio}\n<size=70%>{estado}</size>";
        }

        if (imagenEstadoEquilibrio != null)
        {
            imagenEstadoEquilibrio.color = equilibrioActivo ? colorActivo : colorInactivo;
        }
    }

    // ============================================================
    // ======================= MONEDAS =============================
    // ============================================================

    public void RegistrarMoneda(int cantidad)
    {
        monedasEscena += cantidad;
        ActualizarUI();

        if (GameManager.instance != null)
            GameManager.instance.AgregarMoneda(cantidad);

        Debug.Log($"💰 Moneda recogida | Total en escena: {monedasEscena}");
    }

    public int ObtenerMonedasEscena()
    {
        return monedasEscena;
    }

    // ============================================================
    // ====================== TIEMPO ===============================
    // ============================================================

    public float ObtenerTiempoActual()
    {
        if (timer != null)
            return timer.StopTime;
        return 0f;
    }

    public string ObtenerTiempoFormateado()
    {
        if (timer != null)
        {
            float tiempo = timer.StopTime;
            int minutos = Mathf.FloorToInt(tiempo / 60f);
            int segundos = Mathf.FloorToInt(tiempo % 60f);
            int milisegundos = Mathf.FloorToInt((tiempo * 100f) % 100f);
            return string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, milisegundos);
        }
        return "00:00:00";
    }

    // ============================================================
    // ===================== FINAL DE ESCENA ========================
    // ============================================================

    public void CompletarEscena()
    {
        if (escenaCompletada)
            return;

        escenaCompletada = true;

        Debug.Log("🏁 Escena 3 completada. Registrando datos…");

        // Detener timer
        if (timer != null)
        {
            timer.TimerStop();

            // Registrar tiempo en GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.RegistrarTiempo(timer.StopTime);
                Debug.Log($"⏱️ Tiempo registrado: {ObtenerTiempoFormateado()}");
            }
        }

        // Registrar cristales completados
        Debug.Log($"💎 Cristales activados: {cristalesActivados.Count}/{totalCristales}");

        // Aquí puedes cargar la siguiente escena
        // SceneManager.LoadScene("Escena4");
    }

    // ============================================================
    // ========================= UI UPDATE =========================
    // ============================================================

    private void ActualizarUI()
    {
        if (textoMonedas != null)
            textoMonedas.text = $"💰 {monedasEscena}";

        ActualizarUITiempo();
    }

    private void ActualizarUITiempo()
    {
        if (textoTiempo != null && timer != null)
        {
            textoTiempo.text = $"⏱️ {ObtenerTiempoFormateado()}";
        }
    }

    // ============================================================
    // ===================== MÉTODOS AUXILIARES ====================
    // ============================================================

    public int ObtenerCristalesActivados()
    {
        return cristalesActivados.Count;
    }

    public List<int> ObtenerListaCristalesActivados()
    {
        return new List<int>(cristalesActivados);
    }

    public bool TodosLosCristalesActivados()
    {
        return cristalesActivados.Count >= totalCristales;
    }


    // ============================================================
    // =================== MÉTODOS DE DEBUG ========================
    // ============================================================

    [ContextMenu("Activar Todos los Cristales (Debug)")]
    private void DebugActivarTodosCristales()
    {
        for (int i = 0; i < totalCristales; i++)
        {
            CristalActivado(i);
        }
    }

    [ContextMenu("Obtener Habilidad Equilibrio (Debug)")]
    private void DebugObtenerEquilibrio()
    {
        ObtenerHabilidadEquilibrio();
    }

    [ContextMenu("Cambiar a Plano Etéreo (Debug)")]
    private void DebugCambiarAEtereo()
    {
        CambiarEstadoEquilibrio(true);
    }

    [ContextMenu("Cambiar a Plano Físico (Debug)")]
    private void DebugCambiarAFisico()
    {
        CambiarEstadoEquilibrio(false);
    }

    [ContextMenu("Añadir 10 Monedas (Debug)")]
    private void DebugAñadirMonedas()
    {
        RegistrarMoneda(10);
    }

}