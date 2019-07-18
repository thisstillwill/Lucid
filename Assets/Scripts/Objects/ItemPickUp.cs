using UnityEngine;

public class ItemPickUp : Interactable
{

    public Item item;

    public override void Interact()
    {
        // This can be used if you implement anything in the Interact() function in Interactable.cs
        // base.Interact();

        PickUp();

    }

    void PickUp()
    {
        bool wasPickedUp = Inventory.instance.Add(item);
        //Add Item to Inventory
        if (wasPickedUp)
        {
            AudioManager.Play("Pickup");
            GetComponent<Interactable>().interactDisplay.SetActive(false);
            gameObject.SetActive(false);
        }
        else AudioManager.Play("Negative");
    }

}
