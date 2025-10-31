//using UnityEngine;
//using UnityEngine.InputSystem;

//public class CollectableItem : MonoBehaviour
//{
//    public enum ItemType { Cereza, Kiwi, Bandera }  // Tipos de ítems
//    public ItemType itemType;
//    public int itemValue = 0;
//    public float clickDistance = 3f;

//    private Transform player;
//    public Camera mainCamera;

//    void Start()
//    {
//        //mainCamera = Camera.main;
//        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//        if (playerObj != null)
//            player = playerObj.transform;
//    }

//    void Update()
//    {
//        // Detectar clic izquierdo con el nuevo Input System
//        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
//        {
//            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
//            RaycastHit hit;

//            if (Physics.Raycast(ray, out hit))
//            {
//                if (hit.transform == transform)
//                {
//                    float distance = Vector3.Distance(transform.position, player.position);
//                    if (distance <= clickDistance)
//                    {
//                        CollectItem();
//                    }
//                    else
//                    {
//                        Debug.Log("Muy lejos");
//                    }
//                }
//            }
//            else
//            {
//                Debug.Log("No he pegado con nada");
//            }
//        }
//    }

//    void CollectItem()
//    {
//        if (GameManager.Instance != null)
//        {
//            GameManager.Instance.AddScore(itemValue);
//            GameManager.Instance.AddItem();
//        }

//        Destroy(gameObject);
//    }
//}
