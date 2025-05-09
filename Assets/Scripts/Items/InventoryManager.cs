using UnityEngine;
using System.Collections.Generic;
using System;


public class InventoryManager : MonoBehaviour
{
    public List<Interactable> inventory = new List<Interactable>();

    public event Action OnInventoryChanged;

    private const int maxInventorySize = 6;

    public void AddToInventory(Interactable item)
    {
        if (inventory.Count >= maxInventorySize) return;

        inventory.Add(item);
        GameManager.Instance.inventoryAudio.Play();

        if (OnInventoryChanged != null)
        {
            OnInventoryChanged.Invoke();
        }
    }

    public bool HasItem(Interactable item)
    {
        return inventory.Contains(item);
    }

    public void UseItem(Interactable item)
    {
        if (HasItem(item))
        {
            inventory.Remove(item);
            if (OnInventoryChanged != null)
            {
                OnInventoryChanged.Invoke();
            }
        }
    }
}
