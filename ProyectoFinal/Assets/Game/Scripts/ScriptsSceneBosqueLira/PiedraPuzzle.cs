using UnityEngine;

public class PiedraPuzzle : MonoBehaviour
{
    public int id;
    private SantuarioAgua santuario;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private bool activa = true;

    void Start()
    {
        santuario = FindObjectOfType<SantuarioAgua>();
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    void OnMouseDown()
    {
        if (activa && santuario != null)
        {
            santuario.ClickEnPiedra(id, this);
        }
    }

    public void Desactivar()
    {
        activa = false;
        gameObject.SetActive(false); // 🔹 La piedra desaparece
    }

    public void Reactivar()
    {
        activa = true;
        gameObject.SetActive(true); // 🔹 Reaparece al reiniciar
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
    }
}
