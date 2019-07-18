using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    #region Singleton
    public static Crafting instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Craft found!");
        }
        instance = this;
    }
    #endregion

    private Object[] recipes;
    public List<Recipe> recipeList = new List<Recipe>();
    public int numberOfAbilities = 7;
    public delegate void OnCrafting();
    public OnCrafting onCraftingCallback;

    void Start()
    {

        if (recipeList.Count == 0)
        {
            recipes = Resources.LoadAll("Recipes", typeof(Recipe));
            if (recipes.Length > numberOfAbilities)
            {
                Debug.Log("Too many recipes.");
            }
            else
            {
                for (int i = 0; i < recipes.Length; i++)
                {
                    recipeList.Add((Recipe)recipes[i]);
                }
            }
        }
        if (onCraftingCallback != null)
        {
            onCraftingCallback.Invoke();
        }

    }

    // Removes an item from the recipe list.
    public void RemoveCompletedRecipe(Recipe completedRecipe)
    {
        recipeList.Remove(completedRecipe);
        if (onCraftingCallback != null)
        {
            onCraftingCallback.Invoke();
        }
    }

    public void RestoreCrafting(int[] availableRecipes)
    {
        recipeList.Clear();
        Elements elements = GetComponent<Elements>();
        for (int i = 0; i < availableRecipes.Length; i++)
        {
            recipeList.Add(elements.ConvertNumberToRecipe(availableRecipes[i]));
        }
        if (onCraftingCallback != null)
        {
            onCraftingCallback.Invoke();
        }
    }
}
