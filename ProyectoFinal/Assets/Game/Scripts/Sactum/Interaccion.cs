using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteraccionRaycast : MonoBehaviour
{
    // Configuración del raycast
    public float distanciaRayo = 10f;// Distancia máxima del raycast
    public LayerMask capasIgnoradas; // Para ignorar al player

    void Update()
    {
        // Crear un rayo desde el centro de la cámara (screen center)
        Ray rayo = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Dibujar el rayo en la escena para depuración
        Debug.DrawRay(rayo.origin, rayo.direction * distanciaRayo, Color.red);

        // Raycast ignorando ciertas capas
        if (Physics.Raycast(rayo, out hit, distanciaRayo, ~capasIgnoradas))
        {
            Debug.Log("Rayo golpeó: " + hit.collider.name + " | Tag: " + hit.collider.tag);

            // Verificar si el objeto golpeado es un fragmento
            if (hit.collider.CompareTag("Fragmento"))
            {
                Debug.Log("Es un fragmento!");

                // Detectar clic izquierdo del mouse
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Clic detectado!");

                    // Obtener el script Fragmento del objeto golpeado
                    Fragmento fragmento = hit.collider.GetComponent<Fragmento>();

                    if (fragmento != null)
                    {
                        Debug.Log("Script Fragmento encontrado!");

                        // Mostrar el panel del fragmento y pausar el juego
                        fragmento.MostrarPanel();
                    }
                    else
                    {
                        Debug.LogError("No se encontró el script Fragmento");
                    }
                }
            }
        }
    }
}