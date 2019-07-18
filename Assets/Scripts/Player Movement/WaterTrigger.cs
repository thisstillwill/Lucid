using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    // VARIABLES
    public GameObject player; // reference to player gameobject
    public float waterDrag; // drag multiplier nonplayer objects experience in water
    public float rotDrag; // angular drag nonplayer objects experience in water
    public GameObject oceanSound;
    public GameObject secondLevelSound;
    public GameObject thirdLevelSound;
    private AudioManager AudioManager;

    // Initialization
    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        AudioManager = FindObjectOfType<AudioManager>();
    }

    // detect when player enters water
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            player.GetComponent<PlayerController>().inWater = true;
            this.GetComponent<SubmergedCheck>().enabled = true;
        }
        AudioManager.Play("Splash");
    }

    // detect when player or nonplayer object exits water
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            player.GetComponent<PlayerController>().inWater = false;
            player.GetComponent<PlayerController>().submerged = false; // guarantee
            this.GetComponent<SubmergedCheck>().enabled = false;
        }

        // reset angular drag of nonplayer objects
        else if (other.CompareTag("Physics")) other.GetComponent<Rigidbody>().angularDrag = 0.05f;

        AudioManager.Play("Splash");
        AudioManager.StopAudio("Underwater");
        oceanSound.SetActive(true);
        secondLevelSound.SetActive(true);
        thirdLevelSound.SetActive(true);
    }

    // alter drag of nonplayer objects in water
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Physics") && other.GetType() == typeof(BoxCollider))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>(); // cache rigidbody
            Vector3 objVelocity = rb.velocity; 
            rb.AddForce(-objVelocity * waterDrag, ForceMode.Force); // apply constant drag force
            rb.angularDrag = rotDrag;
        }
    }
}
