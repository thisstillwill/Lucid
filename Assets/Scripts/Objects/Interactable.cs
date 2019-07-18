using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isInteracted = false; // is something interacted with?
    public bool canInteract = false; // can player interact with something?
    public bool continuous = false; // Is this interaction continuous?
    public GameObject player; // reference to player gameobject
    public GameObject interactDisplay; // reference to interact display gameobject

    public AudioManager AudioManager;

    private void Start()
    {
        AudioManager = FindObjectOfType<AudioManager>();
    }

    public bool isMoveable; // for save functionality

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Used for all object interactions.
    public virtual void Interact()
    {
        // This method is meant to be overriden when coding different interactions.
    }
    
    // Calls the interact function if the object can be interacted with and if the player has pressed E to interact with it.
    void Update()
    {
        if (continuous)
        {
            isInteracted = true;
            canInteract = true;
        }
        if (isInteracted && canInteract)
        {
            Interact();
            if (!continuous)
            {
                // reset isInteracted and canInteract to allow for object pickup interaction
                isInteracted = false;
                canInteract = true;
            }
        }
    }

    // Allows player to interact with an object if the player is within the radius of the Object's sphere collider.
    // Displays the Interaction Bubble when near the object.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().nearInteractable = gameObject;
            if (!interactDisplay.activeInHierarchy)
            {
                interactDisplay.SetActive(true);
                canInteract = true;
            }
        }
        if (other.gameObject.tag == "Hook")
        {
            other.GetComponent<HookDetector>().player.GetComponent<GrapplingHook>().hooked = true;
        }
    }

    // Disallows player interaction once the player leaves the collider radius.
    // Deactivates the Interaction Bubble.
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().nearInteractable = null;
            if (interactDisplay.activeInHierarchy)
            {
                interactDisplay.SetActive(false);
                canInteract = false;
            }
        }
    }

    // Called in player controller when the player presses E and is next to an interactable object.
    public void OnInteraction ()
    {
        isInteracted = true;
    }
}
