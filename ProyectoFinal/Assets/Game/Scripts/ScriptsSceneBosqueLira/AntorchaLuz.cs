using UnityEngine;

public class AntorchaLuz : MonoBehaviour
{
    [Header("Luz cuando se enciende")]
    public GameObject luzAntorcha; // Point Light

    [Header("Manager del Santuario de Luz")]
    public SantuarioLuz manager;

    bool encendida = false;

    void Start()
    {
        if (luzAntorcha != null)
            luzAntorcha.SetActive(false);
    }

    void OnMouseDown()
    {
        if (!encendida)
            EncenderAntorcha();
    }

    void EncenderAntorcha()
    {
        encendida = true;

        if (luzAntorcha != null)
            luzAntorcha.SetActive(true);

        manager.RegistrarAntorchaEncendida();
    }
}
