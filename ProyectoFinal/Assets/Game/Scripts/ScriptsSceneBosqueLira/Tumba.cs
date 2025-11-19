using UnityEngine;

public class Tumba : MonoBehaviour
{
    public int id; 
    private SantuarioTierra santuario;

    void Start()
    {
        santuario = FindObjectOfType<SantuarioTierra>();
    }

    void OnMouseDown()
    {
        santuario?.ClickEnTumba(id);
    }
}
