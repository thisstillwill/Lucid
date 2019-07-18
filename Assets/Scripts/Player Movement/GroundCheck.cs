using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    // Checks if the given capsule is grounded to the given layer mask.
    public bool isGrounded(Vector3 top, Vector3 bottom, float radius, LayerMask groundMask)
    {
        bool grounded = Physics.CheckCapsule(top, bottom, radius, groundMask);
        return grounded;
    }
}
