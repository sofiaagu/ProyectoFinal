using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    // ============================================================
    // ======================= REFERENCIAS =========================
    // ============================================================

    [Header("📜 UI")]
    public TextMeshProUGUI textoFragmentos;
    public TextMeshProUGUI textoEnemigos;

    [Header("📌 Referencias de escena")]
    public PuertaController puerta;
    public GameObject boss;
    public Transform bossSpawnPoint;
    public PortalSC portalSalida;

    // ============================================================
    // ==================== VARIABLES INTERNAS =====================
    // ============================================================

    private int fragmentosRecolectados = 0;
    [SerializeField] private int totalFragmentos = 3;
    [SerializeField] private int enemigosNecesarios = 10;

    private bool puertaAbierta = false;
    private bool bossActivado = false;
    private bool nivelCompletado = false;

    // ============================================================
    // ========================= START =============================
    // ============================================================

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ActualizarUI();

        // Boss desactivado al inicio
        if (boss != null)
            boss.SetActive(false);

        // Portal de salida desactivado
        if (portalSalida != null)
            portalSalida.gameObject.SetActive(false);

        Debug.Log($"🎮 Nivel iniciado. Objetivo: Recoger {totalFragmentos} fragmentos");
    }

    void Update()
    {
      
        if (puertaAbierta && !bossActivado && GameManager.instance != null)
        {
            if (GameManager.instance.enemigosEliminados >= enemigosNecesarios)
            {
                ActivarBoss();
            }
        }
        

        ActualizarUI();
    }

    // ============================================================
    // =================== SISTEMA DE FRAGMENTOS ===================
    // ============================================================

    public void RegistrarFragmento()
    {
        fragmentosRecolectados++;
        ActualizarUI();

        Debug.Log($"📦 Fragmento {fragmentosRecolectados}/{totalFragmentos} recolectado");

        if (fragmentosRecolectados >= totalFragmentos)
        {
            Debug.Log("✅ Todos los fragmentos recogidos. Dirígete a la puerta.");
        }
    }

    // ============================================================
    // ====================== SISTEMA DE PUERTA ====================
    // ============================================================

    public void PuertaSeAbrio()
    {
        Debug.Log("🔥 PuertaSeAbrio() LLAMADO");
        puertaAbierta = true;
        Debug.Log($"🚪 Puerta abierta. ¡Activando boss!");

        ActivarBoss();
    }

    // ============================================================
    // ======================== BOSS ===============================
    // ============================================================

    private void ActivarBoss()
    {
        Debug.Log("🔥 ActivarBoss() LLAMADO");
        bossActivado = true;

        if (boss != null)
        {
            Debug.Log("✅ Boss encontrado");

            if (bossSpawnPoint != null)
            {
                boss.transform.position = bossSpawnPoint.position;
                boss.transform.rotation = bossSpawnPoint.rotation;
                Debug.Log("✅ Boss posicionado");
            }

            boss.SetActive(true);
            Debug.Log("✅ Boss activado (SetActive)");

            BossVida bossVida = boss.GetComponent<BossVida>();
            if (bossVida != null)
            {
                Debug.Log("✅ BossVida encontrado, inicializando...");
                bossVida.InicializarVida();
            }
            else
            {
                Debug.LogError("❌ NO SE ENCONTRÓ BossVida en el boss!");
            }

            Debug.Log("👹 ¡BOSS ACTIVADO!");
        }
        else
        {
            Debug.LogError("❌ boss es NULL!");
        }
    }

    public void BossFueDerrotado()
    {
        Debug.Log("💀 Boss derrotado!");

        // Activar portal de salida
        if (portalSalida != null)
        {
            portalSalida.gameObject.SetActive(true);
            Debug.Log("🌀 Portal de salida activado");
        }

        nivelCompletado = true;
    }

    // ============================================================
    // ===================== FINAL DE ESCENA ========================
    // ============================================================

    public void CargarSiguienteNivel(string nombreEscena)
    {
        Debug.Log($"📂 Cargando siguiente nivel: {nombreEscena}");
        SceneManager.LoadScene(nombreEscena);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ============================================================
    // ========================= UI UPDATE =========================
    // ============================================================

    private void ActualizarUI()
    {
        if (textoFragmentos != null)
            textoFragmentos.text = $"Fragmentos: {fragmentosRecolectados}/{totalFragmentos}";

        if (textoEnemigos != null && GameManager.instance != null)
            textoEnemigos.text = GameManager.instance.enemigosEliminados.ToString();
    }

    // ============================================================
    // ===================== MÉTODOS AUXILIARES ====================
    // ============================================================

    public int ObtenerFragmentosRecolectados()
    {
        return fragmentosRecolectados;
    }

    public bool NivelEstaCompletado()
    {
        return nivelCompletado;
    }
}