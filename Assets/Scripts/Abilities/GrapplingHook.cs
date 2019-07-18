using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{

    public GameObject hook;
    public GameObject hookHolder;

    public float hookSpeed;
    public float playerSpeed;

    public float maxDistance;
    public float returnSpeed;


    public bool hooked;
    private bool fired;

    private float currentDistance;
    private Vector3 firingPosition;
    private Vector3 direction;

    private Rigidbody hookRB;
    private Rigidbody rb;

    private RaycastHit hitPoint;


    // Start is called before the first frame update
    void Start()
    {
        hookRB = hook.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Firing the hook.
        if (Input.GetMouseButtonDown(0) && !fired && !GetComponent<PlayerController>().paused && !GetComponent<PlayerController>().submerged)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag != "Water") 
                {
                hitPoint = hit;
                hook.SetActive(true);
                hook.transform.position = hookHolder.transform.position;
                hook.transform.rotation = Camera.main.transform.rotation;
                firingPosition = hook.transform.position;
                hookRB.position = Vector3.MoveTowards(hook.transform.position, hit.point, 1);
                fired = true;
                rb.useGravity = false;

            }
        }
    }

    void FixedUpdate() {
        // The renderer for the grappling hook rope.
        if (fired)
       {
           LineRenderer rope = hook.GetComponent<LineRenderer>();
           rope.SetPosition(0, hookHolder.transform.position);
           rope.SetPosition(1, hook.transform.position);
       } 
        // Moves the hook until it collides with an object or reaches the max distance from the player.
        if (fired && !hooked)
        {
            if (currentDistance < maxDistance)
            {
                hookRB.position = Vector3.MoveTowards(hook.transform.position, hitPoint.point, 1);

                float fireDist = Vector3.Distance(firingPosition, hookRB.position);
                float playDist = Vector3.Distance(transform.position, hookRB.position);
                if (playDist < fireDist)
                {
                    currentDistance = fireDist;
                }
                else currentDistance = playDist;
            }
            else
            {
                ReturnHook();
            }
        }
        // Moves the player towards the hook until the distance between the hook and the player is less than 1.
       if (hooked)
        {
            transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, 1) * playerSpeed;
            rb.useGravity = false;
            if (Vector3.Distance(transform.position, hook.transform.position) < 1)
            {
                rb.velocity *= 0;
                currentDistance = 0;
                hook.SetActive(false);
                fired = false;
                hooked = false;
                rb.useGravity = true;
            }
        }
        else
        {
            if (!GetComponent<PlayerController>().submerged) this.GetComponent<Rigidbody>().useGravity = true;
        } 
    }

    // Moves the hook back towards the player and deactivates the hook once it is closer than the size of the returnSpeed.
        void ReturnHook()
    {
        if (Vector3.Distance(hook.transform.position, hookHolder.transform.position) > returnSpeed) {
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, hookHolder.transform.position, returnSpeed);
                }
        else
        {
            currentDistance = 0;
            hook.SetActive(false);
            fired = false;
            hooked = false;
            rb.useGravity = true;
        }
    }
}
