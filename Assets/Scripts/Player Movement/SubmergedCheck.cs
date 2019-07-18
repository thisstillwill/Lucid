using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmergedCheck : MonoBehaviour
{
    // VARIABLES
    public Vector3 start; // arbitrary point far from collider
    public LayerMask groundLayer;
    private AudioManager audioManager;
    public GameObject oceanSound;
    public GameObject secondLevelSound;
    public GameObject thirdLevelSound;
    private GameObject player; // reference to player game object

    // Start is called before the first frame update
    void Awake()
    {
        player = this.GetComponent<WaterTrigger>().player;
        this.enabled = false;
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point;
        Vector3 goal = Camera.main.transform.position; // goal point is the main camera's position
        Vector3 direction = goal - start; // the direction from start to goal
        direction.Normalize();
        int iterations = 0; // number of times raycast has hit faces on way to goal
        point = start;

        // pass through faces to try to reach goal point
        while (point != goal)
        {
            RaycastHit hit;
            // move point forward stopping on every face hit
            if (Physics.Linecast(point, goal, out hit, ~(1 << groundLayer.value)))
            {
                iterations++;
                point = hit.point + (direction / 100.0f); // move point to hit and push it slightly through mesh
            }
            else
            {
                point = goal; // if no obstruction then goal point can be reached in one step
            }
        }

        // pass through faces to try to return to start point (ensures all back faces are seen)
        while (point != start)
        {
            RaycastHit hit;
            if (Physics.Linecast(point, start, out hit, ~(1 << groundLayer.value)))
            {
                iterations++;
                point = hit.point + (-direction / 100.0f);
            }
            else
            {
                point = start;
            }
        }

        // camera is submerged
        if (iterations % 2 == 0)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (!controller.submerged)
            {
                player.GetComponent<PlayerController>().submerged = true;
                audioManager.Play("Underwater");
                oceanSound.SetActive(false);
                secondLevelSound.SetActive(false);
                thirdLevelSound.SetActive(false);
            }
        }

        // camera is not submerged
        if (iterations % 2 == 1)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller.submerged)
            {
                player.GetComponent<PlayerController>().submerged = false;
                audioManager.StopAudio("Underwater");
                oceanSound.SetActive(true);
                secondLevelSound.SetActive(true);
                thirdLevelSound.SetActive(true);
            }
        }
    }
}