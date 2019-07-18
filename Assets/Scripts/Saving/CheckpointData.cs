using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckpointData
{
    // Lastest checkpoint save
    public string checkpointLevel;

    public float[] playerPosition;
    public float[] camPosition;

    // Terrain status
    public bool inWater;
    public bool submerged;
    public bool grounded;

    // Recall
    public bool recallActive;
    public float[] recallPoint;


    // Portal generator
    public bool portalActive;
    public float[] portalPosition;


    public CheckpointData(GameObject player)
    {
        // Necessary variables for saving
        PlayerController playerController = player.GetComponent<PlayerController>();

        // Checkpoint level
        checkpointLevel = playerController.checkpointLevel;

        // Saves the players position
        playerPosition = new float[3];
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = player.transform.position.y;
        playerPosition[2] = player.transform.position.z;

        // Saves the camera position and rotation
        camPosition = new float[3];
        camPosition[0] = Camera.main.transform.position.x;
        camPosition[1] = Camera.main.transform.position.y;
        camPosition[2] = Camera.main.transform.position.z;

        recallActive = playerController.IsRecallActive();
        if (recallActive)
        {
            recallPoint = playerController.RecallPoint();
        }


        portalActive = playerController.IsPortalActive();
        if (portalActive)
        {
            portalPosition = playerController.PortalPosition();
        }

        inWater = playerController.inWater;
        submerged = playerController.submerged;
        grounded = playerController.grounded;

    }
}