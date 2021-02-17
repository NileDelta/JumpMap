using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In this version the camera is fixed and can not transform.
//This is a simplified test of the JumpMan mechanics. 3D navigation may not be resolvable in this version... that's okay!
//JumpMan opperates using forced perspective and an array of spawns. The player unknowingly controls a defined # of JumpMen

public class JumpManArray : MonoBehaviour
{
    GameObject clone0;
    GameObject clone1;
    GameObject clone2;
    Rigidbody rb;
    [SerializeField] float jumpManSpeed = 1f;
    [SerializeField] float jumpManJump = 1f;

    Vector3 camPosition;
    Vector3 jumpManPosition;
    Vector3 clone0Position;
    Vector3 clone1Position;
    Vector3 clone2Position;
    float jumpMenDistance;

    [SerializeField] int jumpMenToCreate = 1; //number of points to locate between JumpMan Vector and Cam Vector THIS ISN"T USED YET
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camPosition = GameObject.Find("Camera").transform.position;
        clone0 = GameObject.Find("JumpManClone");
        clone1 = GameObject.Find("JumpManClone (1)");
        clone2 = GameObject.Find("JumpManClone (2)");
       
        
    }
    // Update is called once per frame
    void Update()
    {
        moveRight();
        moveLeft();
        jumping();
        jumpManPosition = transform.position;
        traceJumpMen();
        
        
    }
    void moveRight(){
        if (Input.GetKey(KeyCode.A)){
            rb.AddRelativeForce(Vector3.right * jumpManSpeed * Time.deltaTime);}
    }
    void moveLeft(){
        if (Input.GetKey(KeyCode.D)){
            rb.AddRelativeForce(Vector3.left * jumpManSpeed * Time.deltaTime);}
    }
    void jumping(){
        if (Input.GetKeyDown(KeyCode.Space)){
            rb.AddRelativeForce(Vector3.up * jumpManJump * Time.deltaTime);}
    }
    // The following methods configure the forced-perspective JumpMan Array relative to the camera position
    void traceJumpMen()
    {
        //TO DO if the spawn location is the same as current spawnedclone location then return (player is standing still)
        //TO DO ''                     ''not the same as ''                     '' then transform spawned clone
        //Here we calculate the distance between the segment pieces.
        jumpMenDistance = (Mathf.Abs(Vector3.Distance(camPosition,jumpManPosition))/(jumpMenToCreate+1));
    
            //Get the position
            clone0Position = Vector3.Lerp(camPosition,jumpManPosition,0.25f);//the .25 should be replaced with jumpmendistance... but remain as percentage...
            //also how do I spawn a serializefield number of instantiates>>>>> lets focus on 3 for now.
            clone1Position = Vector3.Lerp(camPosition,jumpManPosition,0.5f);
            clone2Position = Vector3.Lerp(camPosition,jumpManPosition,0.75f);
            if (clone0.transform.position != clone0Position)
            {
                clone0.transform.position = clone0Position;
            }
            if (clone1.transform.position != clone1Position)
            {
                clone1.transform.position = clone1Position;
            }
            if (clone2.transform.position != clone2Position)
            {
                clone2.transform.position = clone2Position;
            }
            Debug.Log("cam Position is:" + camPosition);
            Debug.Log("JumpMan Position is:" + jumpManPosition);
            
    }
}
