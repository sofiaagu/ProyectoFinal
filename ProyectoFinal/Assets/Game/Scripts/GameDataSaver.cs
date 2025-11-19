using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PartidaData
{
    public string nombreJugador;
    public int numeroPartida;
    public int score;
    public int enemigosEliminados;
    public string tiempo;
    public int vidasRestantes;
    public string fecha;
    public string hora;
}

[System.Serializable]
public class HistorialPartidas
{
    public List<PartidaData> partidas = new List<PartidaData>();
}

public class GameDataSaver : MonoBehaviour
{
    public static GameDataSaver instance; // Singleton

    [Header("Referencias")]
    public Timer timerScript;
    public TextMeshProUGUI textoConfirmacion; // Texto para mostrar "¡Guardado!"

    private string rutaArchivo;
    private HistorialPartidas historial;

    void Awake()
    {
        // Singleton: Solo una instancia
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Ruta donde se guardará el JSON (en la carpeta de la aplicación)
        rutaArchivo = Path.Combine(Application.persistentDataPath, "HistorialPartidas.json");
        Debug.Log("📁 Ruta del archivo: " + rutaArchivo);

        // Cargar historial existente
        CargarHistorial();

        // Buscar Timer automáticamente si no está asignado
        if (timerScript == null)
        {
            timerScript = FindObjectOfType<Timer>();
        }
    }

    void CargarHistorial()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            historial = JsonUtility.FromJson<HistorialPartidas>(json);
            Debug.Log($"📂 Historial cargado: {historial.partidas.Count} partidas guardadas");
        }
        else
        {
            historial = new HistorialPartidas();
            Debug.Log("📝 Creando nuevo historial");
        }
    }

    public void GuardarPartida()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("❌ No se encontró GameManager");
            MostrarMensaje("Error: No hay GameManager", Color.red);
            return;
        }

        // Crear datos de la partida actual
        PartidaData nuevaPartida = new PartidaData();

        // Número de partida (incrementar según las anteriores)
        nuevaPartida.numeroPartida = historial.partidas.Count + 1;
        nuevaPartida.nombreJugador = "Jugador" + nuevaPartida.numeroPartida;

        // Score y enemigos
        nuevaPartida.score = GameManager.instance.score;
        nuevaPartida.enemigosEliminados = GameManager.instance.enemigosEliminados;

        // Vidas restantes
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            PlayerHealth playerHealth = jugador.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                nuevaPartida.vidasRestantes = playerHealth.CurrentLives;
            }
            else
            {
                nuevaPartida.vidasRestantes = 0;
                Debug.LogWarning("⚠️ No se encontró PlayerHealth");
            }
        }
        else
        {
            nuevaPartida.vidasRestantes = 0;
            Debug.LogWarning("⚠️ No se encontró el jugador");
        }

        // Tiempo
        if (timerScript != null)
        {
            // Asegurarse de que el timer esté detenido
            timerScript.TimerStop();

            // Usar el StopTime que ya tiene guardado
            float tiempoFinal = timerScript.StopTime;

            int minutos = Mathf.FloorToInt(tiempoFinal / 60f);
            int segundos = Mathf.FloorToInt(tiempoFinal % 60f);
            int centesimas = Mathf.FloorToInt((tiempoFinal - (segundos + minutos * 60)) * 100f);

            nuevaPartida.tiempo = string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, centesimas);

            Debug.Log($"⏱️ Tiempo guardado: {nuevaPartida.tiempo} (desde StopTime: {tiempoFinal})");
        }
        else
        {
            nuevaPartida.tiempo = "00:00:00";
            Debug.LogWarning("⚠️ No se encontró Timer");
        }

        // Fecha y hora actual
        DateTime ahora = DateTime.Now;
        nuevaPartida.fecha = ahora.ToString("dd/MM/yyyy");
        nuevaPartida.hora = ahora.ToString("HH:mm:ss");

        // Añadir al historial
        historial.partidas.Add(nuevaPartida);

        // Guardar en archivo JSON
        try
        {
            string json = JsonUtility.ToJson(historial, true); // true = formato legible
            File.WriteAllText(rutaArchivo, json);

            Debug.Log("✅ Partida guardada exitosamente");
            Debug.Log($"📊 Datos guardados - Score: {nuevaPartida.score}, Enemigos: {nuevaPartida.enemigosEliminados}, Tiempo: {nuevaPartida.tiempo}, Vidas: {nuevaPartida.vidasRestantes}");

            MostrarMensaje("¡Partida guardada exitosamente!", Color.green);

            // Abrir carpeta donde se guardó (opcional, solo en Windows)
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", "/select," + rutaArchivo.Replace('/', '\\'));
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error al guardar: " + e.Message);
            MostrarMensaje("Error al guardar", Color.red);
        }
    }

    void MostrarMensaje(string mensaje, Color color)
    {
        if (textoConfirmacion != null)
        {
            textoConfirmacion.text = mensaje;
            textoConfirmacion.color = color;
            textoConfirmacion.gameObject.SetActive(true);

            // Ocultar después de 3 segundos
            StartCoroutine(OcultarMensajeDespuesDe(3f));
        }
    }

    System.Collections.IEnumerator OcultarMensajeDespuesDe(float segundos)
    {
        yield return new WaitForSecondsRealtime(segundos); // Usar realtime por si el juego está pausado
        if (textoConfirmacion != null)
        {
            textoConfirmacion.gameObject.SetActive(false);
        }
    }

    // Método para leer y mostrar el historial en consola (útil para debug)
    public void MostrarHistorialEnConsola()
    {
        if (historial.partidas.Count == 0)
        {
            Debug.Log("📋 No hay partidas guardadas");
            return;
        }

        Debug.Log("═══════════════════════════════════════");
        Debug.Log($"📋 HISTORIAL DE PARTIDAS ({historial.partidas.Count} partidas)");
        Debug.Log("═══════════════════════════════════════");

        foreach (PartidaData partida in historial.partidas)
        {
            Debug.Log($"\n🎮 {partida.nombreJugador}");
            Debug.Log($"   📅 Fecha: {partida.fecha} - {partida.hora}");
            Debug.Log($"   💰 Score: {partida.score}");
            Debug.Log($"   ⚔️ Enemigos: {partida.enemigosEliminados}");
            Debug.Log($"   ⏱️ Tiempo: {partida.tiempo}");
            Debug.Log($"   ❤️ Vidas: {partida.vidasRestantes}");
        }

        Debug.Log("═══════════════════════════════════════");
    }
}