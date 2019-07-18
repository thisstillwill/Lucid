using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvents : MonoBehaviour
{
    Inventory inventory; // The inventory manager; updates the inventory UI.
    Crafting craft; // The crafting manager; updates the crafting UI.

    GameObject player;
    GameObject GameManager;
    public GameObject loadingText;
    public GameObject creditsPanel;
    public GameObject titlePanel;
    public GameObject pausePanel;

    private AudioManager audioManager;

    public Color highlightColor;

    public string HubLevelName;

    // The parent of the mini slots.
    public Transform miniInventoryParent;

    // Craft slots
    public GameObject currentRecipe; // The currently selected crafting slot.
    private Button currentRecipeButton; // The current crafting slot's button.
    private Slot[] craftSlots; // The current crafting slot's nested item slots.

    
    private Slot[] miniSlots; // Mini inventory slots

    public List<Item> recipeComponents; // Stores all of the selected recipe components.
    
    // Initializes the inventory manager and the crafting manager.
    void Start()
    {
        inventory = Inventory.instance;
        craft = Crafting.instance;
        player = GameObject.FindGameObjectWithTag("Player");
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    // DEATH
    // Loads the last save
    public void LoadCheckpoint()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.checkpointLevel != "")
        {
            audioManager.Play("ButtonNoise");
            playerController.OnRevive();
            GameManager.GetComponent<SaveLoad>().LoadCheckpoint();
        }
    }   
    
    // Fromm Pause Panel
    // Loads the last save
    public void LoadCheckpointFromPause()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            player.GetComponent<PlayerController>().paused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
        LoadCheckpoint();
    }

    // Restarts the game
    public void Restart()
    {
        audioManager.Play("ButtonNoise");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Starts the game
    public void StartNewGame()
    {
        audioManager.Play("ButtonNoise");
        loadingText.SetActive(true);
        SceneManager.LoadScene(HubLevelName);

    }

    public void StarFromCheckpoint()
    {
        audioManager.Play("ButtonNoise");
        loadingText.SetActive(true);
        GameManager.GetComponent<SaveLoad>().LoadCheckpoint();

    }

    // Starts the game
    public void QuitGame()
    {
        audioManager.Play("ButtonNoise");
        Application.Quit();

    }

    public void OpenCredits()
    {
        audioManager.Play("ButtonNoise");
        creditsPanel.SetActive(true);
        titlePanel.SetActive(false);
    }

    public void CloseCredits()
    {
        audioManager.Play("ButtonNoise");
        creditsPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    // Resets the currently selected crafting slot.
    private void DeemphasizeCurrentRecipe()
    {
        currentRecipeButton.interactable = true; // Re-enables the recipe's button.
        for (int i = 0; i < craftSlots.Length; i++)
        {
            craftSlots[i].ResetAmount();
        }
        ResetMiniSlots();
        currentRecipe = null;
        recipeComponents.Clear();
    }


   // Sets the most recently clicked-on crafting slot as the current recipe.
    public void SetCraftCurrent()
    {
        if (currentRecipe != null)
        {
            DeemphasizeCurrentRecipe();
        }
        audioManager.Play("ButtonNoise");
        currentRecipe = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        currentRecipeButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        currentRecipeButton.interactable= false;
        craftSlots = currentRecipe.GetComponentsInChildren<Slot>();
        miniSlots = miniInventoryParent.GetComponentsInChildren<Slot>();
    }

    // Selects a recipe component if part of the current recipe and not already selected.
    public void SelectRecipeComponent()
    {
        Slot currentMiniSlot = EventSystem.current.currentSelectedGameObject.GetComponent<Slot>(); // The slot script of the current mini slot.
        if (currentRecipe != null && currentMiniSlot.item != null)
        {
            if (!currentMiniSlot.isSelected)
            {
                audioManager.Play("ButtonNoise");
                int i = currentRecipe.GetComponent<CraftingSlot>().ComponentCheck(currentMiniSlot.item.elementNumber); // Gets the index of the matching recipe component in the crafting slot
                // If the clicked-on component is part of the current recipe and the required amount is more than zero, select the component for crafting.
                if (i != -1 && craftSlots[i].amount > 0) 
                    {
                        recipeComponents.Add(currentMiniSlot.item); // Adds the selected item to the list of other selected recipe components.
                        currentMiniSlot.GetComponent<Button>().interactable = false;
                        craftSlots[i].SubtractFromAmount(); // Subtracts the required amount for the selected item by one.
                        currentMiniSlot.isSelected = true;
                    }
               
            }
        }
    }

    // Crafts the ability if all the required amounts of components have been met.
    public void Craft()
    {
        if (currentRecipe.GetComponent<CraftingSlot>().isCompleted())
        {
            audioManager.Play("Pickup");
            Recipe recipe = currentRecipe.GetComponent<CraftingSlot>().recipe;
            player.GetComponent<PlayerController>().ActivateAbility(recipe);
            currentRecipeButton.interactable = true;
            currentRecipe = null;
            RemoveUsedItems();
            craft.RemoveCompletedRecipe(recipe);
        }
        else audioManager.Play("Negative");
    }

    // Exits the crafting interface; resets the crafting interface upon exiting.
    public void CancelCrafting()
    {   
        if (currentRecipe != null)
        {
            audioManager.Play("ButtonNoise");
            DeemphasizeCurrentRecipe();
            ResetMiniSlots();
            recipeComponents.Clear();
        }
    }

    // Resets all of the currently selected inventory mini slots.
    private void ResetMiniSlots()
    {
        for (int i = 0; i < miniSlots.Length; i++)
        {
            if (miniSlots[i].isSelected)
            {
                // miniSlots[i].gameObject.GetComponent<Image>().color = new Color (255f, 255f, 255f);
                miniSlots[i].GetComponent<Button>().interactable = true;
                miniSlots[i].isSelected = false;
            }

        }
    }

    // Removes the used recipe components upon crafting completion.
    private void RemoveUsedItems()
    {
        foreach (Item item in recipeComponents)
        {
            inventory.Remove(item);
        }
        ResetMiniSlots();
        recipeComponents.Clear();
    }
}
