using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Lastest checkpoint save
    public string checkpointLevel;
    // public bool loadingCheckpoint;

    // Current level

    // Inventory
    public int[] inventoryList;

    // Crafting
    public int[] availableRecipes;

    // ABILITTY VARIABLES
    // Activate abilities.
    public bool canJump;
    public bool canWallJump;
    public bool canSwim;
    public bool canGrab;
    public bool canRecall;
    public bool canPortalWarp;
    public bool canGrapple;

    public PlayerData(GameObject player)
    {
        // Necessary variables for saving
        PlayerController playerController = player.GetComponent<PlayerController>();
        Inventory inventory = Inventory.instance;
        Crafting crafting = Crafting.instance;

        checkpointLevel = playerController.checkpointLevel;
        // hasCheckpointSave = playerController.hasCheckpointSave;

        // Saves if abilities are active
        canJump = playerController.PlayerCanJump();
        canWallJump = playerController.PlayerCanWallJump();
        canSwim = playerController.PlayerCanSwim();
        canGrab = playerController.PlayerCanGrab();
        canRecall = playerController.PlayerCanRecall();
        canPortalWarp = playerController.PlayerCanPortalWarp();
        canGrapple = playerController.PlayerCanGrapple();

        // Storing inventory
        Item[] itemList = inventory.itemList.ToArray();
        inventoryList = new int[inventory.itemList.Count];
        for (int i = 0; i < inventoryList.Length; i++)
        {
            inventoryList[i] = itemList[i].elementNumber;
        }

        // Storing available recipes
        Recipe[] recipeList = crafting.recipeList.ToArray();
        availableRecipes = new int[crafting.recipeList.Count];
        for (int i = 0; i < availableRecipes.Length; i++)
        {
            availableRecipes[i] = recipeList[i].recipeNumber;
        }
      
    }
}
