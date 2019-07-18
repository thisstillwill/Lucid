using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public static class SaveSystem
{

    // FOR SAVING BETWEEN LEVELS/WORLDS...  
    // Saves the essential saves essential player.
    public static void SavePlayer(GameObject player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player";
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // Loads the essential player data.
    public static PlayerData LoadPlayer()
    {
        // Loads the essential player data.
        string playerPath = Application.persistentDataPath + "/player";
        if (File.Exists(playerPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream playerStream = new FileStream(playerPath, FileMode.Open);

            PlayerData playerData = (PlayerData)formatter.Deserialize(playerStream);
            playerStream.Close();

            return playerData;
        }
        else
        {
            Debug.LogError("Save file not found in " + playerPath);
            return null;
        }
    }

    // Saves current level data
    public static void SaveCurrentLevel(GameObject GameManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string levelPath = Application.persistentDataPath + "/level." + SceneManager.GetActiveScene().name; // Make this file depend on the level
        FileStream levelStream = new FileStream(levelPath, FileMode.Create);

        LevelData levelData = new LevelData(GameManager);

        formatter.Serialize(levelStream, levelData);
        levelStream.Close();
    }

    // Loads any level data based on the given string
    public static LevelData LoadLevel(string levelName)
    {
        string levelPath = Application.persistentDataPath + "/level." + levelName; // Make this file depend on the level
        if (File.Exists(levelPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream levelStream = new FileStream(levelPath, FileMode.Open);

            LevelData levelData = (LevelData)formatter.Deserialize(levelStream);
            levelStream.Close();

            return levelData;
        }
        else
        {
            return null;
        }
    }


    // FOR CHECKPOINT SAVING AND LOADING...
    // Saves all of the essential player data, checkpoint location, and all the level states at the moment of saving at a checkpoint.
    public static void SaveCheckpoint(GameObject player, GameObject GameManager, string[] levelNames)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        // Saves the essential player data.
        string playerPath = Application.persistentDataPath + "/checkpoint.player"; // Make this file depend on the level
        FileStream playerStream = new FileStream(playerPath, FileMode.Create);
        PlayerData playerData = new PlayerData(player);
        formatter.Serialize(playerStream, playerData);
        playerStream.Close();

        // Saves checkpoint data
        string checkpointPath = Application.persistentDataPath + "/checkpoint.data"; // Make this file depend on the level
        FileStream checkpointStream = new FileStream(checkpointPath, FileMode.Create);
        CheckpointData checkpointData = new CheckpointData(player);
        formatter.Serialize(checkpointStream, checkpointData);
        checkpointStream.Close();

        // Saves the current world state
        string currentLevelName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (levelNames[i] == currentLevelName)
            {
                string levelPath = Application.persistentDataPath + "/checkpoint." + currentLevelName;
                FileStream levelStream = new FileStream(levelPath, FileMode.Create);
                LevelData levelData = new LevelData(GameManager);
                formatter.Serialize(levelStream, levelData);
                levelStream.Close();
            }
            else
            {
                LevelData otherLevelData = LoadLevel(levelNames[i]);
                if (otherLevelData != null)
                {
                    string levelPath = Application.persistentDataPath + "/checkpoint." + levelNames[i];
                    FileStream levelStream = new FileStream(levelPath, FileMode.Create);
                    formatter.Serialize(levelStream, otherLevelData);
                    levelStream.Close();
                }
                else
                {
                    string levelPath = Application.persistentDataPath + "/checkpoint." + levelNames[i];
                    DeleteFile(levelPath);
                }
               
            }
        }
    }

    // Loads and returns the essential player data that was saved at the checkpoint.
    public static PlayerData LoadCheckpointPlayer()
    {
        // Loads the essential player data.
        string playerPath = Application.persistentDataPath + "/checkpoint.player";
        if (File.Exists(playerPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream playerStream = new FileStream(playerPath, FileMode.Open);

            PlayerData playerData = (PlayerData)formatter.Deserialize(playerStream);
            playerStream.Close();

            return playerData;
        }
        else
        {
            Debug.LogError("Save file not found in " + playerPath);
            return null;
        }
    }

    // Loads and returns the checkpoint location and terrain data.
    public static CheckpointData LoadCheckpointData()
    {
        // Loads the checkpoint data.
        string checkpointPath = Application.persistentDataPath + "/checkpoint.data";
        if (File.Exists(checkpointPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream checkpointStream = new FileStream(checkpointPath, FileMode.Open);

            CheckpointData checkpointData = (CheckpointData) formatter.Deserialize(checkpointStream);
            checkpointStream.Close();

            return checkpointData;
        }
        else
        {
            Debug.LogError("Save file not found in " + checkpointPath);
            return null;
        }
    }

    // Loads and returns the level state of the checkpoint level.
    public static LevelData LoadCheckpointLevel(string levelName)
    {
        // Loads the checkpoint data.
        string levelPath = Application.persistentDataPath + "/checkpoint." + levelName;
        if (File.Exists(levelPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream levelStream = new FileStream(levelPath, FileMode.Open);
            LevelData levelData = (LevelData) formatter.Deserialize(levelStream);
            levelStream.Close();

            return levelData;
        }
        else
        {
            Debug.LogError("Save file not found in " + levelPath);
            return null;
        }
    }

    // Overrides the level saves of all other levels to the states at the moment of checkpoint saving.
    public static void RestoreWorldState(string levelName)
    {
        string oldPath = Application.persistentDataPath + "/checkpoint." + levelName;
        if (File.Exists(oldPath))
        {
            // Gets the checkpoint level state
            BinaryFormatter oldFormatter = new BinaryFormatter();
            FileStream oldStream = new FileStream(oldPath, FileMode.Open);
            if (oldStream.Length != 0)
            {
                LevelData oldLevelData = (LevelData) oldFormatter.Deserialize(oldStream);
                oldStream.Close();

                // Overrides the current level state with the checkpoint level state;
                BinaryFormatter newFormatter = new BinaryFormatter();
                string levelPath = Application.persistentDataPath + "/level." + levelName;
                FileStream levelStream = new FileStream(levelPath, FileMode.Create);

                newFormatter.Serialize(levelStream, oldLevelData);
                levelStream.Close();
            }
            else oldStream.Close();
        }
    }

    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
