using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : Interactable
{

    public override void Interact()
    {
        player.GetComponent<PlayerController>().checkpointLevel = SceneManager.GetActiveScene().name;
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameManager.GetComponent<SaveLoad>().SaveCheckpoint();
        interactDisplay.SetActive(false);
        canInteract = false;
        AudioManager.Play("Save");
    }

}
