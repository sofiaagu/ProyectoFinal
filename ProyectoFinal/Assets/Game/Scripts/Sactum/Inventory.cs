using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    public GameObject inventory;
    public GameObject slotHolder;

    private bool inventoryEnabled;
    private int allSlots;
    private Slot[] slots;

    public static Inventory instance; // Para acceder desde otros scripts

    void Awake()
    {
        // Singleton simple
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (slotHolder != null)
        {
            allSlots = slotHolder.transform.childCount;
            slots = new Slot[allSlots];

            for (int i = 0; i < allSlots; i++)
            {
                slots[i] = slotHolder.transform.GetChild(i).GetComponent<Slot>();
            }
        }

        if (inventory != null)
        {
            inventory.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryEnabled = !inventoryEnabled;
        }

        if (inventoryEnabled == true)
        {
            if (inventory != null)
            {
                inventory.SetActive(true);
            }
        }
        else
        {
            if (inventory != null)
            {
                inventory.SetActive(false);
            }
        }
    }

    public bool AgregarFragmento(Texture2D texturaFragmento)
    {
        // Busca el primer slot vacío
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && !slots[i].EstaOcupado())
            {
                slots[i].AgregarFragmento(texturaFragmento);
                Debug.Log("Fragmento agregado al slot " + i);
                return true;
            }
        }

        Debug.Log("Inventario lleno!");
        return false;
    }
}