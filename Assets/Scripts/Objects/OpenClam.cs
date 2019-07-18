using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClam : Interactable
{
    public float rotation;
    public float speed;
    public override void Interact()
    {
        if (interactDisplay.activeInHierarchy)
        {
            interactDisplay.SetActive(false);
        }
        continuous = true;
        float angle = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, rotation, speed * Time.deltaTime);
        // transform.eulerAngles = new Vector3(rotation, 0, 0);
        transform.Rotate(new Vector3(angle, 0, 0), Space.Self);
        if (transform.localEulerAngles.x > 350 + rotation && transform.localEulerAngles.x < 370 + rotation)
        {
            continuous = false;
            isInteracted = false;
            GetComponent<OpenClam>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            canInteract = false;
            AudioManager.Play("Open");
        }
    }
}
