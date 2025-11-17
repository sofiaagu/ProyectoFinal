using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gestiona todas las funcionalidades del menú principal del juego.
/// Maneja navegación entre escenas, paneles de UI y configuraciones.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    [Tooltip("Panel principal del menú")]
    public GameObject menuPrincipal;

    [Tooltip("Panel de opciones/configuración")]
    public GameObject panelInstrucciones;
    public GameObject panelHistoria;

    [Tooltip("Panel de créditos")]
    public GameObject panelCreditos;

    [Tooltip("Panel de selección de niveles")]
    public GameObject panelSeleccionNivel;

    [Header("Configuración de Audio")]
    [Tooltip("Slider para control de volumen general")]
    public Slider sliderVolumen;

    [Tooltip("Slider para música")]
    public Slider sliderMusica;

    [Tooltip("Slider para efectos de sonido")]
    public Slider sliderSFX;

    [Header("Nombres de Escenas")]
    [Tooltip("Nombre de la siguiente escena a cargar")]
    public string nombreSiguienteEscena = "Nivel1";

    [Tooltip("Arreglo con nombres de las 4 escenas de niveles")]
    public string[] nombresNiveles = new string[4]
    {
        "Nivel1_Ascension",
        "Nivel2_Razon",
        "Nivel3_Luminiscencia",
        "Nivel4_Equilibrio"
    };

    private void Start()
    {
        // Asegurarse de que solo el menú principal esté visible al inicio
        MostrarMenuPrincipal();

        // Cargar configuraciones guardadas
        CargarConfiguraciones();

        // Desbloquear cursor en el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region Funciones de Navegación Principal

    /// <summary>
    /// Inicia el juego cargando la siguiente escena configurada
    /// </summary>
    public void IniciarJuego()
    {
        Debug.Log($"Cargando escena: {nombreSiguienteEscena}");

        // Guardar configuraciones antes de cambiar de escena
        GuardarConfiguraciones();

        // Cargar la siguiente escena
        SceneManager.LoadScene(nombreSiguienteEscena);
    }

    /// <summary>
    /// Cierra la aplicación (funciona en build, no en editor)
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        // Guardar antes de salir
        GuardarConfiguraciones();

#if UNITY_EDITOR
        // En el editor de Unity, detiene el modo de juego
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // En el build, cierra la aplicación
            Application.Quit();
#endif
    }

    #endregion

    #region Gestión de Paneles

    /// <summary>
    /// Muestra el panel de opciones y oculta los demás
    /// </summary>
    public void MostrarInstrucciones()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(true);
        if (panelCreditos != null) panelCreditos.SetActive(false);
        if (panelSeleccionNivel != null) panelSeleccionNivel.SetActive(false);
        if (panelHistoria != null) panelHistoria.SetActive(false);

        Debug.Log("Panel de opciones mostrado");
    }
    public void MostrarHistoria()
    {
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false);
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (panelHistoria != null) panelHistoria.SetActive(true);
        if (panelCreditos != null) panelCreditos.SetActive(false);
        if (panelSeleccionNivel != null) panelSeleccionNivel.SetActive(false);

        Debug.Log("Panel de Historia mostrado");
    }

    /// <summary>
    /// Muestra el panel de créditos y oculta los demás
    /// </summary>
    public void MostrarCreditos()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(true);
        if (panelSeleccionNivel != null) panelSeleccionNivel.SetActive(false);
        if(panelHistoria != null) panelHistoria.SetActive(false);

        Debug.Log("Panel de créditos mostrado");
    }

    /// <summary>
    /// Muestra el panel de selección de nivel
    /// </summary>
    public void MostrarSeleccionNivel()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);
        if (panelSeleccionNivel != null) panelSeleccionNivel.SetActive(true);
        if (panelHistoria != null) panelHistoria.SetActive(false);

        Debug.Log("Panel de selección de nivel mostrado");
    }

    /// <summary>
    /// Vuelve al menú principal desde cualquier otro panel
    /// </summary>
    public void MostrarMenuPrincipal()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);
        if (panelSeleccionNivel != null) panelSeleccionNivel.SetActive(false);
        if (panelHistoria != null) panelHistoria.SetActive(false);

        Debug.Log("Menú principal mostrado");
    }

    #endregion

    #region Carga de Niveles Específicos

    /// <summary>
    /// Carga un nivel específico según su índice (1-4)
    /// </summary>
    /// <param name="numeroNivel">Número del nivel a cargar (1-4)</param>
    public void CargarNivel(int numeroNivel)
    {
        // Validar que el número de nivel esté en rango
        if (numeroNivel < 1 || numeroNivel > 4)
        {
            Debug.LogError($"Número de nivel inválido: {numeroNivel}. Debe estar entre 1 y 4.");
            return;
        }

        // Validar que el arreglo de niveles tenga suficientes elementos
        if (nombresNiveles.Length < numeroNivel)
        {
            Debug.LogError($"El arreglo de niveles no contiene suficientes elementos. Se requiere al menos {numeroNivel} niveles.");
            return;
        }

        string nombreEscena = nombresNiveles[numeroNivel - 1];

        // Verificar si la escena existe en Build Settings
        if (ExisteEscenaEnBuild(nombreEscena))
        {
            Debug.Log($"Cargando Nivel {numeroNivel}: {nombreEscena}");
            GuardarConfiguraciones();
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"La escena '{nombreEscena}' no existe en Build Settings. Agrégala en File > Build Settings.");
        }
    }

    /// <summary>
    /// Sobrecarga para cargar nivel directamente por nombre
    /// </summary>
    /// <param name="nombreEscena">Nombre exacto de la escena</param>
    public void CargarNivelPorNombre(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError("El nombre de la escena está vacío.");
            return;
        }

        if (ExisteEscenaEnBuild(nombreEscena))
        {
            Debug.Log($"Cargando escena: {nombreEscena}");
            GuardarConfiguraciones();
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"La escena '{nombreEscena}' no existe en Build Settings.");
        }
    }

    #endregion

    #region Funciones de Audio/Opciones

    /// <summary>
    /// Cambia el volumen general del juego
    /// </summary>
    /// <param name="volumen">Valor entre 0 y 1</param>
    public void CambiarVolumenGeneral(float volumen)
    {
        AudioListener.volume = volumen;
        PlayerPrefs.SetFloat("VolumenGeneral", volumen);
        Debug.Log($"Volumen general ajustado a: {volumen}");
    }

    /// <summary>
    /// Cambia el volumen de la música
    /// </summary>
    /// <param name="volumen">Valor entre 0 y 1</param>
    public void CambiarVolumenMusica(float volumen)
    {
        // Aquí deberías implementar tu sistema de audio
        // Por ejemplo, si usas un AudioManager:
        // AudioManager.Instance.SetMusicVolume(volumen);

        PlayerPrefs.SetFloat("VolumenMusica", volumen);
        Debug.Log($"Volumen de música ajustado a: {volumen}");
    }

    /// <summary>
    /// Cambia el volumen de efectos de sonido
    /// </summary>
    /// <param name="volumen">Valor entre 0 y 1</param>
    public void CambiarVolumenSFX(float volumen)
    {
        // Implementar con tu sistema de audio
        // AudioManager.Instance.SetSFXVolume(volumen);

        PlayerPrefs.SetFloat("VolumenSFX", volumen);
        Debug.Log($"Volumen de SFX ajustado a: {volumen}");
    }

    /// <summary>
    /// Cambia el modo de pantalla completa
    /// </summary>
    /// <param name="pantallaCompleta">True para pantalla completa</param>
    public void CambiarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
        PlayerPrefs.SetInt("PantallaCompleta", pantallaCompleta ? 1 : 0);
        Debug.Log($"Pantalla completa: {pantallaCompleta}");
    }

    /// <summary>
    /// Cambia la calidad gráfica del juego
    /// </summary>
    /// <param name="nivelCalidad">Índice del nivel de calidad (0 = bajo, 5 = ultra)</param>
    public void CambiarCalidad(int nivelCalidad)
    {
        QualitySettings.SetQualityLevel(nivelCalidad);
        PlayerPrefs.SetInt("Calidad", nivelCalidad);
        Debug.Log($"Calidad gráfica ajustada a nivel: {nivelCalidad}");
    }

    #endregion

    #region Sistema de Guardado y Carga

    /// <summary>
    /// Guarda todas las configuraciones en PlayerPrefs
    /// </summary>
    private void GuardarConfiguraciones()
    {
        if (sliderVolumen != null)
            PlayerPrefs.SetFloat("VolumenGeneral", sliderVolumen.value);

        if (sliderMusica != null)
            PlayerPrefs.SetFloat("VolumenMusica", sliderMusica.value);

        if (sliderSFX != null)
            PlayerPrefs.SetFloat("VolumenSFX", sliderSFX.value);

        PlayerPrefs.Save();
        Debug.Log("Configuraciones guardadas");
    }

    /// <summary>
    /// Carga las configuraciones guardadas
    /// </summary>
    private void CargarConfiguraciones()
    {
        // Cargar volumen general
        if (PlayerPrefs.HasKey("VolumenGeneral"))
        {
            float volumen = PlayerPrefs.GetFloat("VolumenGeneral");
            AudioListener.volume = volumen;
            if (sliderVolumen != null)
                sliderVolumen.value = volumen;
        }

        // Cargar volumen de música
        if (PlayerPrefs.HasKey("VolumenMusica") && sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica");
        }

        // Cargar volumen de SFX
        if (PlayerPrefs.HasKey("VolumenSFX") && sliderSFX != null)
        {
            sliderSFX.value = PlayerPrefs.GetFloat("VolumenSFX");
        }

        // Cargar pantalla completa
        if (PlayerPrefs.HasKey("PantallaCompleta"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("PantallaCompleta") == 1;
        }

        // Cargar calidad gráfica
        if (PlayerPrefs.HasKey("Calidad"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Calidad"));
        }

        Debug.Log("Configuraciones cargadas");
    }

    /// <summary>
    /// Resetea todas las configuraciones a valores por defecto
    /// </summary>
    public void ResetearConfiguraciones()
    {
        PlayerPrefs.DeleteAll();
        AudioListener.volume = 1f;

        if (sliderVolumen != null) sliderVolumen.value = 1f;
        if (sliderMusica != null) sliderMusica.value = 0.8f;
        if (sliderSFX != null) sliderSFX.value = 1f;

        Screen.fullScreen = true;
        QualitySettings.SetQualityLevel(3); // Medium-High

        Debug.Log("Configuraciones reseteadas a valores por defecto");
    }

    #endregion

    #region Funciones Auxiliares

    /// <summary>
    /// Verifica si una escena existe en Build Settings
    /// </summary>
    /// <param name="nombreEscena">Nombre de la escena a verificar</param>
    /// <returns>True si la escena existe</returns>
    private bool ExisteEscenaEnBuild(string nombreEscena)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string rutaEscena = SceneUtility.GetScenePathByBuildIndex(i);
            string nombre = System.IO.Path.GetFileNameWithoutExtension(rutaEscena);

            if (nombre == nombreEscena)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Recarga la escena actual
    /// </summary>
    public void RecargarEscenaActual()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        Debug.Log($"Recargando escena: {escenaActual.name}");
        SceneManager.LoadScene(escenaActual.name);
    }

    #endregion

    #region Funciones de Debug (Opcional)

    /// <summary>
    /// Muestra información de depuración en consola
    /// </summary>
    public void MostrarInfoDebug()
    {
        Debug.Log("===== INFORMACIÓN DEL MENÚ =====");
        Debug.Log($"Escena actual: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Siguiente escena: {nombreSiguienteEscena}");
        Debug.Log($"Niveles configurados: {nombresNiveles.Length}");
        Debug.Log($"Volumen general: {AudioListener.volume}");
        Debug.Log($"Pantalla completa: {Screen.fullScreen}");
        Debug.Log($"Calidad gráfica: {QualitySettings.GetQualityLevel()}");
        Debug.Log("================================");
    }

    #endregion
}