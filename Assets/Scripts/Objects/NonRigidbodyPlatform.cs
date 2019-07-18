using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonRigidbodyPlatform : MonoBehaviour
{
    public Vector3 end; // ending position
    public float delay; // time platform waits between cycles
    public float timeToLerp; // time to reach desired position
    public float power; // power to raise percentage by
    private Vector3 start; // starting position
    private float startTime; // time when lerping started
    private bool forth; // is the platform moving forward?
    private bool back; // is the platform moving backwards
    private bool waiting; // is coroutine running?

    // Initialization
    private void Start()
    {
        start = transform.position; // save the platform's starting position
        forth = true; // platform starts by moving forward
        StartCoroutine(Wait()); // platform starts waiting on first cycle
    }

    // move platform back and forth
    private void FixedUpdate()
    {
        if (!waiting)
        {
            if (forth)
            {
                // calculate percentage complete and lerp target based off that value
                float currentTime = Time.time - startTime;
                float percentageComplete = currentTime / timeToLerp;
                transform.position = Vector3.Lerp(start, end, Mathf.Pow(percentageComplete, power));

                // stop lerping when complete
                if (percentageComplete >= 1.0f)
                {
                    forth = false;
                    back = true;
                    StartCoroutine(Wait()); // start waiting
                }
            }
            else if (back)
            {
                // calculate percentage complete and lerp target based off that value
                float currentTime = Time.time - startTime;
                float percentageComplete = currentTime / timeToLerp;
                transform.position = Vector3.Lerp(end, start, Mathf.Pow(percentageComplete, power));

                // stop lerping when complete
                if (percentageComplete >= 1.0f)
                {
                    back = false;
                    forth = true;
                    StartCoroutine(Wait()); // start waiting
                }
            }
        }
    }

    private void ResetTime()
    {
        startTime = Time.time;
    }

    // wait for set amount of time
    IEnumerator Wait()
    {
        waiting = true; // set flag that platform should not move
        yield return new WaitForSeconds(delay); // wait delay seconds
        waiting = false; // set flag that platform should now move
        ResetTime(); // reset time elapsed for lerp
    }

    private void OnTriggerStay(Collider other)
    {
        // other.transform.parent = transform;
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().onPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // other.transform.parent = null;
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().onPlatform = false;
        }
    }
}
