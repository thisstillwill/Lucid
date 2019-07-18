using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMove : PressureTrigger
{
    // VARIABLES
    public Vector3 wishPosition; // desired position
    public float timeToLerp; // time to reach desired position
    private Vector3 startPosition; // starting position of target
    private float startTime; // time when lerping started
    private bool lerping; // is the target lerping?

    // overrides TriggerEffect() in PressureTrigger
    public override void TriggerEffect()
    {
        // initialize variables for lerp
        startTime = Time.time; // save time when lerp started
        startPosition = target.transform.localPosition; // save target start position
        lerping = true; // set flag for lerp to start
    }

    // perform interpolation
    private void FixedUpdate()
    {
        if (lerping)
        {
            // calculate percentage complete and lerp target based off that value
            float currentTime = Time.time - startTime;
            float percentageComplete = currentTime / timeToLerp;
            target.transform.localPosition = Vector3.Lerp(startPosition, wishPosition, percentageComplete);

            // stop lerping when complete
            if (percentageComplete >= 1.0f)
            {
                lerping = false;
            }
        }
    }
}