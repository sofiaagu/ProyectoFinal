using UnityEngine;

public class PlataformaMovimiento : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    public bool moverHorizontal = true;   // Si está activado, se mueve de derecha a izquierda
    public bool moverVertical = false;    // Si está activado, se mueve de arriba a abajo
    public float distancia = 3f;          // Distancia total del recorrido
    public float velocidad = 2f;          // Velocidad del movimiento

    private Vector3 puntoInicio;
    private Vector3 puntoDestino;
    private bool yendoAlDestino = true;

    void Start()
    {
        puntoInicio = transform.position;

        // Define el punto final según el tipo de movimiento
        if (moverHorizontal)
            puntoDestino = puntoInicio + Vector3.right * distancia;
        else if (moverVertical)
            puntoDestino = puntoInicio + Vector3.up * distancia;
    }

    void Update()
    {
        // Mueve la plataforma entre los dos puntos
        transform.position = Vector3.MoveTowards(transform.position,
            yendoAlDestino ? puntoDestino : puntoInicio,
            velocidad * Time.deltaTime);

        // Cambia de dirección al llegar a un extremo
        if (Vector3.Distance(transform.position, yendoAlDestino ? puntoDestino : puntoInicio) < 0.05f)
            yendoAlDestino = !yendoAlDestino;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 destinoPreview = transform.position;

        if (moverHorizontal)
            destinoPreview += Vector3.right * distancia;
        else if (moverVertical)
            destinoPreview += Vector3.up * distancia;

        Gizmos.DrawLine(transform.position, destinoPreview);
        Gizmos.DrawSphere(destinoPreview, 0.1f);
    }
}
