using UnityEngine;
using System;
using System.Collections;

public class CristalNode : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject puenteAsociado;
    public LineRenderer lineRenderer;
    public Transform puntoInicioLinea; // Desde dónde sale la línea (opcional)
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

    [Tooltip("Distancia mínima para activar el cristal")]
    public float rangoActivacion = 3f;

    [Header("Mensaje sin Habilidad")]
    [TextArea(2, 4)]
    public string mensajeSinHabilidad = "Necesitas la habilidad de Luminiscencia para activar este cristal.";
    public TMPro.TextMeshProUGUI textoMensaje; // Referencia al texto del Canvas
    public float tiempoMensaje = 3f;

    private bool mostrandoMensaje = false;

    public Action OnCristalClickado;

    private bool activado = false;
    private Material materialCristal;
    private Vector3 posicionInicial;
    private Coroutine coroutinePista;

    void Start()
    {
        // Buscar el renderer en este objeto o en sus hijos
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            materialCristal = renderer.material;
        }

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
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                // Verificar distancia del jugador
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player == null)
                {
                    Debug.LogWarning("No se encontró jugador con tag 'Player'. Activando cristal de todos modos (para testing).");
                    OnCristalClickado?.Invoke();
                    return;
                }

                float distancia = Vector3.Distance(transform.position, player.transform.position);

                Debug.Log($"Distancia al cristal: {distancia:F2}m (Rango requerido: {rangoActivacion}m)");

                if (distancia > rangoActivacion)
                {
                    Debug.LogWarning($"❌ Estás muy lejos del cristal. Acércate más (necesitas estar a {rangoActivacion}m o menos)");
                    return;
                }

                Debug.Log("✅ Estás dentro del rango de activación");

                // Verificar si el jugador tiene la habilidad
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
        }
    }

    void MostrarMensajeSinHabilidad()
    {
        if (textoMensaje != null)
        {
            StopAllCoroutines();
            StartCoroutine(MostrarMensajeCoroutine());
        }
        else
        {
            Debug.Log(mensajeSinHabilidad);
        }
    }

    IEnumerator MostrarMensajeCoroutine()
    {
        textoMensaje.text = mensajeSinHabilidad;
        textoMensaje.gameObject.SetActive(true);

        yield return new WaitForSeconds(tiempoMensaje);

        textoMensaje.gameObject.SetActive(false);
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
    public void ActivarPermanente(Transform destinoLinea = null)
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

        // Activar línea de energía hacia el puente
        Transform destino = destinoLinea != null ? destinoLinea : puntoDestino;

        if (lineRenderer != null && destino != null)
        {
            lineRenderer.enabled = true;

            // Determinar punto de inicio (desde donde sale la línea)
            Vector3 puntoInicio = puntoInicioLinea != null ? puntoInicioLinea.position : transform.position;

            lineRenderer.SetPosition(0, puntoInicio);
            lineRenderer.SetPosition(1, destino.position);

            // Usar el color del LineRenderer ya configurado, pero aplicar el color activo
            Color colorLinea = lineRenderer.startColor;
            if (colorLinea == Color.white || colorLinea == Color.clear)
            {
                // Si no tiene color asignado, usar el color activo del cristal
                colorLinea = colorActivo;
            }

            lineRenderer.startColor = colorLinea;
            lineRenderer.endColor = colorLinea;

            Debug.Log($"LineRenderer activado desde {puntoInicio} hacia {destino.position} con color {colorLinea}");

            // Animar la línea
            StartCoroutine(AnimarLineaEnergia());
        }
        else
        {
            if (lineRenderer == null)
                Debug.LogWarning($"Cristal {gameObject.name}: No tiene LineRenderer asignado");
            if (destino == null)
                Debug.LogWarning($"Cristal {gameObject.name}: No hay destino para la línea");
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

    void OnDrawGizmosSelected()
    {
        if (!esCristalPista)
        {
            // Dibujar rango de activación
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangoActivacion);
        }
    }
}