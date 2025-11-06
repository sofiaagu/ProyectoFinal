using UnityEngine;

public class CristalNode : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject puenteAsociado;
    public LineRenderer lineRenderer;
    public Transform puntoDestino;

    [Header("Efectos")]
    public GameObject efectoActivacion;
    public Light luzCristal;

    [Header("Colores")]
    public Color colorInactivo = Color.gray;
    public Color colorActivo = Color.cyan;

    private bool activado = false;
    private Material materialCristal;

    void Start()
    {
        materialCristal = GetComponent<Renderer>().material;

        // Configurar estado inicial
        if (materialCristal != null)
        {
            materialCristal.EnableKeyword("_EMISSION");
            materialCristal.SetColor("_EmissionColor", colorInactivo);
        }

        if (puenteAsociado != null)
            puenteAsociado.SetActive(false);

        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (luzCristal != null)
        {
            luzCristal.color = colorInactivo;
            luzCristal.intensity = 1f;
        }
    }

    void Update()
    {
        // Rotación del cristal
        transform.Rotate(Vector3.up, 30f * Time.deltaTime);

        // Flotación
        float offset = Mathf.Sin(Time.time * 2f) * 0.2f;
        transform.position = new Vector3(transform.position.x, transform.position.y + offset * Time.deltaTime, transform.position.z);

        // Detectar click del mouse
        if (Input.GetMouseButtonDown(0))
        {
            DetectarClick();
        }
    }

    void DetectarClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                // Verificar si el jugador tiene la habilidad
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();
                    if (abilities != null && abilities.tieneLuminiscencia)
                    {
                        Activar();
                    }
                    else
                    {
                        Debug.Log("Necesitas la habilidad de Luminiscencia");
                    }
                }
            }
        }
    }

    public void Activar()
    {
        if (activado) return;

        activado = true;

        // Cambiar color del cristal
        if (materialCristal != null)
        {
            materialCristal.SetColor("_EmissionColor", colorActivo * 2f);
        }

        // Activar puente
        if (puenteAsociado != null)
        {
            puenteAsociado.SetActive(true);
        }

        // Activar línea de energía
        if (lineRenderer != null && puntoDestino != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, puntoDestino.position);
            lineRenderer.startColor = colorActivo;
            lineRenderer.endColor = colorActivo;
        }

        // Efecto de partículas
        if (efectoActivacion != null)
        {
            Instantiate(efectoActivacion, transform.position, Quaternion.identity);
        }

        // Cambiar luz
        if (luzCristal != null)
        {
            luzCristal.color = colorActivo;
            luzCristal.intensity = 5f;
        }

        Debug.Log("Cristal activado - Puente formado!");
    }
}