using UnityEngine;

public class ParedOscuraCC : MonoBehaviour
{
    public float distancia = 5f;   // Distancia a recorrer
    public float velocidad = 2f;
    public bool horizontal = true;

    private Vector3 posInicial;
    private Vector3 posObjetivo;
    private bool vaHaciaObjetivo = true;

    void Start()
    {
        posInicial = transform.position;

        if (horizontal)
            posObjetivo = posInicial + transform.right * distancia;
        else
            posObjetivo = posInicial + transform.up * distancia;
    }

    void Update()
    {
        Vector3 destino = vaHaciaObjetivo ? posObjetivo : posInicial;
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.01f)
            vaHaciaObjetivo = !vaHaciaObjetivo;
    }

    // Detectar contacto con CharacterController
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1); // Solo se aplica 1 vez al entrar
        }
    }

}
