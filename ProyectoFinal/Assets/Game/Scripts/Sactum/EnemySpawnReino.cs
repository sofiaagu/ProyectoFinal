using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Enemigos")]
    public GameObject enemigoPrefab; 
    public int cantidadEnemigos = 5; // Cuántos enemigos spawnar

    [Header("Puntos de Spawn")]
    public Transform[] puntosDeSpawn; // Arrastra aquí los puntos de spawn

    [Header("Opciones de Spawn")]
    public bool spawnearAlInicio = true; // Si true, spawnea enemigos al inicio de la escena
    public bool spawnContinuo = false; // Si true, seguirá spawnando enemigos con tiempo
    public float tiempoEntreSpawns = 10f; // Tiempo entre cada spawn continuo
    public int maximoEnemigosEnMapa = 10; // Máximo de enemigos simultáneos

    // Lista para llevar control de enemigos activos en la escena
    private List<GameObject> enemigosActivos = new List<GameObject>();

    void Start()
    {

        // Spawnear todos los enemigos iniciales
        if (spawnearAlInicio)
        {
            SpawnearEnemigos();
        }

        // Si spawn continuo está activado, invoca SpawnEnemigoContinuo repetidamente
        if (spawnContinuo)
        {
            InvokeRepeating("SpawnEnemigoContinuo", tiempoEntreSpawns, tiempoEntreSpawns);
        }
    }

    void Update()
    {
        // Limpia la lista de enemigos que fueron destruidos
        enemigosActivos.RemoveAll(enemigo => enemigo == null);
    }

    public void SpawnearEnemigos()
    {
        // Validaciones de seguridad
        if (puntosDeSpawn.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn asignados!");
            return;
        }

        if (enemigoPrefab == null)
        {
            Debug.LogError("No hay prefab de enemigo asignado!");
            return;
        }
        // Instancia la cantidad definida de enemigos
        for (int i = 0; i < cantidadEnemigos; i++)
        {
            SpawnEnemigo();
        }
    }

    // SPAWN DE UN ENEMIGO INDIVIDUAL
    void SpawnEnemigo()
    {
        // Selecciona un punto aleatorio
        int indiceAleatorio = Random.Range(0, puntosDeSpawn.Length);
        Transform puntoSpawn = puntosDeSpawn[indiceAleatorio];

        // Instancia el enemigo
        GameObject nuevoEnemigo = Instantiate(enemigoPrefab, puntoSpawn.position, puntoSpawn.rotation);
        enemigosActivos.Add(nuevoEnemigo);

        Debug.Log($"Enemigo spawneado en: {puntoSpawn.name}");
    }

    void SpawnEnemigoContinuo()
    {
        // Solo spawnea si no se alcanzó el máximo
        if (enemigosActivos.Count < maximoEnemigosEnMapa)
        {
            SpawnEnemigo();
        }
    }

    // Método público para spawnear manualmente desde otro script
    public void SpawnearUnEnemigo()
    {
        if (enemigosActivos.Count < maximoEnemigosEnMapa)
        {
            SpawnEnemigo();
        }
    }

    // Método para eliminar todos los enemigos
    public void EliminarTodosLosEnemigos()
    {
        foreach (GameObject enemigo in enemigosActivos)
        {
            if (enemigo != null)
            {
                Destroy(enemigo);
            }
        }
        enemigosActivos.Clear();
    }
}