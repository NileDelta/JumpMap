using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//README
// Jumpman Mover is the physics handler for the JumpMan Character. 
// His actions and animations are translated from values altered by this script such as JumpMan's Scale, speed, and orientation.
// The Mover object is fixed to the vehicle, and the probe rays anchor to the camera view and the Mover object to project the JumpMan character.


//STATE CONDITIONS
    
public enum Mover_State { Rising, Falling, Grounded, Crouched }
    
public enum Grounding_State { Nothing, Obstruction, Object, Respawn, Finish }


public class JumpManMover : MonoBehaviour
{
    [SerializeField] float speed = .1f; // JumpMan's Speed.
    [SerializeField] float drawDis = 1000f; // Distance JumpMan can probe the landscape.
    [SerializeField] float probeOffset = .01f; // Distance between rayA and rayC, and rayB and rayC.
    [SerializeField] float slowFall = .01f; // Force applied against gravity in certain conditions.
    [SerializeField] float waitTime = 1f; // Time to delay JumpMan's movement at the start of the game.

    Rigidbody rb;
    Camera cam; 
    GameObject vehicle;

    //STATE SYSTEMS
    private Mover_State mover_State; // Rising, Falling, Grounded, Crouched
    private Mover_State prevMover_State;
    private Grounding_State grounding_State; // Nothing, Obstruction, Object, Respawn, Finish
    private Grounding_State prevGrounding_State;

    
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
    }

    void Update() 
    {
        probeOffsetVector = new Vector3(0,probeOffset,0); // This value may become hard coded.
        NavProbe(); //Used to evaluate the immediate conditions that the player can use to change jumpman pos, EX: moving up and down stairs, taking large steps or hops laterally or vertically
        StateConditions();
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
    public void GroundingProbe()
    {
        // TO DO
            rayC = new Ray(camPos, (moverPos - camPos)); //Center Ray
            rayA = new Ray((camPos+probeOffsetVector), rayC.direction);
            rayB = new Ray((camPos-probeOffsetVector), rayC.direction);
            
        
        //FIRST, no sense checking anything if that hit is a death.    
            if (Physics.Raycast(rayC, out hitC, drawDis, Respawn)) 
            {
                gameObject.transform.position = start; 
                // TO DO - set state to "Falling"
            }

        //SECOND, no sense checking if the player doesn't want to land or be grounded. This check can dislodge them.
            if (verMov < 0 && Input.GetMouseButtonDown(0)) 
            {
                rb.useGravity = true; 
                // TO DO    - set state to "Falling"
                //          - make sure no further operations for the grounding probe work while this input combination is active.
            }

        //THIRD, if "Falling" and rayC.hit = platform,  gravity velocity should be halved - simulating falling through an object but also reducing velocity to prepare to land.    
            if (Physics.Raycast(rayB, out hitB, drawDis, Platform) && rb.useGravity == true)
                // TO DO    - ADD && is "Falling" 
            {
                SlowFalling();
                Debug.DrawRay(rayB.origin, rayC.direction, Color.magenta);
                Debug.Log("Slowing!");
            }

        //FOURTH, Grounding
            if (Physics.Raycast(rayC, out hitC, drawDis, Platform))
            {
                rb.useGravity = false;
                Debug.DrawRay(rayC.origin, rayC.direction, Color.green);
                Debug.Log("Grounded!");
            }
        
        //FIFTH, after all those checks, assume jumpMan is "Falling"
            else
            {

            //SET ENUM GROUNDINGPROBE STATE based on hitobject tag.
                rb.useGravity = true;
                Debug.DrawRay(rayC.origin,rayC.direction,Color.yellow);

            //Compare hit value for B,
            // IF hitobject layer = sill, reset Jumpman else...

            //Cast Probe A and C
            //Compare hit distances, define edge_state and condition JumpManPos.

            //if probe is invalid, jumpman is NOT grounded and Mover has gravity.
            //JumpMan DOES NOT HAVE PHYSICS! probes indicate his behaviour.
            }
        
    }

    private void SlowFalling()
    {
        rb.AddForce(0, slowFall, 0);
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
        if (mover_State == Mover_State.Rising)
        {
            //turn gravity on
        }
        if (mover_State == Mover_State.Falling)
        {
            //turn gravity on
            //look for probe condition and input conditions to land somewhere...
            //if player holds buttons, fall faster.
            //if player holds combination, they ignore landing entirely.
        }
        if (mover_State == Mover_State.Grounded)
        {
            //turn gravity off
            //look for player input to update things?
        }
        if (mover_State == Mover_State.Crouched)
        {
            //turn gravity off?
            //
        }
    }
}
