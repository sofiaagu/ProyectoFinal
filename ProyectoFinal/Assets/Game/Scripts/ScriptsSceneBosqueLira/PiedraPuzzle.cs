using UnityEngine;

public class PiedraPuzzle : MonoBehaviour
{
    public int idPiedra; // 1, 2, 3...
    private SantuarioAgua santuario;
    private bool activada = false; // evita doble activación local

    void Start()
    {
        santuario = FindObjectOfType<SantuarioAgua>();
    }

    void OnMouseDown()
    {
        if (activada) return; // ya activada localmente, no hacer más

        if (santuario == null) return;

        // Le decimos al santuario quién hizo click (this)
        bool fueCorrecto = santuario.ClickEnPiedra(idPiedra, this);

        if (fueCorrecto)
        {
            // Marcamos como activada y la ocultamos (desaparece)
            activada = true;
            gameObject.SetActive(false);
        }
        else
        {
            // Si fue incorrecto, no hacemos nada (el santuario se encargará de restaurar)
            // Aquí podrías poner sonido o animación de error.
        }
    }

    // Método que el Santuario puede llamar para reactivar la piedra si el puzzle se reinicia
    public void Reactivar()
    {
        activada = false;
        gameObject.SetActive(true);
    }
}
