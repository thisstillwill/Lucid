using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    public Recipe recipe;
    public Transform slotsParent;
    public Text recipeTitle;
    private Slot[] slots;
    private int currentSlot;
    private GameObject GameManager;
    /* // 12 collectibles total
    public Item zeroth;     // Element 0
    public Item first;      // Element 1
    public Item second;     // Element 2
    public Item third;      // Element 3
    public Item fourth;     // Element 4
    public Item fifth;      // Element 5
    public Item sixth;      // Element 6
    public Item seventh;    // Element 7
    public Item eigth;      // Element 8
    public Item ninth;      // Element 9
    public Item tenth;      // Element 10
    public Item eleventh;   // Element 11 */

    void Start()
    {
        currentSlot = 0;
    }

    public void AddRecipe(Recipe newRecipe, Slot[] slots, GameObject gameManager)
    {
        GameManager = gameManager;
        if (recipe != null)
        {
            ResetCraftingSlot(slots);
        }
        recipe = newRecipe;
        recipeTitle.text = recipe.name;
        this.slots = slots;
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            if (recipe.requiredItems[i] != 0)
            {
                RecipeCheck(i, slots);
            }
        }
    }


    // Checks which element is part of recipe.
    private void RecipeCheck(int index, Slot[] slots)
    {
        Elements elements = GameManager.GetComponent<Elements>();
        slots[currentSlot].AddRecipeComponent(elements.ConvertNumberToItem(index), recipe.requiredItems[index]);
        currentSlot++;

    }

    public int ComponentCheck(int elementNumber)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<Slot>().item.isRecipeItem)
            {
                if (slots[i].GetComponent<Slot>().item.elementNumber == elementNumber && slots[i].amount >= 0)
                {
                    return i;
                }
            } 
        }
        return -1;
    }

    public bool isCompleted()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<Slot>().amount != 0)
            {
                return false;
            }
        }
        return true;
    }

    public void DeactivateCraftingSlot(Slot[] slots)
    {
        ResetCraftingSlot(slots);
    }

    private void ResetCraftingSlot(Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        recipe = null;
        recipeTitle.text = "";
        currentSlot = 0;
    } 
}

