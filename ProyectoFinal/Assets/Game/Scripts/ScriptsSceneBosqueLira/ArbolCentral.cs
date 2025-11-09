using TMPro;
using UnityEngine;

public class ArbolCentral : MonoBehaviour
{
    public TextMeshProUGUI mensajeUI;
    public ParticleSystem efectoRestauracion;

    private bool restaurado = false;

    void Update()
    {
        if (!restaurado && GameFlowManager.Instance != null && GameFlowManager.Instance.TieneTodasLasSemillas())
        {
            restaurado = true;
            RestaurarBosque();
        }
    }

    void RestaurarBosque()
    {
        mensajeUI.text = "🌳 ¡Has restaurado el Bosque de Lira!";
        if (efectoRestauracion != null)
            efectoRestauracion.Play();
        Debug.Log("✨ Bosque restaurado con las 3 semillas");
    }
}
