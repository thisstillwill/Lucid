using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    
    // Inventory inventory;
    Crafting craft;

    public Transform recipesSlots;
    public GameObject craftingPanel;
    CraftingSlot[] craftSlots;
    private GameObject GameManager;



    // Start is called before the first frame update
    void Start()
    {
      
        // inventory = Inventory.instance;
        craft = Crafting.instance;
        craft.onCraftingCallback += UpdateUI;
        craftSlots = recipesSlots.GetComponentsInChildren<CraftingSlot>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager");

    }
    

    void UpdateUI()
    {
        for (int i = 0; i < craft.numberOfAbilities; i++)
        {
            Slot[] itemSlots = craftSlots[i].slotsParent.GetComponentsInChildren<Slot>();
            if (i < craft.recipeList.Count)
            {
                
                craftSlots[i].AddRecipe(craft.recipeList[i], itemSlots, GameManager);

            }
            else craftSlots[i].DeactivateCraftingSlot(itemSlots);
        }
    }

}
