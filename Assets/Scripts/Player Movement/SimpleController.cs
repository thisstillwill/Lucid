using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    // BASIC PLAYER VARIABLES
    // movement

    public GameObject winScreen;
    public float walkSpeed;

    // mouselook and camera variables
    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;


    public float yAngle; // Y angle for mouse look
    public float xAngle; // X angle for player rotation

    private GameObject player;

    public bool paused = false;

    private Rigidbody rb;
    private Camera cam;

    // Initialization
    void Awake()
    {
        // initialize instance variables
        player = this.gameObject; // store reference to the player's gameobject (mostly for semantics)
        rb = this.GetComponent<Rigidbody>(); // store reference to the player's rigidbody
        cam = Camera.main; // store reference to main camera
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        paused = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // If the game is NOT paused, continue with player movement and input.
        if (!paused)
        {
            // get input for movement;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // MOUSELOOK
            // get input for mouselook 
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            // calculate mouselook and clamp it on the y axis
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);
            mouseLook += smoothV;
            mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);
            yAngle = mouseLook.y;
            xAngle = mouseLook.x;


            // CURSOR
            // release cursor from game
            if (Input.GetKeyDown("escape"))
                Cursor.lockState = CursorLockMode.None;
        }
    }


    void FixedUpdate()
    {
        if (!paused)
        {
            // MOUSELOOK
            // use mouselook vector to transform player and cam
            cam.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
            // MOVEMENT AND SWIMMING
            // move the player's position constantly with respect to time.
            rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * walkSpeed * Time.deltaTime) + (transform.right * Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime));
        }
    }

    void OnTriggerEnter()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        winScreen.SetActive(true);
    }
}

