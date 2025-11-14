using UnityEngine;
using System;
using System.Collections;

public class CristalNode : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject puenteAsociado;
    public LineRenderer lineRenderer;
    public Transform puntoInicioLinea;
    public Transform puntoDestino;

    [Header("Efectos")]
    public GameObject efectoActivacion;
    public Light luzCristal;

    [Header("Colores")]
    public Color colorInactivo = new Color(0.3f, 0.3f, 0.3f);
    public Color colorActivo = Color.cyan;
    public Color colorPista = Color.yellow;

    [Header("Configuración")]
    [Tooltip("¿Es un cristal de pista o interactuable?")]
    public bool esCristalPista = false;
    public float rangoActivacion = 3f;

    [Header("Mensaje sin Habilidad")]
    [TextArea(2, 4)]
    public string mensajeSinHabilidad = "Necesitas la habilidad de Luminiscencia para activar este cristal.";
    public TMPro.TextMeshProUGUI textoMensaje;
    public float tiempoMensaje = 3f;

    private bool activado = false;
    private Material materialCristal;
    private Vector3 posicionInicial;
    private Coroutine coroutinePista;

    public Action OnCristalClickado;

    void Start()
    {
        // Obtener material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            materialCristal = renderer.material;
            materialCristal.EnableKeyword("_EMISSION");
            materialCristal.SetColor("_EmissionColor", colorInactivo * 0.5f);
        }

        posicionInicial = transform.position;

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
        // Rotación suave
        transform.Rotate(Vector3.up, 30f * Time.deltaTime);

        // Flotación
        float offset = Mathf.Sin(Time.time * 2f) * 0.2f;
        transform.position = new Vector3(
            posicionInicial.x,
            posicionInicial.y + offset,
            posicionInicial.z
        );

        // Click solo en cristales interactuables
        if (!esCristalPista && Input.GetMouseButtonDown(0))
            DetectarClick();
    }

    void DetectarClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;

        if (hit.transform != transform && !hit.transform.IsChildOf(transform)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No hay jugador. Activando para testing.");
            OnCristalClickado?.Invoke();
            return;
        }

        float distancia = Vector3.Distance(transform.position, player.transform.position);
        if (distancia > rangoActivacion)
        {
            Debug.Log("Muy lejos para activar este cristal.");
            return;
        }

        PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();
        if (abilities != null && abilities.tieneLuminiscencia)
        {
            OnCristalClickado?.Invoke();
        }
        else
        {
            MostrarMensajeSinHabilidad();
        }
    }

    void MostrarMensajeSinHabilidad()
    {
        if (textoMensaje == null)
        {
            Debug.Log(mensajeSinHabilidad);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(MostrarMensajeCoroutine());
    }

    IEnumerator MostrarMensajeCoroutine()
    {
        textoMensaje.text = mensajeSinHabilidad;
        textoMensaje.gameObject.SetActive(true);

        yield return new WaitForSeconds(tiempoMensaje);

        textoMensaje.gameObject.SetActive(false);
    }

    // ======================================================================
    //  PISTA (CRISTAL BRILLA Y LUEGO VUELVE A NORMAL / ACTIVO)
    // ======================================================================
    public void MostrarPista(float duracion)
    {
        if (coroutinePista != null)
            StopCoroutine(coroutinePista);

        coroutinePista = StartCoroutine(MostrarPistaCoroutine(duracion));
    }

    IEnumerator MostrarPistaCoroutine(float duracion)
    {
        Color colorOriginalLuz = luzCristal != null ? luzCristal.color : Color.white;

        if (materialCristal != null)
            materialCristal.SetColor("_EmissionColor", colorPista * 3f);

        if (luzCristal != null)
        {
            luzCristal.color = colorPista;
            luzCristal.intensity = 8f;
        }

        if (efectoActivacion != null)
            Instantiate(efectoActivacion, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(duracion);

        if (!activado)
        {
            if (materialCristal != null)
                materialCristal.SetColor("_EmissionColor", colorInactivo * 0.5f);

            if (luzCristal != null)
            {
                luzCristal.color = colorOriginalLuz;
                luzCristal.intensity = 1f;
            }
        }
        else
        {
            if (materialCristal != null)
                materialCristal.SetColor("_EmissionColor", colorActivo * 2f);

            if (luzCristal != null)
            {
                luzCristal.color = colorActivo;
                luzCristal.intensity = 5f;
            }
        }
    }

    // ======================================================================
    //  ACTIVACIÓN PERMANENTE (USO REAL IN-GAME)
    // ======================================================================
    public void ActivarPermanente(Transform destinoLinea = null)
    {
        if (activado) return;
        activado = true;

        // Cristal cambia de color
        if (materialCristal != null)
            materialCristal.SetColor("_EmissionColor", colorActivo * 2f);

        // Activar puente
        if (puenteAsociado != null)
            puenteAsociado.SetActive(true);

        // Línea de energía
        Transform destino = destinoLinea != null ? destinoLinea : puntoDestino;

        if (lineRenderer != null && destino != null)
        {
            lineRenderer.enabled = true;

            Vector3 puntoInicio = puntoInicioLinea != null ?
                puntoInicioLinea.position :
                transform.position;

            lineRenderer.SetPosition(0, puntoInicio);
            lineRenderer.SetPosition(1, destino.position);

            lineRenderer.startColor = colorActivo;
            lineRenderer.endColor = colorActivo;

            StartCoroutine(AnimarLineaEnergia());
        }

        if (efectoActivacion != null)
            Instantiate(efectoActivacion, transform.position, Quaternion.identity);

        // Luz activa
        if (luzCristal != null)
        {
            luzCristal.color = colorActivo;
            luzCristal.intensity = 5f;
        }
    }

    // ======================================================================
    //  DESACTIVAR
    // ======================================================================
    public void Desactivar()
    {
        activado = false;

        // Color inactivo
        if (materialCristal != null)
            materialCristal.SetColor("_EmissionColor", colorInactivo * 0.5f);

        // Puente off
        if (puenteAsociado != null)
            puenteAsociado.SetActive(false);

        // Línea off
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        // Luz a modo inactivo
        if (luzCristal != null)
        {
            luzCristal.color = colorInactivo;
            luzCristal.intensity = 1f;
        }
    }

    // ======================================================================
    //  PULSO DE LUZ (EFECTO COMPLETADO / FEEDBACK)
    // ======================================================================
    public void PulsarLuz()
    {
        StartCoroutine(PulsarLuzCoroutine());
    }

    IEnumerator PulsarLuzCoroutine()
    {
        if (luzCristal == null) yield break;

        float intensidadOriginal = luzCristal.intensity;
        float intensidadMax = intensidadOriginal * 2.5f;

        float t = 0f;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            luzCristal.intensity = Mathf.Lerp(intensidadOriginal, intensidadMax, t / 0.15f);
            yield return null;
        }

        t = 0f;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            luzCristal.intensity = Mathf.Lerp(intensidadMax, intensidadOriginal, t / 0.15f);
            yield return null;
        }

        luzCristal.intensity = intensidadOriginal;
    }

    // ======================================================================
    //  ANIMACIÓN DE RAYO (LINE RENDERER)
    // ======================================================================
    IEnumerator AnimarLineaEnergia()
    {
        if (lineRenderer == null) yield break;

        float tiempo = 0f;
        float duracion = 1f;

        lineRenderer.startColor = colorActivo;
        lineRenderer.endColor = colorActivo;

        Vector3 inicio = puntoInicioLinea != null ? puntoInicioLinea.position : transform.position;
        Vector3 fin = puntoDestino != null ? puntoDestino.position : transform.position;

        lineRenderer.SetPosition(0, inicio);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            Vector3 posActual = Vector3.Lerp(inicio, fin, t);
            lineRenderer.SetPosition(1, posActual);

            float w = Mathf.Lerp(0f, 0.12f, t);
            lineRenderer.startWidth = w;
            lineRenderer.endWidth = w;

            yield return null;
        }

        lineRenderer.SetPosition(1, fin);
    }

    // Legacy
    public void Activar() => ActivarPermanente();

    void OnDrawGizmosSelected()
    {
        if (!esCristalPista)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangoActivacion);
        }
    }
}
