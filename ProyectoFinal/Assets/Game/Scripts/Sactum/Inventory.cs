using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    public GameObject inventory;
    public GameObject slotHolder;// Objeto padre que contiene los slots

    // Variables internas
    private bool inventoryEnabled;// Si el inventario está activo
    private int allSlots; // Número total de slots
    private Slot[] slots; // Array de slots para almacenar fragmentos

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
            // Contar todos los slots hijos
            allSlots = slotHolder.transform.childCount;
            slots = new Slot[allSlots];

            // Guardar referencias a cada componente Slot
            for (int i = 0; i < allSlots; i++)
            {
                slots[i] = slotHolder.transform.GetChild(i).GetComponent<Slot>();
            }
        }

        // Ocultar inventario al inicio
        if (inventory != null)
        {
            inventory.SetActive(false);
        }
    }

    void Update()
    {
        // Detectar pulsación de tecla I
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryEnabled = !inventoryEnabled;
        }

        // Activar o desactivar panel según estado
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

    // Método para agregar un fragmento al inventario
    public bool AgregarFragmento(Texture2D texturaFragmento)
    {
        // Busca el primer slot vacío
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && !slots[i].EstaOcupado())
            {
                // Agregar fragmento al slot
                slots[i].AgregarFragmento(texturaFragmento);
                Debug.Log("Fragmento agregado al slot " + i);
                return true;
            }
        }
        // Si no hay slots vacíos
        Debug.Log("Inventario lleno!");
        return false;
    }
}