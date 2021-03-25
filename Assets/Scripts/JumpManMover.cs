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
    Plane probe_Plane;

    public int deathCount;

    //STATE SYSTEMS
    public Mover_State mover_State; // Rising, Falling, Grounded, Crouched
    public Mover_State prevMover_State;

    public bool horMoving;
    public bool verMoving;

    //VECTORS
    public float probeOffset;
    public float groundDis;
    Vector3 probeOffsetDir;
    Vector3 moverPos;
    Vector3 camPos;
    Vector3 startPos;
    Vector3 probePos;
    Vector3 probe_PlanePos;
    public Vector3 moverCamPos; //THESE ARE A CUSTOM COORDINATE SYSTEM, convert pixels to 10,000x10,000 whole units, z is world dim relative to cam.
    public Vector3 probeCamPos;
    Vector3 bottomCamPos;
    public Vector3 edgeBellowCamPos;
    public Vector3 edgeBellowProPos;
    Vector3 GroundBoxCastSize = new Vector3(.01f,.05f,.001f);
    Vector3 ProbeBoxCastSize = new Vector3(.01f,.05f,.001f);
    Quaternion probeRotationA;
    Quaternion probeRotationB;


    //RAYS & RAYCASTHITS

    Ray probeRay;
    Ray edgeRay;
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
        Invoke("WaitTime", waitTime);
        probe_PlanePos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z - 2);
        probe_Plane = new Plane(cam.transform.forward, probe_PlanePos);
        edgeBellowCamPos = new Vector3(cam.pixelWidth,cam.pixelHeight,1000);
        probeCamPos = (cam.WorldToScreenPoint(probePos));
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
        Grounding();
        StateConditions();
    }

    void Update()
    {
        //NavProbe(); //Used to evaluate the immediate conditions that the player can use to change jumpman pos, EX: moving up and down stairs, taking large steps or hops laterally or vertically
        //UpdateJumpManPos();
        DeathCheck();
    }
    
    private void MoverInput()
    {
        horMov = Input.GetAxisRaw("Horizontal");
        verMov = Input.GetAxisRaw("Vertical"); //will be used for NavProbe and conditional movements
        if (horMov != 0){horMoving = true;}
        if (verMov != 0){verMoving = true;}
        else if (horMov == 0){horMoving = false; }
        else if (verMov == 0){verMoving = false; }

        // TO DO use jumping tutorial to do press button trick. use value for Y Velocity
        // ADD Kyote time checks.    
    }
    private void UpdatePositions()
    {
        camPos = cam.transform.position;
        
        float enter = 0.0f;
        probe_Plane.Raycast(edgeRay, out enter);
        edgeBellowProPos = edgeRay.GetPoint(enter);
     
        rb.MovePosition( transform.position + edgeBellowProPos * speed * Time.deltaTime);
        if (horMoving)
        {
        rb.MovePosition(transform.position+(new Vector3(horMov, verMov, 0) * speed * Time.deltaTime));
        }

        moverPos = transform.position;
        moverCamPos = cam.WorldToScreenPoint(moverPos);
        // TO DO convert this vector to be dependent on Vehicle transformations to adjust for vehicle rotation in world.
    }
    private void GroundingProbe()
    {
        int layerMask = 1 << 10;
            layerMask = ~layerMask; // NOT USING BUILT IN LAYER NAMES. Rays detect all objects except layerMask 10 "Player"
        
        if(Mathf.Approximately(transform.position.x,edgeBellowCamPos.x)==false && mover_State == Mover_State.Falling)
        {
            
            probePos = moverPos;
            probeCamPos = cam.WorldToScreenPoint(probePos); //CURRENTLY OPPERATES AT PIXELWIDTH,PIXELHEIGHT
            if (probeCamPos.y-edgeBellowCamPos.y <=5)
            {
                Debug.Log("STOP!");
                mover_State = Mover_State.Grounded;
            }
            
            //FIRST, Is JumpMan's position invalid?    
            

            probeOffset = 0;
            probeRay = new Ray(camPos, (new Vector3(probePos.x, probePos.y - probeOffset, probePos.z) - camPos));
            Physics.Raycast(probeRay, out hitC, drawDis, layerMask);
            //Debug.DrawRay(camPos,(probeOffsetDir-camPos),Color.yellow,2);

            while (Physics.Raycast(camPos, (new Vector3(probePos.x, probePos.y - probeOffset, probePos.z) - camPos), out hitC, drawDis, layerMask) == false && probeOffset <45)
            {
                probeOffset = probeOffset + 0.001f;
                Debug.Log("probe is scanning...");
                Debug.DrawRay(camPos, (new Vector3(probePos.x, probePos.y - probeOffset, probePos.z) - camPos), Color.white, 1);

            }
            if (hitC.collider.gameObject.layer == 8)
            {
                Debug.Log("probe found something");
                Debug.DrawRay(camPos, (hitC.point - camPos), Color.magenta, 5);
                edgeBellowCamPos = cam.WorldToScreenPoint(hitC.point);
                edgeRay = new Ray(camPos, (hitC.point - camPos));
            }
            



            //STORE moverPos relative to camview XY
            //cast ray through moverPos, store hitStored for comparative
            //rayRot = rayRot + .0001x
            //cast same ray with rotation, compare hit to hitStored
            //      while newhit.distance ~= hitStored.distance && normals are same, rayRot = rayRot + .0001x
            //      if newhit.distance < hitStored.distance store hit in ANOTHER rayhit, rayRot = rayRot + .0001x
            //          if next hit.distance ~= then found EDGE!
            // STORE EDGE HIT AND relative camview XY, if MoverPos Y - edge hit Y < Threshold, STOP FALLING!
            // COMPARE camview Y Values.
            // Start falling if moverPos Y > threshold.




            //bool cHasHit = Physics.BoxCast(moverPos, GroundBoxCastSize, (moverPos - camPos), out hitC, cam.transform.rotation, drawDis, layerMask);
            //bool aHasHit = Physics.BoxCast(moverPos, ProbeBoxCastSize, (moverPos - camPos), out hitA, probeRotationA , drawDis, layerMask);
            //LineCast that scans downward by rotating downward from cam.transform.rotation, at set increments. store new RayCastHit.hitDown1 if its an edge (DO MAGIC HERE!)
            //send probe down again, compare hit to stored hit until a new unique edge is found, store hitDown2. (THESE NEED TO BE NEW STORED VALUES)
            //once hitDown2=true, then let gravity take JumpMan until screenpoint is close to hitDown1 screenpoint for y! 

            //Now do all this in reverse for rising? or do the same scan for above... probable should actually.
            //      Ok so now that we've got all that scanning working and not buggy at all we can incorporate a nav function that does a similar version as above.

            //bool bHasHit = Physics.BoxCast(moverPos, ProbeBoxCastSize, (moverPos - camPos), out hitB, cam.transform.rotation, drawDis, layerMask);

            //bool probeHasHit = cHasHit || aHasHit || bHasHit;

            // if (probeHasHit)
            // {
            //     Debug.Log("hit Something");



            //     //SECOND, If the player doesn't want to land or be grounded. This check can dislodge them.
            //     if (verMov < 0 && Input.GetMouseButtonDown(0)) // TO DO - set this mouse input to a value that times out.
            //     {
            //         mover_State = Mover_State.Falling; // This check affectively disables grounding probe if true.
            //     }

            //     //THIRD, should JumpMan be slow falling? If so, gravity velocity should be halved - simulating falling through an object but also reducing velocity to prepare to land.    
            //     if (bHasHit)
            //     {
            //         if (hitB.collider.gameObject.layer == 8 && mover_State == Mover_State.Falling)
            //         {
            //             SlowFalling();
            //             Debug.Log("Slowing!");
            //         }
            //     }

            //     //FOURTH, Grounding
            //     if (cHasHit)
            //     {
            //         Debug.Log(hitC.collider.gameObject.layer);
            //         if (hitC.collider.gameObject.layer == 8 && mover_State == Mover_State.Falling
            //         || hitC.collider.gameObject.layer == 8 && hitC.distance - hitB.distance < hitA.distance - hitC.distance //Does rayC hit a platform and is it the top of a ledge?
            //         || hitC.collider.gameObject.layer == 8 && hitC.distance > hitA.distance && hitC.distance > hitB.distance) //JM on a Wire
            //         {
            //             mover_State = Mover_State.Grounded;
            //             Debug.DrawRay(camPos, hitC.point - camPos, Color.red);
            //         }
            //     }

            //     //FIFTH, if these probe conditions are ever met, he should be falling.
            //     if (cHasHit == false)
            //             //|| Mathf.Approximately(hitA.distance - hitC.distance, hitC.distance - hitB.distance))
            //     {
            //         Debug.Log("going to fall now...");
            //         mover_State = Mover_State.Falling;
            //     }


            //         Debug.DrawRay(camPos, hitC.point - camPos, Color.yellow);
            //         Debug.DrawRay(camPos, hitA.point - camPos, Color.yellow);
            //         Debug.DrawRay(camPos, hitB.point - camPos, Color.yellow);


            // }

            // else
            // {
            //     mover_State = Mover_State.Falling;
            //     Debug.DrawRay(camPos, moverPos - camPos, Color.red);
            //     Debug.Log("No Hit.");
            // }
        }
    }
    private void Grounding()
    {
        groundDis = moverCamPos.y-edgeBellowCamPos.y;
        if (groundDis <= 5 && mover_State == Mover_State.Falling)
        {
            Debug.Log("Landed!");
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            mover_State = Mover_State.Grounded;
        }
        if (Mathf.Abs(groundDis) > 5 && mover_State == Mover_State.Grounded)
        {
            Debug.Log("Actually, we falling...");
            mover_State = Mover_State.Falling;
        }
    }

    private void DeathCheck()
    {
        if (moverCamPos.y <= 1)
        {
            gameObject.transform.position = startPos;
            deathCount = deathCount + 1;
            Debug.Log("Death to you!");
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            
        }
    }

    // private void SlowFalling()
    // {
    //     rb.AddRelativeForce(new Vector3(0, -rb.velocity.y / 2, 0));
    // }
    // private void NavProbe()
    // {
    //     // TO DO Check for transitional conditions for hopping and stepping up/down.
    // }
    // private void UpdateJumpManPos()
    // {
    //     // TO DO JumpMan position (JumpManPos) is always transformed along the probe.
    //     //If Grounded, JumpManPos is set to hit pos.
    //     //If NOT Grounded, JumpManPos is 1m from Camera. (Mover remains between and falls due to gravity, probe is 'scanning')
    // }

    private void StateConditions()
    {
        if (mover_State != prevMover_State)
        {
            prevMover_State = mover_State;
            MoverStates();
        }


    }

    private void MoverStates() //Send here to force MoverState Check/fix
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
        // if (mover_State == Mover_State.Rising)
        // {
        //     rb.useGravity = true;
        //     Debug.Log("current state is Rising");
        // }
        if (mover_State == Mover_State.Falling)
        {
            //rb.useGravity = true;
            Debug.Log("current state is Falling");
        }

    }
    // private void OnDrawGizmosSelected() {
    //             //Gizmos.DrawLine(camPos,new Vector3(probePos.x,probePos.y-probeOffset,probePos.z));
    //             if (hitC.collider.gameObject.layer==8)
    //             {
    //                 Gizmos.color = Color.green;
    //             }
    //             else
    //             {
    //             Gizmos.color = Color.yellow;
    //             }
    //             Gizmos.DrawLine(camPos,new Vector3(probePos.x,probePos.y-probeOffset,probePos.z));
    // }
}
