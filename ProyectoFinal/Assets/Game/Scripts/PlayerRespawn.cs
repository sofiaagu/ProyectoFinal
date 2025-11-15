using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Configuración")]
    public float alturaMinima = 970f;
    public Transform puntoRespawn; // PÚBLICO para que los cubos puedan cambiarlo
    public float tiempoAntesDeRespawn = 2f;

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

        if (puntoRespawn != null)
        {
            transform.position = puntoRespawn.position;
        }
        else
        {
            transform.position = new Vector3(0, 1, 0);
        }

        if (cc != null) cc.enabled = true;
        estaMuriendo = false;
    }
}