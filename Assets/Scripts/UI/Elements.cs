using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements : MonoBehaviour
{
    public Item element0;
    public Item element1;
    public Item element2;
    public Item element3;
    public Item element4;
    public Item element5;
    public Item element6;
    public Item element7;
    public Item element8;
    public Item element9;

    public Recipe recipe0;
    public Recipe recipe1;
    public Recipe recipe2;
    public Recipe recipe3;
    public Recipe recipe4;
    public Recipe recipe5;
    public Recipe recipe6;

    public Item ConvertNumberToItem(int n)
    {
        Item[] elementsList = { element0, element1, element2, element3, element4, element5, element6, element7, element8, element9};
        return elementsList[n];
    }


    public Recipe ConvertNumberToRecipe(int n)
    {
        Recipe[] recipeList = { recipe0, recipe1, recipe2, recipe3, recipe4, recipe5, recipe6 };
        return recipeList[n];
    }
}
