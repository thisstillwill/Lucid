using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelThemeFollow : MonoBehaviour
{
    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        gameObject.transform.position = player.transform.position;
        if (player.GetComponent<PlayerController>().submerged) GetComponent<AudioLowPassFilter>().enabled = true;
        else GetComponent<AudioLowPassFilter>().enabled = false;
    }
}
