using UnityEngine;

public class BossVida : MonoBehaviour
{
    // Vida máxima y actual del boss
    private float vidaMaxima;
    private float vidaActual;

    // Para asegurarnos que solo se inicialice una vez
    private bool inicializado = false;

    // Este método será llamado por el SceneController
    public void InicializarVida()
    {
        // Evitar inicialización repetida
        if (inicializado) return; 

        Debug.Log("InicializarVida llamado");

        int fragmentos = 0;

        // Obtener cantidad de fragmentos recolectados desde SceneController
        if (SceneController.instance != null)
        {
            fragmentos = SceneController.instance.ObtenerFragmentosRecolectados();
            Debug.Log($"SceneController encontrado. Fragmentos: {fragmentos}");
        }
        else
        {
            Debug.LogError(" SceneController.instance es NULL!");
        }

        // Ajustar vida máxima según cantidad de fragmentos
        if (fragmentos == 1)
        {
            vidaMaxima = 100f;
            Debug.Log(" 1 fragmento detectado → 100 HP");
        }
        else if (fragmentos == 2)
        {
            vidaMaxima = 80f;
            Debug.Log(" 2 fragmentos detectados → 80 HP");
        }
        else if (fragmentos >= 3)
        {
            vidaMaxima = 50f;
            Debug.Log(" 3 fragmentos detectados → 50 HP");
        }
        else
        {
            vidaMaxima = 100f; // Valor por defecto si no hay fragmentos
            Debug.Log($" {fragmentos} fragmentos detectados → 100 HP por defecto");
        }

        // Inicializar vida actual
        vidaActual = vidaMaxima;
        inicializado = true;

        Debug.Log($" BOSS INICIALIZADO: {vidaActual}/{vidaMaxima} HP con {fragmentos} fragmentos");
    }

    public void RecibirDaño(float daño)
    {
        // No se puede recibir daño si no está inicializado
        if (!inicializado)
        {
            Debug.LogWarning(" Boss no está inicializado, no puede recibir daño");
            return;
        }

        // Reducir vida actual
        vidaActual -= daño;
        Debug.Log($"Boss recibió {daño} de daño. Vida restante: {vidaActual}/{vidaMaxima}");

        // Si la vida llega a 0 o menos, muere
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log(" Boss derrotado!");

        // Notificar al SceneController que el boss fue derrotado
        if (SceneController.instance != null)
        {
            SceneController.instance.BossFueDerrotado();
        }

        // Destruir objeto del boss
        Destroy(gameObject);
    }

    // Métodos de acceso para vida actual y máxima
    public float ObtenerVidaActual()
    {
        return vidaActual;
    }

    public float ObtenerVidaMaxima()
    {
        return vidaMaxima;
    }
}