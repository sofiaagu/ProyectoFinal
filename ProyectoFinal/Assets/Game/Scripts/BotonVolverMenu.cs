using UnityEngine;

public class BotonVolverMenu : MonoBehaviour
{
    public void VolverAlMenuClick()
    {
        if (GameManager.instance != null)
            GameManager.instance.VolverAlMenuYResetear();
    }
    
    public void VolverALaEscena()
    {
        if (GameManager.instance != null)
            GameManager.instance.ReiniciarJuego();
    }
    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
