using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabInteract : Interactable
{
    // VARIABLES
    public bool beingCarried; // is object being carried?

    // Initialization
    private void Awake()
    {
        beingCarried = false;
    }

    private void Start()
    {
        AudioManager = FindObjectOfType<AudioManager>();
    }

    // overrides Interact() in Interactable
    public override void Interact()
    {
        if (player.GetComponent<PlayerController>().PlayerCanGrab())
        {
            // carry the object
            if (!beingCarried)
            {
                beingCarried = true;
                GetComponent<GrabUpdate>().enabled = true; // enable update script
                AudioManager.Play("Grab");
            }

            // drop the object if already carrying it
            else if (beingCarried)
            {
                beingCarried = false;
            }
        } 
    }
}
