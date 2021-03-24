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

    //[SerializeField] float slowFall = .01f; // Force applied against gravity in certain conditions.
    // TO DO    - Normalize this value and insert as a percentage to multiply velocity.y to.
    [SerializeField] float waitTime = 2f; // Time to delay JumpMan's movement at the start of the game.

    Rigidbody rb;
    Camera cam;
    GameObject vehicle;

    //STATE SYSTEMS
    private Mover_State mover_State; // Rising, Falling, Grounded, Crouched
    private Mover_State prevMover_State;

    //VECTORS
    Vector3 moverPos;
    Vector3 camPos;
    Vector3 startPos;
    Vector3 GroundBoxCastSize = new Vector3(.01f,.05f,.001f);
    Vector3 ProbeBoxCastSize = new Vector3(.01f,.05f,.001f);
    Quaternion probeRotationA;
    Quaternion probeRotationB;


    //RAYS & RAYCASTHITS
    RaycastHit hitR; //Reset
    RaycastHit hitC;
    RaycastHit hitA;
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
        startPos = gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
        vehicle = GameObject.Find("Vehicle");
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        mover_State = Mover_State.Grounded;
        probeRotationA = cam.transform.rotation * Quaternion.Euler(.05f,0f,0f);
        probeRotationB = cam.transform.rotation * Quaternion.Euler(-.05f,0f,0f);
        Invoke("WaitTime", waitTime);
    }
    private void WaitTime()
    {
        prevMover_State = mover_State;
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
        NavProbe(); //Used to evaluate the immediate conditions that the player can use to change jumpman pos, EX: moving up and down stairs, taking large steps or hops laterally or vertically
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
        // TO DO Using available player inputs and conditions based on grounding states and probe conditions, translate to Mover movement.
        //rb.velocity = new Vector3(horMov, verMov, rb.velocity.z) * speed; //CHANGE THIS TO ADD FORCE!
        //rb.AddForce(horMov, verMov, 0);
        rb.MovePosition(transform.position+(new Vector3(horMov, verMov*3, 0) * speed * Time.deltaTime));
        moverPos = transform.position;
        // TO DO convert this vector to be dependent on Vehicle transformations to adjust for vehicle rotation in world.
    }
    private void GroundingProbe()
    {
        //FIRST, Is JumpMan's position invalid?    
        if (Physics.Raycast(new Ray(camPos, (moverPos - camPos)), out hitR, drawDis, Respawn))// TO DO - change this to screen on point check within borders... something to do with comparing rotation angles.
        {
            gameObject.transform.position = startPos;
            mover_State = Mover_State.Falling;
        }

        int layerMask = 1 << 10;
        layerMask = ~layerMask; // NOT USING BUILT IN LAYER NAMES. Rays detect all objects except layerMask 10 "Player"

        bool cHasHit = Physics.BoxCast(moverPos, GroundBoxCastSize, (moverPos - camPos), out hitC, cam.transform.rotation, drawDis, layerMask);
        bool aHasHit = Physics.BoxCast(moverPos, ProbeBoxCastSize, (moverPos - camPos), out hitA, probeRotationA , drawDis, layerMask);
        //LineCast that scans downward by rotating downward from cam.transform.rotation, at set increments. store new RayCastHit.hitDown1 if its an edge (DO MAGIC HERE!)
        //send probe down again, compare hit to stored hit until a new unique edge is found, store hitDown2. (THESE NEED TO BE NEW STORED VALUES)
        //once hitDown2=true, then let gravity take JumpMan until screenpoint is close to hitDown1 screenpoint for y! 

        //Now do all this in reverse for rising? or do the same scan for above... probable should actually.
        //      Ok so now that we've got all that scanning working and not buggy at all we can incorporate a nav function that does a similar version as above.

        bool bHasHit = Physics.BoxCast(moverPos, ProbeBoxCastSize, (moverPos - camPos), out hitB, cam.transform.rotation, drawDis, layerMask);

        bool probeHasHit = cHasHit || aHasHit || bHasHit;

        if (probeHasHit)
        {
            Debug.Log("hit Something");

            

            //SECOND, If the player doesn't want to land or be grounded. This check can dislodge them.
            if (verMov < 0 && Input.GetMouseButtonDown(0)) // TO DO - set this mouse input to a value that times out.
            {
                mover_State = Mover_State.Falling; // This check affectively disables grounding probe if true.
            }

            //THIRD, should JumpMan be slow falling? If so, gravity velocity should be halved - simulating falling through an object but also reducing velocity to prepare to land.    
            if (bHasHit)
            {
                if (hitB.collider.gameObject.layer == 8 && mover_State == Mover_State.Falling)
                {
                    SlowFalling();
                    Debug.Log("Slowing!");
                }
            }

            //FOURTH, Grounding
            if (cHasHit)
            {
                Debug.Log(hitC.collider.gameObject.layer);
                if (hitC.collider.gameObject.layer == 8 && mover_State == Mover_State.Falling
                || hitC.collider.gameObject.layer == 8 && hitC.distance - hitB.distance < hitA.distance - hitC.distance //Does rayC hit a platform and is it the top of a ledge?
                || hitC.collider.gameObject.layer == 8 && hitC.distance > hitA.distance && hitC.distance > hitB.distance) //JM on a Wire
                {
                    mover_State = Mover_State.Grounded;
                    Debug.DrawRay(camPos, hitC.point - camPos, Color.red);
                }
            }

            //FIFTH, if these probe conditions are ever met, he should be falling.
            if (cHasHit == false)
                    //|| Mathf.Approximately(hitA.distance - hitC.distance, hitC.distance - hitB.distance))
            {
                Debug.Log("going to fall now...");
                mover_State = Mover_State.Falling;
            }

            
                Debug.DrawRay(camPos, hitC.point - camPos, Color.yellow);
                Debug.DrawRay(camPos, hitA.point - camPos, Color.yellow);
                Debug.DrawRay(camPos, hitB.point - camPos, Color.yellow);
                
            
        }

        else
        {
            mover_State = Mover_State.Falling;
            Debug.DrawRay(camPos, moverPos - camPos, Color.red);
            Debug.Log("No Hit.");
        }
    }

    private void SlowFalling()
    {
        rb.AddRelativeForce(new Vector3(0, -rb.velocity.y / 2, 0));
    }

    private void NavProbe()
    {
        // TO DO Check for transitional conditions for hopping and stepping up/down.
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
