using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFollow : MonoBehaviour
{
    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
    }
}
