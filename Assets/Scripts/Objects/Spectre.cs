using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spectre : MonoBehaviour
{
    public float walkRadius;
    public float runRadius;
    public float watchRadius;
    public float walkSpeed;
    public float runSpeed;
    public float scalingFactor;
    public LayerMask playerMask;
    public GameObject center;
    public float maxDistance;

    private GameObject player;
    private Rigidbody rb;
    private Vector3 startingPosition;



    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, center.transform.position) > maxDistance)
        {
            transform.position = startingPosition;
            rb.velocity = Vector3.zero;
        } 
        else if (Physics.CheckSphere(transform.position, runRadius, playerMask))
        {
            Vector3 direction = transform.position - player.transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, player.transform.rotation, Time.deltaTime / scalingFactor);
            rb.AddForce(direction * runSpeed);
        }
        else if (Physics.CheckSphere(transform.position, walkRadius, playerMask))
        {
            Vector3 direction = transform.position - player.transform.position;
            rb.AddForce(direction * walkSpeed);
        }
        else if (!Physics.CheckSphere(transform.position, watchRadius, playerMask))
        {
            if (!Physics.CheckSphere(transform.position, watchRadius, playerMask))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), 100 * Time.deltaTime);
            }
            rb.velocity = Vector3.zero;
        }
    }
}
