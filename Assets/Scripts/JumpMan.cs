using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMan : MonoBehaviour
{
    
    [SerializeField] float jumpManSpeed = 1f;
    [SerializeField] float jumpManJump = 1f;
    Rigidbody rbJM;
    [SerializeField] int numberOfJM;
    int currentCount; //this value is used to name cloned JumpMan and prevent count exceeding numberOfJM
    int distanceBetweenFirstJM; //the distance between following JMs will grow logorithmically until numberOfJM is satisfied
    GameObject jumpMan0;
    GameObject SearchingJMs;
    GameObject NullJMs;
    GameObject mainCamera;
    // Define variables for ActiveJM so that referred JumpManClone is providing correct Vector Values
    // Define tag assignment variables
    // Tag for nullJM
    // Tag for ActiveJM 
    // Tag for SearchingJM (still responds to ActiveJM siimilar to nullJM but is actively searching for new collisions to become ActiveJM and changing previous activeJM to nullJM)  
        // can ActiveJM become a bool state that can only have 1 active variable assigned which is updated by previous line condition?

    // Tag Platforms and ActivePlatform
        //if ActiveJM touches Platforms they are assigned as Active

    void Start()
    {
        
        //assign static values
        //assign component names for instantiating copies
        //create var shortcuts for component parts of jumpman and camera
    }

    void Update()
    {
        jumpMan0 = GameObject.Find("JumpMan");
        rbJM = jumpMan0.GetComponent<Rigidbody>();
        SearchingJMs = GameObject.FindWithTag("SearchingJM");
        NullJMs = GameObject.FindWithTag("NullJM");
        jumpMan0.GetComponent<Rigidbody>().useGravity = true;
        //NullJMs.GetComponent<Rigidbody>().useGravity = false;
        //SearchingJMs.GetComponent<Rigidbody>().useGravity = false;
        //create naming logic for naming jumpman clones name = "JumpMan" + currentCount; currentCount++;
        //create logic to Assign tags to jumpMan clones as they spawn (or default should be nullJM and begin by assigning JumpMan0 as Active)
        
        //begin searching for landing on all clones, assign first clone (0) as 'Active clone'
        //assign logic that ActiveClone controls gravity (gravity may need affect clones differently at different scales(if so lets hope its logorithmic to calc, if not fake it))
        //assign movement paraments to active clone? or assign to the vector...
        moveRight();
        moveLeft();
        jumping();

    }
    void moveRight(){
        if (Input.GetKey(KeyCode.A)){
            Debug.Log("pressingA");}
            //rbJM.AddRelativeForce(Vector3.right * jumpManSpeed * Time.deltaTime);}
    }
    void moveLeft(){
        if (Input.GetKey(KeyCode.D)){
            rbJM.AddRelativeForce(Vector3.left * jumpManSpeed * Time.deltaTime);}
    }
    void jumping(){
        if (Input.GetKeyDown(KeyCode.Space)){
            rbJM.AddRelativeForce(Vector3.up * jumpManJump * Time.deltaTime);}
    }
}
