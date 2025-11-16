using UnityEngine;

public class InventoryController : MonoBehaviour  // ← Cambié el nombre
{
    [Header("Arrastra aquí los 3 slots desde la jerarquía")]
    public InventorySlot slot1;
    public InventorySlot slot2;
    public InventorySlot slot3;

    private InventorySlot[] slots;

    void Awake()
    {
        slots = new InventorySlot[] { slot1, slot2, slot3 };
    }

    public void AddItemToSlot(int slotIndex, Texture itemTexture)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            if (slots[slotIndex] != null)
            {
                slots[slotIndex].AddItem(itemTexture);
                Debug.Log($"Item añadido al slot {slotIndex}");
            }
            else
            {
                Debug.LogWarning($"El slot {slotIndex} no está asignado");
            }
        }
        else
        {
            Debug.LogWarning($"Índice de slot inválido: {slotIndex}");
        }
    }

    public bool IsSlotFull(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            return slots[slotIndex] != null && slots[slotIndex].HasItem();
        }
        return false;
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            if (slots[slotIndex] != null)
            {
                slots[slotIndex].RemoveItem();
            }
        }
    }

    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            ClearSlot(i);
        }
    }
}