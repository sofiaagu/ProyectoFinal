using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderScene : MonoBehaviour
{
    // 🔹 Carga una escena por nombre
    public void LoaderScenes(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }
}