using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlot : MonoBehaviour
{
    [Header("Referencias")]
    public RawImage itemImage;
    public Texture emptyTexture;

    private Texture currentItemTexture;
    private bool hasItem = false;

    void Start()
    {
        if (itemImage != null && emptyTexture != null)
        {
            itemImage.texture = emptyTexture;
        }
    }

    public void AddItem(Texture itemTexture)
    {
        if (itemImage != null && itemTexture != null)
        {
            currentItemTexture = itemTexture;
            itemImage.texture = itemTexture;
            hasItem = true;
        }
    }

    public void RemoveItem()
    {
        if (itemImage != null && emptyTexture != null)
        {
            itemImage.texture = emptyTexture;
            currentItemTexture = null;
            hasItem = false;
        }
    }

    public bool HasItem()
    {
        return hasItem;
    }

    public Texture GetItemTexture()
    {
        return currentItemTexture;
    }
}

