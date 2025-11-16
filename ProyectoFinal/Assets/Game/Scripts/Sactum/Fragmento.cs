// Script MODIFICADO para trabajar con tu sistema de Fragmento
using UnityEngine;

public class Fragmento : MonoBehaviour
{
    [Header("Panel de Confirmación")]
    public GameObject panelUI; // Panel de confirmación al hacer click

    [Header("Configuración del Item para Inventario")]
    public Texture itemTexture; // La textura que se mostrará en el inventario
    public int slotIndex = 0; // 0, 1 o 2 para los 3 slots

    private InventoryController inventoryManager;  // ← Cambié el tipo

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryController>();

        if (inventoryManager == null)
        {
            Debug.LogError("No se encontró InventoryManager en la escena");
        }

        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }
    }

    // Método que se llama desde tu sistema de raycast cuando haces click
    public void MostrarPanel()
    {
        Debug.Log("Mostrando panel del fragmento: " + gameObject.name);
        if (panelUI != null)
        {
            panelUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // Método que se llama cuando presionas el botón de recolectar en el panel
    public void RecolectarFragmento()
    {
        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }

        Time.timeScale = 1f;

        // Añadir el item al inventario
        if (inventoryManager != null && itemTexture != null)
        {
            inventoryManager.AddItemToSlot(slotIndex, itemTexture);
            Debug.Log($"¡Fragmento recolectado y añadido al slot {slotIndex}!");
        }
        else
        {
            Debug.LogWarning("No se pudo añadir el item: InventoryManager o itemTexture no está asignado");
        }

        Destroy(gameObject);
    }
}

// Script manager para controlar todos los slots
public class InventoryManager : MonoBehaviour
{
    [Header("Arrastra aquí los 3 slots desde la jerarquía")]
    public InventorySlot slot1;
    public InventorySlot slot2;
    public InventorySlot slot3;

    private InventorySlot[] inventorySlots;

    void Awake()
    {
        // Crear el array con los slots asignados
        inventorySlots = new InventorySlot[3];
        inventorySlots[0] = slot1;
        inventorySlots[1] = slot2;
        inventorySlots[2] = slot3;
    }

    public void AddItemToSlot(int slotIndex, Texture itemTexture)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            if (inventorySlots[slotIndex] != null)
            {
                inventorySlots[slotIndex].AddItem(itemTexture);
                Debug.Log($"Item añadido al slot {slotIndex}");
            }
            else
            {
                Debug.LogWarning($"El slot {slotIndex} no está asignado en el InventoryManager");
            }
        }
        else
        {
            Debug.LogWarning($"Índice de slot inválido: {slotIndex}");
        }
    }

    public bool IsSlotFull(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            return inventorySlots[slotIndex] != null && inventorySlots[slotIndex].HasItem();
        }
        return false;
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            if (inventorySlots[slotIndex] != null)
            {
                inventorySlots[slotIndex].RemoveItem();
            }
        }
    }

    public void ClearAllSlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            ClearSlot(i);
        }
    }
}