using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    public Transform itemsParent;
    public Transform craftsParent;
    Slot[] slots;
    Slot[] craftSlots;


    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<Slot>();
        craftSlots = craftsParent.GetComponentsInChildren<Slot>();
    }

    // Updates the Inventory UI to fill up all available Item Slots with the icons of the stored items.
    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.itemList.Count)
            {
                slots[i].AddItem(inventory.itemList[i]);
                craftSlots[i].AddItem(inventory.itemList[i]);
            }
            else
            {
                slots[i].ClearSlot();
                craftSlots[i].ClearSlot();
            }
        }
    }
}
