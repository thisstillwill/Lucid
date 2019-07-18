using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AguaCheck : MonoBehaviour
{
    public GameObject player;

    // Initialization
    void Awake()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point; 
        Vector3 start = new Vector3(0, 100, 0); // an arbitrary point far from the collider
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
            if (Physics.Linecast(point, goal, out hit))
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
            if (Physics.Linecast(point, start, out hit))
            {
                iterations++;
                point = hit.point + (-direction / 100.0f);
            }
            else
            {
                point = start;
            }
        }

        // camera is not submerged
        if (iterations % 2 == 0)
        {
            player.GetComponent<PlayerController>().submerged = true;
        }
        // camera is submerged
        if (iterations % 2 == 1)
        {
            player.GetComponent<PlayerController>().submerged = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            player.GetComponent<PlayerController>().inWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            player.GetComponent<PlayerController>().inWater = false;
        }
    }
}
