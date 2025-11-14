using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteraccionRaycast : MonoBehaviour
{
    public float distanciaRayo = 10f;
    public LayerMask capasIgnoradas; // Para ignorar al player

    void Update()
    {
        Ray rayo = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Debug.DrawRay(rayo.origin, rayo.direction * distanciaRayo, Color.red);

        // Raycast ignorando ciertas capas
        if (Physics.Raycast(rayo, out hit, distanciaRayo, ~capasIgnoradas))
        {
            Debug.Log("Rayo golpeó: " + hit.collider.name + " | Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Fragmento"))
            {
                Debug.Log("✅ Es un fragmento!");

                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("🖱️ Clic detectado!");

                    Fragmento fragmento = hit.collider.GetComponent<Fragmento>();

                    if (fragmento != null)
                    {
                        Debug.Log("✅ Script Fragmento encontrado!");
                        fragmento.MostrarPanel();
                    }
                    else
                    {
                        Debug.LogError("❌ No se encontró el script Fragmento");
                    }
                }
            }
        }
    }
}