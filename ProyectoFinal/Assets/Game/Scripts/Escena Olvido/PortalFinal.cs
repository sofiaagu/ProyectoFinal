using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalFinal : MonoBehaviour
{
    [Header("Escena a cargar")]
    public string escenaDestino = "NombreDeLaEscena";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚪 Entraste al portal final.");

            // Registrar tiempo REAL de la escena
            ControllerSceneLira lira = FindAnyObjectByType<ControllerSceneLira>();
            if (lira != null)
                lira.CompletarEscena();

            // Cargar la siguiente escena
            SceneManager.LoadScene(escenaDestino);
        }
    }
}
