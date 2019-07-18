using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabUpdate : MonoBehaviour
{
    // VARIABLES
    public float throwForce; // force object is thrown
    public float holdOffset; // distance object is held from camera
    public float moveSmooth; // smooth value for object translation
    public float rotSmooth; // smooth value for object rotation

    public LayerMask ground;
    private Camera cam; // reference to main camera
    private GameObject player; // reference to player gameobject
    private Vector3 wishPosition; // desired position of object
    private bool beingCarried; // is object being carried?
    private bool touched; // is object touching a surface?


    // Initialization
    private void Awake()
    {
       player = GameObject.Find("Player");
       cam = Camera.main;
    }

    private void Update()
    {
        // throw  object by applying a force in reference to  camera's transform
        if (beingCarried && Input.GetMouseButtonDown(1))
        {
            Drop();
            GetComponent<Rigidbody>().AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // check if  object is being carried from interact script
        beingCarried = gameObject.GetComponent<GrabInteract>().beingCarried;

        if (beingCarried)
        {
            // don't carry the object through walls
            if (!Obstructed()) Carry();
            else Drop();

            // disables sphere collider when submerged to allow for swimming
            if (Input.GetButton("Jump") || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && player.GetComponent<PlayerController>().submerged)
            {
                GetComponent<SphereCollider>().enabled = false;
            }
            else GetComponent<SphereCollider>().enabled = true;
        }
        else if (!beingCarried)
        {
            Drop();
        }
    }

    void Carry()
    {
        // update wish position based on current screen dimensions and  camera's position in the world
        int x = Screen.width / 2;
        int y = Screen.height / 2;
        wishPosition = cam.ScreenToWorldPoint(new Vector3(x, y, cam.nearClipPlane + holdOffset));

        // disable object gravity and cancel its velocity
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // if object is touching something rotate it to match player gameobject's rotation
        if (touched)
        {
            transform.position = Vector3.Lerp(transform.position, wishPosition, Time.deltaTime * moveSmooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, player.transform.rotation, Time.deltaTime * rotSmooth);
        }

        // otherwise rotate object to match camera's rotation
        if (!touched)
        {
            transform.position = Vector3.Lerp(transform.position, wishPosition, Time.deltaTime * moveSmooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, cam.transform.rotation, Time.deltaTime * rotSmooth);
        }
    }


    private void Drop()
    {
        gameObject.GetComponent<GrabInteract>().beingCarried = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        this.enabled = false; // disable script when not needed
    }

    private void OnCollisionEnter(Collision collision)
    {
        // drop object if in contact with player
        if (collision.gameObject.name == "Player")
        {
            Drop();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && beingCarried) {
            touched = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            touched = false;
        }
    }

    // returns true if no line-of-sight path between object and camera
    public bool Obstructed()
    {
        RaycastHit hit;
        Physics.Linecast(transform.position, cam.transform.position, out hit, ~(1 << ground.value));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }
        return true;
    }
}
