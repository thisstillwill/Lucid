using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureTrigger : MonoBehaviour
{
    // VARIABLES
    public GameObject target; // target object for pressure plate effect
    public GameObject trigger; // trigger object for pressure plate effect
    public bool triggered;
    private AudioManager AudioManager;

    private void Start()
    {
        AudioManager = FindObjectOfType<AudioManager>();
    }

    // used for all pressure plate interactions
    public virtual void TriggerEffect()
    {
        // This method is meant to be overriden when coding different interactions
    }

    // detect if object is on pressure plate
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() != typeof(SphereCollider) && !triggered)
        {
            // trigger effect when collding object matches parameters
            if (other.name == trigger.name)
            {
                triggered = true;
                AudioManager.Play("Positive");
                TriggerEffect();
            }
            else AudioManager.Play("Negative");
        }
    }
}
