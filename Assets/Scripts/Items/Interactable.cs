using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private string name;
    public string Name { get { return name; } set { name = value; } }

    public bool isOnUse = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.inputManager.NearbyInteractable = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.inputManager.NearbyInteractable = null;
        }
    }

    public void Interact()
    {
        GameManager.Instance.inventoryManager.AddToInventory(this);
        gameObject.SetActive(false);
        GameManager.Instance.inputManager.NearbyInteractable = null;
    }

    public virtual void UseFromInventory()
    {
    }

    public virtual void StartUse()
    {
        isOnUse = true;
    }

    public virtual void EndUse()
    {
        isOnUse = false;
    }

}
