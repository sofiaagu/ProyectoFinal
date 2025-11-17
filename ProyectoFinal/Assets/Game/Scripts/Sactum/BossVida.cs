using UnityEngine;

public class BossVida : MonoBehaviour
{
    private float vidaMaxima;
    private float vidaActual;
    private bool inicializado = false;

    // Este método será llamado por el SceneController
    public void InicializarVida()
    {
        if (inicializado) return; // Solo inicializar una vez

        Debug.Log("🔥 InicializarVida llamado");

        int fragmentos = 0;

        if (SceneController.instance != null)
        {
            fragmentos = SceneController.instance.ObtenerFragmentosRecolectados();
            Debug.Log($"🔥 SceneController encontrado. Fragmentos: {fragmentos}");
        }
        else
        {
            Debug.LogError("🔥 SceneController.instance es NULL!");
        }

        if (fragmentos == 1)
        {
            vidaMaxima = 100f;
            Debug.Log("🔥 1 fragmento detectado → 100 HP");
        }
        else if (fragmentos == 2)
        {
            vidaMaxima = 80f;
            Debug.Log("🔥 2 fragmentos detectados → 80 HP");
        }
        else if (fragmentos >= 3)
        {
            vidaMaxima = 50f;
            Debug.Log("🔥 3+ fragmentos detectados → 50 HP");
        }
        else
        {
            vidaMaxima = 100f;
            Debug.Log($"🔥 {fragmentos} fragmentos detectados → 100 HP por defecto");
        }

        vidaActual = vidaMaxima;
        inicializado = true;

        Debug.Log($"👹 BOSS INICIALIZADO: {vidaActual}/{vidaMaxima} HP con {fragmentos} fragmentos");
    }

    public void RecibirDaño(float daño)
    {
        if (!inicializado)
        {
            Debug.LogWarning("⚠️ Boss no está inicializado, no puede recibir daño");
            return;
        }

        vidaActual -= daño;
        Debug.Log($"💥 Boss recibió {daño} de daño. Vida restante: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("💀 Boss derrotado!");

        if (SceneController.instance != null)
        {
            SceneController.instance.BossFueDerrotado();
        }

        Destroy(gameObject);
    }

    public float ObtenerVidaActual()
    {
        return vidaActual;
    }

    public float ObtenerVidaMaxima()
    {
        return vidaMaxima;
    }
}