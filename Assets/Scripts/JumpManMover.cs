using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//README
// Jumpman Mover is the physics handler for the JumpMan Character. 
// His actions and animations are translated from values altered by this script such as JumpMan's Scale, speed, and orientation.
// The Mover object is fixed to the vehicle, and the probe rays anchor to the camera view and the Mover object to project the JumpMan character.


//STATE CONDITIONS
    
public enum Mover_State { Rising, Falling, Grounded, Crouched }

    
public class JumpManMover : MonoBehaviour
{
    [SerializeField] float speed = .1f; // JumpMan's Speed.
    [SerializeField] float drawDis = Mathf.Infinity; // Distance JumpMan can probe the landscape.
    [SerializeField] float probeOffset = .01f; // Distance between rayA and rayC, and rayB and rayC.
    [SerializeField] float slowFall = .01f; // Force applied against gravity in certain conditions.
    // TO DO    - Normalize this value and insert as a percentage to multiply velocity.y to.
    [SerializeField] float waitTime = 1f; // Time to delay JumpMan's movement at the start of the game.

    Rigidbody rb;
    Camera cam; 
    GameObject vehicle;

    //STATE SYSTEMS
    private Mover_State mover_State; // Rising, Falling, Grounded, Crouched
    private Mover_State prevMover_State;
    
    //VECTORS
    Vector3 moverPos;
    Vector3 camPos;
    Vector3 start;
    Vector3 probeOffsetVector;


    //RAYS & RAYCASTHITS
    Ray rayC;
    RaycastHit hitC;
    Ray rayA;
    RaycastHit hitA;
    Ray rayB;
    RaycastHit hitB;


    //INPUT/CONTROLLER VALUES
    float horMov;
    float verMov;


    public LayerMask Nothing;
    public LayerMask Obstruction;
    public LayerMask Platform;
    public LayerMask Respawn;
    public LayerMask Finish;

    

//==============================================================================================================================================================
    void Start()
    {
        start = gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
        vehicle = GameObject.Find("Vehicle");
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        rb.useGravity = false;
        mover_State = Mover_State.Grounded;
        Invoke ("WaitTime", waitTime);
    }
    private void WaitTime()
    {
        mover_State = Mover_State.Falling;
    }

    void FixedUpdate()
    {
        MoverInput();
        UpdatePositions();
        GroundingProbe();
        StateConditions();
        
    }

    void Update() 
    {
        probeOffsetVector = new Vector3(0,probeOffset,0); // This value may become hard coded.
        NavProbe(); //Used to evaluate the immediate conditions that the player can use to change jumpman pos, EX: moving up and down stairs, taking large steps or hops laterally or vertically
        
        UpdateMoverPos();
        UpdateJumpManPos();
                       
    }
    private void MoverInput()
    {
        horMov = Input.GetAxisRaw("Horizontal");
        verMov = Input.GetAxisRaw("Vertical"); //will be used for NavProbe and conditional movements
        
        // TO DO use jumping tutorial to do press button trick. use value for Y Velocity
        // ADD Kyote time checks.    
    }

