using UnityEngine;

public class ParedOscuraCC : MonoBehaviour
{
    public float distancia = 5f;   // Distancia total que recorrerá la pared
    public float velocidad = 2f;// Velocidad del movimiento
    public bool horizontal = true; // Si es true, se moverá en X; si es false, en Y

    private Vector3 posInicial; // Donde empieza la pared
    private Vector3 posObjetivo;// Hasta dónde se moverá
    private bool vaHaciaObjetivo = true;// Controla el sentido del movimiento

    void Start()
    {
        // Guardamos la posición inicial para saber de dónde parte
        posInicial = transform.position;

        // Calcula el punto objetivo dependiendo de si es movimiento horizontal o vertical
        if (horizontal)
            posObjetivo = posInicial + transform.right * distancia;
        else
            posObjetivo = posInicial + transform.up * distancia;
    }

    void Update()
    {
        // Decide si moverse hacia el objetivo o volver al inicio
        Vector3 destino = vaHaciaObjetivo ? posObjetivo : posInicial;

        // Mover la pared suavemente usando MoveTowards
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        // Si llegó al destino, cambia la dirección
        if (Vector3.Distance(transform.position, destino) < 0.01f)
            vaHaciaObjetivo = !vaHaciaObjetivo;
    }

    // Detecta cuando el jugador entra al trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))// Si chocó con el jugador
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1); // Solo se aplica 1 vez al entrar
        }
    }

}
