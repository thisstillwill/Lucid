using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WaterEffect : MonoBehaviour
{
    public LayerMask underwaterLayer;
    public LayerMask normalLayer;
    private GameObject player;
    private PostProcessLayer postProcessing;

    // Initialization
    private void Awake()
    {
        player = GameObject.Find("Player");
        postProcessing = GetComponent<PostProcessLayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().submerged)
        {
           postProcessing.volumeLayer = underwaterLayer;
        }
        else postProcessing.volumeLayer = normalLayer;
    }
}
