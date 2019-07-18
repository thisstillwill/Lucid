using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBounce : MonoBehaviour
{
	public float power;
	public GameObject player;
	private Rigidbody rb;
    private AudioManager AudioManager;

    private void Start()
    {
        AudioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
	{
		rb = other.GetComponent<Rigidbody>();

		// bounce object if it has a rigidbody
		if (rb != null)
		{

			// hook the player if grappling hook is shot at it
			if (other.gameObject.tag == "Hook")
			{
				other.gameObject.GetComponent<HookDetector>().player.GetComponent<GrapplingHook>().hooked = true;
			}

			// mark that player is launched
			if (other.gameObject.name == "Player") player.GetComponent<PlayerController>().launched = true;

			// shoot object on the velocity normal to the bounce pad surface
			Vector3 force = new Vector3(0f, power, 0f);
			float angleX = transform.rotation.eulerAngles.x;
			float angleY = transform.rotation.eulerAngles.y;
			float angleZ = transform.rotation.eulerAngles.z;
			force = Quaternion.Euler(angleX, angleY, angleZ) * force;
			rb.velocity = force;


            AudioManager.Play("Launch");
		}
	}
}
