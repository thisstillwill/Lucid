using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{

    public GameObject player;
    public bool checkpointLoad;
    private bool gameStart;

    public string HubLevelName;
    public string FirstLevelName;
    public string SecondLevelName;
    public string ThirdLevelName;

    public string[] levelNames;
    // Start is called before the first frame update
   

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // If the player is loading a checkpoint...
        if (checkpointLoad)
        {
            CheckpointRestore();
        }
        // If the player is not loading a checkpoint but has loaded a new level...
        else
        {
            // Maintains the essential data like inventory and available recipes between the levels.
            PlayerData playerData = SaveSystem.LoadPlayer();
            if (playerData != null)
            {
                RestoreData(playerData);
            }
          
            // Loads the level state of the level being loaded.
            LevelData levelData = SaveSystem.LoadLevel(SceneManager.GetActiveScene().name);
            if (levelData != null)
            {
                RestoreCurrentLevel(levelData);
            }
        }
    }

    // called third
    void Start()
    {
        levelNames = new string[2];
        levelNames[0] = HubLevelName;
        levelNames[1] = FirstLevelName;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Deletes the between-level saves when the game is closed
    void OnApplicationQuit()
    {
        DeleteTempFiles();
    }


    public void SaveCheckpoint()
    {
        SaveSystem.SaveCheckpoint(player, gameObject, levelNames);
    }

    public void LoadCheckpoint()
    {
        PlayerData data = SaveSystem.LoadCheckpointPlayer();
        if (SceneManager.GetActiveScene().name == data.checkpointLevel)
        {
            CheckpointRestore();
        }
        else
        {
            SceneManager.LoadScene(data.checkpointLevel);
            checkpointLoad = true;
        }
    }


    private void RestoreData(PlayerData data)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.checkpointLevel = data.checkpointLevel;

        playerController.RestoreAbilities(data);
        if (data.canGrapple)
        {
            player.GetComponent<GrapplingHook>().enabled = true;
        }

        // Restores the inventory
        Inventory inventory = Inventory.instance;
        inventory.RestoreInventory(data.inventoryList);

        // Restores the crafting interface
        Crafting crafting = Crafting.instance;
        crafting.RestoreCrafting(data.availableRecipes);

    }

    private void ToCheckpoint(CheckpointData data)
    {
       // Resets player position
        Vector3 playerPosition = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        player.transform.position = playerPosition;

       // Resets Camera position
        Vector3 cameraPosition = new Vector3(data.camPosition[0], data.camPosition[1], data.camPosition[2]);
        Camera.main.transform.position = cameraPosition;

        //Restores terrain status
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.grounded = data.grounded;
        playerController.inWater = data.inWater;
        playerController.submerged = data.submerged;
    }

    private void RestoreCurrentLevel(LevelData level)
    {
        if (level != null)
        {
            GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
            ObjectSave[] worldObjects = GameManager.GetComponentsInChildren<ObjectSave>(true);
            if (worldObjects.Length != 0 && worldObjects != null)
            {
                int j = 0;
                for (int i = 0; i < worldObjects.Length; i++)
                {
                    worldObjects[i].gameObject.SetActive(level.activeStates[i]);
                    if (worldObjects[i].isMovable)
                    {
                        Vector3 position = new Vector3(level.xPositions[j], level.yPositions[j], level.zPositions[j]);
                        worldObjects[i].gameObject.transform.position = position;
                        j++;
                    }
                }
            }
        }
    }

    private void CheckpointRestore()
    {
        // Restores the player data to what it was at the moment of saving with the checkpoint.
        PlayerData playerData = SaveSystem.LoadCheckpointPlayer();
        RestoreData(playerData);

        // Restores the player's position to what it was at the moment of saving with the checkpoint.
        CheckpointData checkpointData = SaveSystem.LoadCheckpointData();
        ToCheckpoint(checkpointData);

        // Goes through each of the levels in the build...
        for (int i = 0; i < levelNames.Length; i++)
        {
            // Restores the level state for the level in which the player saved at a checkpoint.
            if (playerData.checkpointLevel == levelNames[i])
            {
                LevelData checkpointLevelData = SaveSystem.LoadCheckpointLevel(playerData.checkpointLevel);
                RestoreCurrentLevel(checkpointLevelData);
            }
            // For the other levels, overrides the level saves with the saved data at the moment of saving at a checkpoint.
            else
            {
                SaveSystem.RestoreWorldState(levelNames[i]);
            }
        }
        checkpointLoad = false; // prevents bugs of always checkpoint loading.
    }

    public void DeleteTempFiles()
    {
        for (int i = 0; i < levelNames.Length; i++)
        {
            string levelPath = Application.persistentDataPath + "/level." + levelNames[i];
            SaveSystem.DeleteFile(levelPath);
        }
        string playerPath = Application.persistentDataPath + "/player";
        SaveSystem.DeleteFile(playerPath);
    }
}
