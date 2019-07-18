using UnityEngine;


[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    new public string name = "New Recipe";
    public int recipeNumber;
    // public Item craftingItem = null;
    // public Sprite craftingItemIcon = null;
    // Stores the number of required items for each collectible
    // Collectible 0 = Sneaker
    // Collectible 1 = Coconut
    // Collectible 2 = Island Flower
    // Collectible 3 = Snail
    // Collectible 4 = Pearl
    // Collectible 5 = Spectre
    // Collectible 6 = Creature Egg
    // Collectible 7 = Pocket Portal
    // Collectible 8 = Recall Totem
    // Collectible 9 = Rusty Hook
    public int[] requiredItems = new int[10];

}

