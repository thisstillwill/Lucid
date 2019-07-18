using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomText : MonoBehaviour
{
    public string[] messages = {"Nighty night", "Time for bed", "Sweet dreams", "...ZZZzzz..." };

    private void Awake()
    {
        this.enabled = false;
    }

    private void Start()
    {
        this.GetComponent<Text>().text = messages[Random.Range(0, messages.Length)];
    }
}
