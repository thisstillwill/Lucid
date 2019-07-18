using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public GameObject GameManager;
    private AudioManager AudioManager;
	public string checkpointLevel;
	public bool loadingCheckpoint;
    public GameObject pausePanel;

	// BASIC PLAYER VARIABLES
	// movement
	public float walkSpeed;
	public float sprintSpeed;
	private float move;
    public LayerMask ground;
	public bool grounded;
	public bool onPlatform;
	private Vector3 centerTop;
	private Vector3 centerBottom;
	public bool launched;

    public float currentY;
    public float yThreshold;

	// mouselook and camera variables
	Vector2 mouseLook;
	Vector2 smoothV;
	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;
	public float tiltSpeed;
	public float zBack; // speed that tilt reverses
	public float maxTilt;
	public float angleSnap;
	private float zRotation; // tilt amount

	public float yAngle; // Y angle for mouse look
	public float xAngle; // X angle for player rotation

	// DEATH
	// player death conditions
	public Canvas FadeOut;
	public GameObject DeathPanel;
	public float yMin; // minimum allowed y position;

    // Interactables
    public GameObject nearInteractable;
	private GameObject player;

	// For activating inventory 
	public bool paused = false;
	public GameObject inventoryPanel;
	public GameObject crosshair;

    // ABILITY VARIABLES
    // Activate abilities
    private bool canJump;
    private bool canWallJump;
    private bool canGrab;
    private bool canSwim;
    private bool canPortalWarp;
    private bool canRecall;
	private bool canGrapple;

    private GameObject currentActiveAbilityPanel;
    public GameObject unlockJumpPanel;
    public GameObject unlockWallJumpPanel;
    public GameObject unlockGrabPanel;
    public GameObject unlockSwimmingPanel;
    public GameObject unlockPortalGeneratorPanel;
    public GameObject unlockRecallPanel;
    public GameObject unlockGrapplingHookPanel;


	// jumping and wall jumping
	[Range(1, 20)]
	public float jumpVelocity;
	[Range(1, 20)]
	public float gravity; // overall gravity multiplier
	[Range(1, 20)]
	public float upG; // additional gravity multiplier when not holding jump
	[Range(1, 20)]
	public float downG; // additional gravity multiplier when falling
	public string wallName; // name of last wall collided with

	// swimming
	public bool inWater;
	public bool submerged;
	public float swimSpeed;

	// Recall
	private bool recallActive;
	private Vector3 recallPoint;
	public GameObject recall;
	public GameObject recallPanel;

	// Portal generator
	private bool portalActive;
	public GameObject portal;

	// Physics grab
	private Rigidbody rb;
	private Camera cam;

	// Initialization
	void Awake()
	{
		// initialize instance variables
		player = this.gameObject; // store reference to the player's gameobject (mostly for semantics)
		rb = this.GetComponent<Rigidbody>(); // store reference to the player's rigidbody
		cam = Camera.main; // store reference to main camera
		move = walkSpeed;
		// lock cursor
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		Time.timeScale = 1f;
		paused = false;

		FadeOut.enabled = false;
        AudioManager = FindObjectOfType<AudioManager>();
	}

	// Update is called once per frame
	private void Update()
	{
		// If the game is NOT paused, continue with player movement and input.
		if (!paused)
		{
            if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButton(0))
                {
                 Cursor.lockState = CursorLockMode.Locked;
                 Cursor.visible = false;
                }
            }

			// DEATH
			// kill the player if they attempt to swim 
			if (inWater && !canSwim) Death();
			// kill the player if they fall off the level
			if (transform.position.y < yMin) Death();

			// MOVEMENT
			// update if player is grounded
			centerBottom = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);
			centerTop = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			grounded = gameObject.GetComponent<GroundCheck>().isGrounded(centerTop, centerBottom, 0.45f, ground);
            if (grounded) launched = false; // allow for normal air physics when grounded
            

			// get input for movement;
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

            // calculate player's movement multiplier, accounting for whether sprint is enabled
            if (grounded || inWater)
            {
                if (Input.GetKey(KeyCode.LeftShift) && (vertical > 0) || Input.GetKey(KeyCode.RightShift) && (vertical > 0))
                {
                    move = sprintSpeed;
                }
                else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                {
                    move = walkSpeed;
                }
            }
            else if (launched) move = walkSpeed;

			// JUMPING
			// apply upG multiplier when pressing jump from ground
			if (Input.GetButtonDown("Jump") && grounded && !submerged && canJump)
			{
				rb.velocity = Vector3.up * jumpVelocity;
				rb.velocity += Vector3.up * Physics.gravity.y * (upG / 2) * gravity * Time.deltaTime;
				wallName = "";
				grounded = false;
                AudioManager.Play("Jump");
			}

			// Applies the square of the down multiplier if on the platform and the jump button is not pressed (to prevent upward velocity shooting the player off the platform).
			if (onPlatform && !Input.GetButton("Jump"))
			{
				rb.velocity += Vector3.up * Physics.gravity.y * downG * downG * gravity * Time.deltaTime;
			}
			// apply downG multiplier when falling
			else if (rb.velocity.y < 0 && !submerged)
			{
				rb.velocity += Vector3.up * Physics.gravity.y * (downG / 2) * gravity * Time.deltaTime;
			}
			// apply upG multiplier when positive y velocity but not holding jump or when player is launched
			else if (rb.velocity.y > 0 && (!Input.GetButton("Jump") && !submerged && canJump) || launched)
			{
				rb.velocity += Vector3.up * Physics.gravity.y * (upG / 2) * gravity * Time.deltaTime;
			}

			// SWIMMING
			// allow the player to jump at the surface of the water if they are not grounded 
			if (Input.GetButton("Jump") && inWater && !grounded && !submerged && canSwim)
			{
				rb.velocity = Vector3.up * jumpVelocity;
				rb.velocity += Vector3.up * Physics.gravity.y * (upG / 2) * gravity * Time.deltaTime;
				wallName = "";
            }


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
			if (Input.GetKeyDown("escape") && !pausePanel.activeInHierarchy)
            {
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    pausePanel.SetActive(true);
                    paused = true;
            }
	

			// INTERACTABLES
			// Interacts with interactables by pressing E.
			if (Input.GetKeyDown(KeyCode.E) && nearInteractable != null)
			{
				nearInteractable.GetComponent<Interactable>().isInteracted = true;
			}

			// INVENTORY ACCESS
			// If the game is not paused, open the inventory when pressing I and pauses the game.
			if (Input.GetKeyDown(KeyCode.I))
			{
                AudioManager.Play("Inventory");
                Time.timeScale = 0f;
				Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
				crosshair.SetActive(false);
				paused = true;
				inventoryPanel.SetActive(true);
			}

			//RECALL ABILITY
			// Press R to place a recall button that allows you to teleport back to a previous location 
			if (Input.GetKeyDown(KeyCode.R) && !recallActive && canRecall)
			{
				if (grounded || submerged || inWater)
				{
                    recallPoint = transform.position;
					recallActive = true;
					recall.SetActive(true);
					recall.transform.position = recallPoint;
					recallPanel.SetActive(true);
				}
			}
			else if (Input.GetKeyDown(KeyCode.R) && recallActive)
			{
				transform.position = recallPoint;
				recallActive = false;
				recall.SetActive(false);
				recallPanel.SetActive(false);
			}
			else if (Input.GetKeyDown(KeyCode.X) && recallActive)
			{
				recallActive = false;
				recall.SetActive(false);
				recallPanel.SetActive(false);
			}


			// PORTAL ABILITY
			// Generates a portal right in front of the player
			if (Input.GetKeyDown(KeyCode.P) && !portalActive && canPortalWarp)
			{
				Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				RaycastHit hit;
				if (!Physics.Raycast(ray, out hit, 7f))
				{
                    AudioManager.Play("PortalMaker");
					portalActive = true;
					portal.SetActive(true);
					portal.transform.rotation = transform.rotation;
					portal.transform.position = transform.position + transform.forward * 5 + new Vector3(0f, 4f, 0f);
				}
			}
			else if (Input.GetKeyDown(KeyCode.P) && portalActive)
			{
				portal.SetActive(false);
				portalActive = false;
			}
		}
		// If the game is paused.
		else
		{
            if (Input.GetKeyDown("escape") && pausePanel.activeInHierarchy)
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                pausePanel.SetActive(false);
                paused = false;
            }

            // INVENTORY ACCESS
            // If the inventory is open, then deactivate it after pressing I and unpauses the game;
            if (Input.GetKeyDown(KeyCode.I) && inventoryPanel.activeInHierarchy)
			{
				Time.timeScale = 1f;
				Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
				crosshair.SetActive(true);
				paused = false;
				inventoryPanel.SetActive(false);

			}

			if (Input.GetKeyDown(KeyCode.E) && nearInteractable != null)
			{
                if (nearInteractable.tag == "Crafting Table")
                {
                    if (currentActiveAbilityPanel != null && currentActiveAbilityPanel.activeInHierarchy)
                    {
                        currentActiveAbilityPanel.SetActive(false);
                    }
                    CraftingTable table = nearInteractable.GetComponent<CraftingTable>();
                    table.interactDisplay.SetActive(true);
                    table.craftingPanel.SetActive(false);
                    crosshair.SetActive(true);
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    paused = false;
                }
				else nearInteractable.GetComponent<Interactable>().isInteracted = true;
			}
		}
	}


	void FixedUpdate()
	{
        if (Mathf.Approximately(Mathf.Clamp(rb.velocity.y, -1f, 1f), rb.velocity.y))
        {
                currentY = transform.position.y;
        }

        // MOUSELOOK
        // use mouselook vector to transform player and cam
        cam.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
		player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);

		// CAMERA TILT
		if (!FadeOut.enabled)
		{
            // tilt the camera if player is not swimming
            if (!submerged)
			{
				// if strafing left
				if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
				{
					if (zRotation > angleSnap)
					{
						zRotation -= tiltSpeed * zBack * Time.deltaTime;
					}
					else if (zRotation < -angleSnap)
					{
						zRotation += tiltSpeed * zBack * Time.deltaTime;
					}
					else
					{
						zRotation = 0;
					}
					cam.transform.Rotate(0, 0, zRotation);
				}
				else if (Input.GetKey(KeyCode.A) && zRotation < maxTilt && !Input.GetKey(KeyCode.D) && zRotation >= 0)
				{
					zRotation += tiltSpeed * Time.deltaTime;
					cam.transform.Rotate(0, 0, zRotation);
				}
				else if (!Input.GetKey(KeyCode.A) && zRotation > 0 || !Input.GetKey(KeyCode.A) && zRotation > 0 && Input.GetKey(KeyCode.D))
				{
					zRotation -= tiltSpeed * zBack * Time.deltaTime;
					if (zRotation < angleSnap)
					{
						zRotation = 0;
					}
					cam.transform.Rotate(0, 0, zRotation);
				}
				else if (zRotation > maxTilt)
				{
					cam.transform.Rotate(0, 0, zRotation);
				}
				// if strafing right
				else if (Input.GetKey(KeyCode.D) && zRotation > -maxTilt && !Input.GetKey(KeyCode.A) && zRotation <= 0)
				{
					zRotation -= tiltSpeed * Time.deltaTime;
					cam.transform.Rotate(0, 0, zRotation);

				}
				else if (!Input.GetKey(KeyCode.D) && zRotation < 0 || !Input.GetKey(KeyCode.D) && zRotation < 0 && Input.GetKey(KeyCode.A))
				{
					zRotation += tiltSpeed * zBack * Time.deltaTime;
					if (zRotation > -angleSnap)
					{
						zRotation = 0;
					}
					cam.transform.Rotate(0, 0, zRotation);
				}
				else if (zRotation < -maxTilt)
				{
					cam.transform.Rotate(0, 0, zRotation);
				}
			}
			// if the player is swimming tilt the camera back towards the center
			else
			{
				if (zRotation > 0)
				{
					zRotation -= tiltSpeed * zBack * Time.deltaTime;
					if (zRotation < angleSnap)
					{
						zRotation = 0;
					}
					cam.transform.Rotate(0, 0, zRotation);
				}
				else if (zRotation < 0)
				{
					zRotation += tiltSpeed * zBack * Time.deltaTime;
					if (zRotation > -angleSnap)
					{
						zRotation = 0;
					}
					cam.transform.Rotate(0, 0, zRotation);
				}
			}
		}

		// MOVEMENT AND SWIMMING
		// move the player's position constantly with respect to time
		if (submerged && canSwim)
		{
            // cancel rigidbody's velocity when submerged and disable gravity
            rb.velocity = Vector3.zero;
            if (submerged) rb.useGravity = false;

			// move player constantly up and down when submerged
			if (Input.GetButton("Jump"))
			{
				rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * move * Time.deltaTime) + (transform.right * Input.GetAxis("Horizontal") * move * Time.deltaTime) + (transform.up * swimSpeed * Time.deltaTime));
			}
			else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !grounded)
			{
				rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * move * Time.deltaTime) + (transform.right * Input.GetAxis("Horizontal") * move * Time.deltaTime) + (-transform.up * swimSpeed * Time.deltaTime));
			}
			// when submerged but not swimming up and down
			else rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * move * Time.deltaTime) + (transform.right * Input.GetAxis("Horizontal") * move * Time.deltaTime));
		}
		else
		{
            rb.useGravity = true;
			rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * move * Time.deltaTime) + (transform.right * Input.GetAxis("Horizontal") * move * Time.deltaTime));
		}
	}

	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.CompareTag("Wall")) currentY = transform.position.y;
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (Mathf.Abs(currentY - transform.position.y) >= yThreshold) AudioManager.Play("Fall");
        }

        // colliding with a new wall in the air allows for another jump
		if (!grounded && collision.gameObject.tag == "Wall" && collision.gameObject.name != wallName && canWallJump)
		{
			rb.velocity = new Vector3(0, -0.1f, 0);
			if (Input.GetButtonDown("Jump") || Input.GetButton("Jump"))
			{
				// record name of wall player collides with
				wallName = collision.gameObject.name;
				rb.velocity = Vector3.up * jumpVelocity;
				rb.velocity += Vector3.up * Physics.gravity.y * (upG / 2) * gravity * Time.deltaTime;
				AudioManager.Play("Jump");
			}
		}
	}

	// staying in contact with a new wall slows player's fall dramatically
	void OnCollisionStay(Collision collision)
	{
		if (!grounded && collision.gameObject.tag == "Wall" && collision.gameObject.name != wallName && canWallJump)
		{
			rb.velocity = new Vector3(0, -0.1f, 0);
			if (Input.GetButtonDown("Jump") || Input.GetButton("Jump"))
			{
				wallName = collision.gameObject.name;
				rb.velocity = Vector3.up * jumpVelocity;
				rb.velocity += Vector3.up * Physics.gravity.y * (upG / 2) * gravity * Time.deltaTime;
				AudioManager.Play("Jump");
			}
		}
	}

	public void Death()
	{
		Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
		crosshair.SetActive(false);
		paused = true;
		GameManager.GetComponent<SaveLoad>().DeleteTempFiles();
		DeathPanel.SetActive(true);
        DeathPanel.GetComponentInChildren<RandomText>().enabled = true;
		FadeOut.enabled = true;
		FadeOut.GetComponent<CanvasGroup>().alpha = 0;
	}

	public void OnRevive()
	{
        rb.velocity = Vector3.zero;
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		crosshair.SetActive(true);
		paused = false;
        FadeOut.GetComponent<CanvasGroup>().alpha = 0;
		FadeOut.enabled = false;
		DeathPanel.SetActive(false);
    }


    // ACTIVATING ABILITIES
    // Activates crafted ability based on the given recipe number.
    public void ActivateAbility(Recipe completedRecipe)
	{
		// Activate jumping
		if (completedRecipe.recipeNumber == 0)
		{
			canJump = true;
            unlockJumpPanel.SetActive(true);
            currentActiveAbilityPanel = unlockJumpPanel;
		}

		// Activate wall jumping
		if (completedRecipe.recipeNumber == 1)
		{
			canWallJump = true;
            unlockWallJumpPanel.SetActive(true);
            currentActiveAbilityPanel = unlockWallJumpPanel;
        }

		// Activate telekinesis
		if (completedRecipe.recipeNumber == 2)
		{
			canGrab = true;
            unlockGrabPanel.SetActive(true);
            currentActiveAbilityPanel = unlockGrabPanel;
        }

		// Activate swimming
		if (completedRecipe.recipeNumber == 3)
		{
			canSwim = true;
            unlockSwimmingPanel.SetActive(true);
            currentActiveAbilityPanel = unlockSwimmingPanel;
        }

		// Activate make portals
		if (completedRecipe.recipeNumber == 4)
		{
			canPortalWarp = true;
            unlockPortalGeneratorPanel.SetActive(true);
            currentActiveAbilityPanel = unlockPortalGeneratorPanel;
        }

		// Activate recall
		if (completedRecipe.recipeNumber == 5)
		{
			canRecall = true;
            unlockRecallPanel.SetActive(true);
            currentActiveAbilityPanel = unlockRecallPanel;

        }

		// Activate grappling hook
		if (completedRecipe.recipeNumber == 6)
		{
			canGrapple = true;
			player.GetComponent<GrapplingHook>().enabled = true;
            unlockGrapplingHookPanel.SetActive(true);
            currentActiveAbilityPanel = unlockGrapplingHookPanel;
        }
	}

	// DATA SAVE
	// Returns the private ability variables.
	public bool PlayerCanJump()
	{
		return canJump;
	}
	public bool PlayerCanWallJump()
	{
		return canWallJump;
	}
	public bool PlayerCanSwim()
	{
		return canSwim;
	}
	public bool PlayerCanGrab()
	{
		return canGrab;
	}
	public bool PlayerCanRecall()
	{
		return canRecall;
	}
	public bool PlayerCanPortalWarp()
	{
		return canPortalWarp;
	}
	public bool PlayerCanGrapple()
	{
		return canGrapple;
	}

	// Returns private recall variables
	public bool IsRecallActive()
	{
		return recallActive;
	}
	public float[] RecallPoint()
	{
		float[] point = new float[3];
		point[0] = recallPoint.x;
		point[1] = recallPoint.y;
		point[2] = recallPoint.z;
		return point;
	}

	// Returns portal variables
	public bool IsPortalActive()
	{
		return portalActive;
	}
	public float[] PortalPosition()
	{
		float[] portalPos = new float[3];
		portalPos[0] = portal.transform.position.x;
		portalPos[0] = portal.transform.position.y;
		portalPos[0] = portal.transform.position.z;
		return portalPos;
	}

	// Restores the abilities based on the save
	public void RestoreAbilities(PlayerData data)
	{
		canJump = data.canJump;
		canWallJump = data.canWallJump;
		canSwim = data.canSwim;
		canGrab = data.canGrab;
		canRecall = data.canRecall;
		canPortalWarp = data.canPortalWarp;
        canGrapple = data.canGrapple;
	}
}

