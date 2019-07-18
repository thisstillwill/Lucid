using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : Interactable
{
    public float rotation;

    public override void Interact()
    {
        continuous = true;
        Quaternion newRotation = Quaternion.AngleAxis(rotation, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, .05f);
        if (transform.localEulerAngles.y > (rotation - 2f))
        {
            continuous = false;
            isInteracted = false;
            interactDisplay.SetActive(false);
            GetComponent<OpenDoor>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            canInteract = false;
            AudioManager.Play("Open");
        }
    }
}
