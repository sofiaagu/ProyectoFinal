using UnityEngine;
using System;
using System.Collections;

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
    public Color colorPista = Color.yellow;

    [Header("Configuración")]
    [Tooltip("¿Es un cristal de pista o interactuable?")]
    public bool esCristalPista = false;

    public Action OnCristalClickado;

    private bool activado = false;
    private Material materialCristal;
    private Vector3 posicionInicial;
    private Coroutine coroutinePista;

    void Start()
    {
        materialCristal = GetComponent<Renderer>().material;
        posicionInicial = transform.position;

        // Configurar estado inicial
        if (materialCristal != null)
        {
            materialCristal.EnableKeyword("_EMISSION");
            materialCristal.SetColor("_EmissionColor", colorInactivo);
        }

        if (puenteAsociado != null && !esCristalPista)
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

        // Flotación suave
        float offset = Mathf.Sin(Time.time * 2f) * 0.2f;
        transform.position = new Vector3(
            posicionInicial.x,
            posicionInicial.y + offset,
            posicionInicial.z
        );

        // Detectar click del mouse (solo para cristales interactuables)
        if (!esCristalPista && Input.GetMouseButtonDown(0))
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
                        // Invocar evento para que el manager lo procese
                        OnCristalClickado?.Invoke();
                    }
                    else
                    {
                        Debug.Log("Necesitas la habilidad de Luminiscencia");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Muestra este cristal como pista temporal
    /// </summary>
    public void MostrarPista(float duracion)
    {
        if (coroutinePista != null)
            StopCoroutine(coroutinePista);

        coroutinePista = StartCoroutine(MostrarPistaCoroutine(duracion));
    }

    IEnumerator MostrarPistaCoroutine(float duracion)
    {
        // Encender con color de pista
        if (materialCristal != null)
        {
            materialCristal.SetColor("_EmissionColor", colorPista * 3f);
        }

        if (luzCristal != null)
        {
            luzCristal.color = colorPista;
            luzCristal.intensity = 8f;
        }

        // Efecto de partículas
        if (efectoActivacion != null)
        {
            Instantiate(efectoActivacion, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(duracion);

        // Volver al estado inactivo
        if (!activado)
        {
            if (materialCristal != null)
            {
                materialCristal.SetColor("_EmissionColor", colorInactivo);
            }

            if (luzCristal != null)
            {
                luzCristal.color = colorInactivo;
                luzCristal.intensity = 1f;
            }
        }
    }

    /// <summary>
    /// Activa el cristal permanentemente (cuando el jugador lo hace bien)
    /// </summary>
    public void ActivarPermanente()
    {
        if (activado) return;

        activado = true;

        // Cambiar color del cristal
        if (materialCristal != null)
        {
            materialCristal.SetColor("_EmissionColor", colorActivo * 2f);
        }

        // Activar puente individual si existe
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

            // Animar la línea
            StartCoroutine(AnimarLineaEnergia());
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

        Debug.Log($"Cristal {gameObject.name} activado!");
    }

    /// <summary>
    /// Desactiva el cristal (para reiniciar la secuencia)
    /// </summary>
    public void Desactivar()
    {
        activado = false;

        // Restaurar color inactivo
        if (materialCristal != null)
        {
            materialCristal.SetColor("_EmissionColor", colorInactivo);
        }

        // Desactivar puente
        if (puenteAsociado != null)
        {
            puenteAsociado.SetActive(false);
        }

        // Desactivar línea
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        // Restaurar luz
        if (luzCristal != null)
        {
            luzCristal.color = colorInactivo;
            luzCristal.intensity = 1f;
        }
    }

    /// <summary>
    /// Hace pulsar la luz del cristal (efecto de completado)
    /// </summary>
    public void PulsarLuz()
    {
        StartCoroutine(PulsarLuzCoroutine());
    }

    IEnumerator PulsarLuzCoroutine()
    {
        if (luzCristal != null)
        {
            float intensidadOriginal = luzCristal.intensity;
            luzCristal.intensity = 10f;
            yield return new WaitForSeconds(0.2f);
            luzCristal.intensity = intensidadOriginal;
        }
    }

    IEnumerator AnimarLineaEnergia()
    {
        if (lineRenderer == null) yield break;

        float tiempo = 0f;
        float duracion = 0.5f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            // Animar el ancho de la línea
            lineRenderer.startWidth = Mathf.Lerp(0f, 0.1f, progreso);
            lineRenderer.endWidth = Mathf.Lerp(0f, 0.1f, progreso);

            yield return null;
        }
    }

    // Método legacy para compatibilidad con código anterior
    public void Activar()
    {
        ActivarPermanente();
    }
}