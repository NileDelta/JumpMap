using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In this version the camera is fixed and can not transform.
//This is a simplified test of the JumpMan mechanics. 3D navigation may not be resolvable in this version... that's okay!
//JumpMan opperates using forced perspective and an array of spawns. The player unknowingly controls a defined # of JumpMen

public class JumpManArray : MonoBehaviour
{
    GameObject clonePrefab;
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
    Vector3 clone0Scale;
    Vector3 clone1Scale;
    Vector3 clone2Scale;
    float jumpMenDistance;
    private int count; 

    [SerializeField] int jumpMenToCreate = 1; //number of points to locate between JumpMan Vector and Cam Vector THIS ISN"T USED YET
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camPosition = GameObject.Find("Camera").transform.position;
        clone0 = GameObject.Find("JumpManClone");
        clone1 = GameObject.Find("JumpManClone (1)");
        clone2 = GameObject.Find("JumpManClone (2)");
        clone0Scale = new Vector3(0.25f,0.25f,0.25f);
        clone1Scale = new Vector3(0.5f,0.5f,0.5f);
        clone2Scale = new Vector3(0.75f,0.75f,0.75f);

        //RETURN HERE TO REWRITE USING INSTANTIATE AND 1 JUMPMAN prefab - tags will be used to identify active jumpman which other jumpmen will react from
        
        //Instantiate(clonePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //clone0.name = "JumpManClone"+count;//TO DO instantiate and count our clones based on the serializefield
        //count=count+1;
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
        jumpMenDistance = (Mathf.Abs(Vector3.Distance(camPosition, jumpManPosition)) / (jumpMenToCreate + 1));

        //Get the position
        clone0Position = Vector3.Lerp(camPosition, jumpManPosition, 0.25f);//the .25 should be replaced with a variable jumpmendistance... but remain as percentage...
                                                                           
        clone1Position = Vector3.Lerp(camPosition, jumpManPosition, 0.5f);
        clone2Position = Vector3.Lerp(camPosition, jumpManPosition, 0.75f);
        repositionClones();
        Debug.Log("cam Position is:" + camPosition);
        Debug.Log("JumpMan Position is:" + jumpManPosition);

    }

    private void repositionClones()
    {
        if (clone0.transform.position != clone0Position)
        {
            clone0.transform.position = clone0Position;
            clone0.transform.localScale = clone0Scale;
        }
        if (clone1.transform.position != clone1Position)
        {
            clone1.transform.position = clone1Position;
            clone1.transform.localScale = clone1Scale;
        }
        if (clone2.transform.position != clone2Position)
        {
            clone2.transform.position = clone2Position;
            clone2.transform.localScale = clone2Scale;
        }
    }
}
