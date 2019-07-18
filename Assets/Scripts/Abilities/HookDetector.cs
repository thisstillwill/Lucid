using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour
{
    public GameObject player;
    private HingeJoint grabHinge;
    private Rigidbody rb;
    private bool hooked;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //To shoot your hook, call this method:
    void OnCollisionEnter(Collision col)
    {
            rb.velocity *= 0;
            player.GetComponent<GrapplingHook>().hooked = true;
    }
}
