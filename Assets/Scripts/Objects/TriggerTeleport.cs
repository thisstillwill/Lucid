using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTeleport : PressureTrigger
{
    // VARIABLES
    public Vector3 position;
    public Vector3 rotation;

    // overrides TriggerEffect() in PressureTrigger
    public override void TriggerEffect()
    {
        target.transform.position = position;
        target.transform.eulerAngles = rotation;
        triggered = false;
    }
}
