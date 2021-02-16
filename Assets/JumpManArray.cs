using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In this version the camera is fixed and not transform.
//This is a simplified test of the JumpMan mechanics. 3D navigation may not be resolvable in this version... that's okay!
//JumpMan opperates using forced perspective and an array of spawns. The player unknowingly controls a defined # of JumpMen (Serialize this Field).
//In this version JumpMan should be able to Jump and Move left and right

public class JumpManArray : MonoBehaviour
{
    // define our variables
    Rigidbody rb;
    
    [SerializeField] float jumpManSpeed = 1f;
    [SerializeField] float jumpManJump = 1f;
    // Start is called before the first frame update
    // To do [SerializedField] int JumpManArrayed
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        moveRight();
        moveLeft();
        jumping();
        // To do locate game camera location
        // To do position Jumpman 10m linearly (serialize this field) away from camera
        // To do calculate actual distance of Jumpman
    }
    void moveRight()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeForce(Vector3.right * jumpManSpeed * Time.deltaTime);
        }
    }
    void moveLeft()
    {
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeForce(Vector3.left * jumpManSpeed * Time.deltaTime);
        }
    }
    void jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * jumpManJump * Time.deltaTime);
        }
    }
}
