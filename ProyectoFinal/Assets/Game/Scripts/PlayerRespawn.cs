//using UnityEngine;

//public class PlayerRespawn : MonoBehaviour
//{
//    [Header("Configuración")]
//    public float alturaMinima = -10f;
//    public Transform puntoRespawn;

//    [Header("Efectos (Opcional)")]
//    public GameObject efectoMuerte;
//    public AudioClip sonidoMuerte;

//    private AudioSource audioSource;

//    void Start()
//    {
//        audioSource = GetComponent<AudioSource>();
//        if (audioSource == null)
//            audioSource = gameObject.AddComponent<AudioSource>();
//    }

//    void Update()
//    {
//        // Verificar si cayó por debajo de la altura mínima
//        if (transform.position.y < alturaMinima)
//        {
//            Morir();
//        }
//    }

//    void Morir()
//    {
//        Debug.Log("Player murió - Reapareciendo...");

//        // Efecto de muerte
//        if (efectoMuerte != null)
//        {
//            Instantiate(efectoMuerte, transform.position, Quaternion.identity);
//        }

//        // Sonido
//        if (sonidoMuerte != null && audioSource != null)
//        {
//            audioSource.PlayOneShot(sonidoMuerte);
//        }

//        // Reaparacer
//        Respawn();
//    }

//    void Respawn()
//    {
//        // Detener velocidad si tiene Rigidbody
//        Rigidbody rb = GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            rb.linearVelocity = Vector3.zero;
//            rb.angularVelocity = Vector3.zero;
//        }

//        // Teletransportar al punto de respawn
//        if (puntoRespawn != null)
//        {
//            transform.position = puntoRespawn.position;
//        }
//        else
//        {
//            // Si no hay punto asignado, volver al origen
//            transform.position = new Vector3(0, 1, 0);
//        }

//        Debug.Log("Player reaparecido en: " + transform.position);
//    }
//}
using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Configuración")]
    public float alturaMinima = 900f;
    public Transform puntoRespawn;
    public float tiempoAntesDeRespawn = 3f;

    [Header("Efectos (Opcional)")]
    public GameObject efectoMuerte;
    public AudioClip sonidoMuerte;

    private bool estaMuriendo = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (puntoRespawn == null)
        {
            GameObject respawnPoint = new GameObject("RespawnPoint");
            respawnPoint.transform.position = transform.position;
            puntoRespawn = respawnPoint.transform;
        }
    }

    void Update()
    {
        if (!estaMuriendo && transform.position.y < alturaMinima)
        {
            StartCoroutine(Morir());
        }
    }

    IEnumerator Morir()
    {
        estaMuriendo = true;

        yield return new WaitForSeconds(tiempoAntesDeRespawn);

        if (efectoMuerte != null)
            Instantiate(efectoMuerte, transform.position, Quaternion.identity);

        if (sonidoMuerte != null && audioSource != null)
            audioSource.PlayOneShot(sonidoMuerte);

        Respawn();
    }

    void Respawn()
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = puntoRespawn.position;

        if (cc != null) cc.enabled = true;

        estaMuriendo = false;
    }
}