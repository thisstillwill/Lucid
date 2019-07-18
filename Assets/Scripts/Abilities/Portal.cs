using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject portalScreen;
    public string sceneToLoad;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SaveSystem.SavePlayer(other.gameObject);
            SaveSystem.SaveCurrentLevel(GameManager);
            portalScreen.SetActive(true);
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.paused = true;
            Time.timeScale = 0f;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
