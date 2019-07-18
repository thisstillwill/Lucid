using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
   public float xRotate;
   public float yRotate;
   public float zRotate;

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Rotate(xRotate, yRotate, zRotate, Space.Self);

    }
}
