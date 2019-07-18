using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : Interactable
{
    

    public GameObject craftingPanel;
    public override void Interact()
    {
        // This can be used if you implement anything in the Interact() function in Interactable.cs
        // base.Interact();
        if (!craftingPanel.activeInHierarchy)
        {
            interactDisplay.SetActive(false);
            craftingPanel.SetActive(true);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.crosshair.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerController.paused = true;
            AudioManager.Play("Inventory");
        }
    }
}
