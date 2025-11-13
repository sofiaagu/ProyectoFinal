using UnityEngine;

public class Tumba : MonoBehaviour
{
    public int id; // índice de la tumba
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
