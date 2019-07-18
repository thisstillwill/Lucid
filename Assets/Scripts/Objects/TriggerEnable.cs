using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnable : PressureTrigger
{
    // overrides TriggerEffect() in PressureTrigger
    public override void TriggerEffect()
    {
        target.SetActive(true);
    }
}
