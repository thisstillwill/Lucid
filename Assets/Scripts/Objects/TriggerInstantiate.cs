using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInstantiate : PressureTrigger
{
    // VARIABLES
    public Vector3 offset;
    public Vector3 angle;

    // overrides TriggerEffect() in PressureTrigger
    public override void TriggerEffect()
    {
        Instantiate(target, transform.position + offset, Quaternion.Euler(angle));
    }
}
