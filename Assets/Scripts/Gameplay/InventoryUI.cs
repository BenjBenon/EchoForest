using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public struct InventorySlotUI
    {
        public Image slotImage;
        public Button slotButton;
    }

    [SerializeField] private List<InventorySlotUI> inventorySlots;


    [System.Serializable]
    public struct ItemSpriteDict
    {
        public string itemName;
        public Sprite sprite;
    }

    [SerializeField] private List<ItemSpriteDict> spriteDictionary;

    private Dictionary<string, Sprite> dictionaryNameSprite = new Dictionary<string, Sprite>();

    private void Awake()
    {
        foreach (var entry in spriteDictionary)
        {
            dictionaryNameSprite[entry.itemName] = entry.sprite;
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].slotButton.onClick.AddListener(() => UseItem(i));
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.inventoryManager != null)
        {
            GameManager.Instance.inventoryManager.OnInventoryChanged += UpdateInventoryUI;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.inventoryManager != null)
        {
            GameManager.Instance.inventoryManager.OnInventoryChanged -= UpdateInventoryUI;
        }
    }

    private void UpdateInventoryUI()
    {
        var inventory = GameManager.Instance.inventoryManager.inventory;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventory.Count)
            {
                var item = inventory[i];

                if (dictionaryNameSprite.TryGetValue(item.Name, out Sprite sprite))
                {
                    inventorySlots[i].slotImage.sprite = sprite;
                    inventorySlots[i].slotImage.enabled = true;
                    inventorySlots[i].slotButton.interactable = true;
                }
                else
                {
                    inventorySlots[i].slotImage.enabled = false;
                    inventorySlots[i].slotButton.interactable = false;

                }
            }
            else
            {
                inventorySlots[i].slotImage.sprite = null;
                inventorySlots[i].slotImage.enabled = false;
                inventorySlots[i].slotButton.interactable = false;
            }
        }

    }

    public void UseItem(int slotIndex)
    {

        var inventory = GameManager.Instance.inventoryManager.inventory;

        if (slotIndex < 0 || slotIndex >= inventory.Count) return;

        var item = inventory[slotIndex];
        item.UseFromInventory();
        GameManager.Instance.inventoryManager.UseItem(item);
    }
}
