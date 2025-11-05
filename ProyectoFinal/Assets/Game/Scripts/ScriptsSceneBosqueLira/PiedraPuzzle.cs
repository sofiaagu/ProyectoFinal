using UnityEngine;

public class PiedraPuzzle : MonoBehaviour
{
    public int idPiedra; // Asigna 1, 2 o 3 en el Inspector
    private SantuarioAgua santuario;

    void Start()
    {
        santuario = FindObjectOfType<SantuarioAgua>();
    }

    void OnMouseDown()
    {
        santuario.ClickEnPiedra(idPiedra);
    }
}