    private void UpdatePositions()
    {
        camPos = cam.transform.position;
        moverPos = gameObject.transform.position;
    }
    private void GroundingProbe()
    {
        // TO DO
            int layerMask = 1 << 10;
            layerMask = ~layerMask; // NOT USING BUILT IN LAYER NAMES. Rays detect all objects except layerMask 10 "Player"
            rayC = new Ray(camPos, (moverPos - camPos)); //Center Ray
            Physics.Raycast(rayC, out hitC, drawDis, layerMask); Debug.Log(hitC.collider.gameObject.name);
            rayA = new Ray((camPos+probeOffsetVector), rayC.direction); //how to make the probe offset extrude from a different angle?????
            Physics.Raycast(rayA, out hitA, drawDis, layerMask); Debug.Log(hitA.collider.gameObject.name);
            rayB = new Ray((camPos-probeOffsetVector), rayC.direction);
            Physics.Raycast(rayB, out hitB, drawDis, layerMask); Debug.Log(hitB.collider.gameObject.name);
                  
        //FIRST, Is JumpMan's position invalid?    
        if (Physics.Raycast(rayC, out hitC, drawDis, Respawn))// TO DO - Add check if JumpMan is not visable but should be 
            {
                gameObject.transform.position = start; 
                mover_State = Mover_State.Falling;
            }

        //SECOND, If the player doesn't want to land or be grounded. This check can dislodge them.
        if (verMov < 0 && Input.GetMouseButtonDown(0)) // TO DO - set this mouse input to a value that times out.
            {
                mover_State = Mover_State.Falling; // This check affectively disables grounding probe if true.
            }
        
        //THIRD, should JumpMan be slow falling? If so, gravity velocity should be halved - simulating falling through an object but also reducing velocity to prepare to land.    
        if (Physics.Raycast(rayB, out hitB, drawDis, Platform) && mover_State == Mover_State.Falling)
            {
                SlowFalling();
                Debug.DrawRay(rayB.origin, rayC.direction, Color.magenta);
                Debug.Log("Slowing!");
            }

        //FOURTH, Grounding
        if (mover_State == Mover_State.Falling && Physics.Raycast(rayC, out hitC, drawDis, Platform) //Is JM falling & does rayC hit a Platform?
            || Physics.Raycast(rayC, out hitC, drawDis, Platform) && hitC.distance-hitB.distance < hitA.distance-hitC.distance //Does rayC hit a platform and is it the top of a ledge?
            || Physics.Raycast(rayC, out hitC, drawDis, Platform) && hitC.distance > hitA.distance && hitC.distance > hitB.distance) //Does rayC hit a platform and is it a wire?
            {
                mover_State = Mover_State.Grounded;
                Debug.DrawRay(rayB.origin, rayC.direction, Color.red);
            }
        
        //FIFTH, if these probe conditions are ever met, he should be falling.
        if (mover_State == Mover_State.Grounded && hitC.collider == null
            ||Mathf.Approximately(hitA.distance-hitC.distance, hitC.distance-hitB.distance))
        {
            mover_State = Mover_State.Falling;
        }

          
    }

    private void SlowFalling()
    {
        rb.AddRelativeForce(new Vector3(0, -rb.velocity.y/2, 0));
    }

    private void NavProbe()
    {
            // TO DO Check for transitional conditions for hopping and stepping up/down.
    }
    
    private void UpdateMoverPos()
    {
        // TO DO Using available player inputs and conditions based on grounding states and probe conditions, translate to Mover movement.
        rb.velocity = new Vector3(horMov, verMov, rb.velocity.z) * speed;
        moverPos = transform.position;
        // TO DO convert this vector to be dependent on Vehicle transformations to adjust for vehicle rotation in world.
    }

    private void UpdateJumpManPos()
    {
        // TO DO JumpMan position (JumpManPos) is always transformed along the probe.
            //If Grounded, JumpManPos is set to hit pos.
            //If NOT Grounded, JumpManPos is 1m from Camera. (Mover remains between and falls due to gravity, probe is 'scanning')
    }
    private void StateConditions()
    {
        if (mover_State != prevMover_State)
        {
            prevMover_State = mover_State;
            MoverStates();
        }


    }

    private void MoverStates() //condition gravity and animations
    {
        if (mover_State == Mover_State.Grounded)
        {
            rb.useGravity = false;
            Debug.Log("current state is Grounded");
        }
        if (mover_State == Mover_State.Crouched)
        {
            rb.useGravity = false;
            Debug.Log("current state is Crouched");
        }
        if (mover_State == Mover_State.Rising)
        {
            rb.useGravity = true;
            Debug.Log("current state is Rising");
        }
        if (mover_State == Mover_State.Falling)
        {
            rb.useGravity = true;
            Debug.Log("current state is Falling");
        }
        
    }
}
