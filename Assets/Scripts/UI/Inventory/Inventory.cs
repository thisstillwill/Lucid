using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Makes sure that there is only one inventory that is always easily accessible.
    #region Singleton
    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
        }
        instance = this;
    }
    #endregion

    public List<Item> itemList = new List<Item>();
    public int space = 15;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    void Start()
    {
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    // Adds an Item to the inventory list.
    public bool Add(Item item)
    {
            if (itemList.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }
            itemList.Add(item);
            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        return true;
    }

   
    public void RestoreInventory(int[] items)
    {
        itemList.Clear();
        Elements elements = gameObject.GetComponent<Elements>();
        for (int i = 0; i < items.Length; i++)
        {
            Add(elements.ConvertNumberToItem(items[i]));
        }
    }

    // Removes an item from the inventory list.
    public void Remove(Item item)
    {
        itemList.Remove(item);
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }


}
