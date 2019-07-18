using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelData
{
    // Lastest checkpoint save
    public string levelName;

    // Current level state
    public bool[] activeStates;
    public float[] xPositions;
    public float[] yPositions;
    public float[] zPositions;

    public LevelData(GameObject GameManager)
    {
        levelName = SceneManager.GetActiveScene().name;

        ObjectSave[] worldObjects = GameManager.GetComponentsInChildren<ObjectSave>(true);
        activeStates = new bool[worldObjects.Length];
        List<Vector3> listOfMoveables = new List<Vector3>();
        for (int i = 0; i < worldObjects.Length; i++)
        {
            if (worldObjects[i].gameObject.activeSelf) activeStates[i] = true;
            if (worldObjects[i].isMovable)
            {
                listOfMoveables.Add(worldObjects[i].gameObject.transform.position);
            }
        }
        xPositions = new float[listOfMoveables.Count];
        yPositions = new float[listOfMoveables.Count];
        zPositions = new float[listOfMoveables.Count];

        for (int i = 0; i < listOfMoveables.Count; i++)
        {
            Vector3 currentPosition = listOfMoveables[i];
            xPositions[i] = currentPosition.x;
            yPositions[i] = currentPosition.y;
            zPositions[i] = currentPosition.z;
        }
    }
}
