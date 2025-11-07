using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    public bool semillaAguaRecogida;
    public bool semillaTierraRecogida;
    public bool semillaLuzRecogida;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Marcar semilla recogida
    public void MarcarSemillaRecogida(string tipo)
    {
        switch (tipo)
        {
            case "Agua":
                semillaAguaRecogida = true;
                break;
            case "Tierra":
                semillaTierraRecogida = true;
                break;
            case "Luz":
                semillaLuzRecogida = true;
                break;
        }

        Debug.Log($"🌱 Semilla {tipo} recogida");
    }

    // Comprobar si tiene todas
    public bool TieneTodasLasSemillas()
    {
        return semillaAguaRecogida && semillaTierraRecogida && semillaLuzRecogida;
    }

    // Cargar siguiente escena (según santuario actual)
    public void IrASiguientePlano(string planoActual)
    {
        string siguiente = "";

        switch (planoActual)
        {
            case "SantuarioAgua":
                siguiente = "SantuarioTierra";
                break;
            case "SantuarioTierra":
                siguiente = "SantuarioLuz";
                break;
            case "SantuarioLuz":
                siguiente = "ArbolCentral";
                break;
            default:
                Debug.Log("🌳 Has llegado al plano final.");
                return;
        }

        Debug.Log($"➡️ Cargando siguiente plano: {siguiente}");
        SceneManager.LoadScene(siguiente);
    }
}
