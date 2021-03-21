using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//README
// Jumpman Mover is the physics handler for the JumpMan Character. 
// His actions and animations are translated from values altered by this script such as JumpMan's Scale, speed, and orientation.
// The Mover object is fixed to the vehicle, and the probe rays anchor to the camera view and the Mover object to project the JumpMan character.
public class JumpManMover : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float drawDis = 1000f;

    Rigidbody rb;
    Camera cam; 
    GameObject vehicle;

    
    //VECTORS
    Vector3 moverPos;
    Vector3 camPos;
    Vector3 start;


    //RAYS & RAYCASTHITS
    Ray rayC;
    RaycastHit hitC;
    Ray rayA;
    RaycastHit hitA;
    Ray rayB;
    RaycastHit hitB;


    //INPUT/CONTROLLER VALUES
    float HorMov;
    float VerMov;


    public LayerMask Nothing;
    public LayerMask Obstruction;
    public LayerMask Platform;
    public LayerMask Respawn;
    public LayerMask Finish;

    //STATE CONDITIONS
    public enum Mover_State
    {
        Rising,
        Falling,
        Grounded,
        Crouched
    }
    public enum GroundingProbe_State //Same as Unity Object Tag Names.
    {
        Untagged,
        Obstruction,
        Object,
        Respawn,
        Finish
    }

//==============================================================================================================================================================
    void Start()
    {
        start = gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
        vehicle = GameObject.Find("Vehicle");
        cam = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        MoverInput();
        UpdatePositions();
        GroundingProbe();
    }

    void Update() 
    {
        NavProbe(); //Used to evaluate the immediate conditions that the player can use to change jumpman pos, EX: moving up and down stairs, taking large steps or hops laterally or vertically
        UpdateMoverPos();
        UpdateJumpManPos();
        
       
    }
    private void MoverInput()
    {
        HorMov = Input.GetAxisRaw("Horizontal");
        VerMov = Input.GetAxisRaw("Vertical"); //will be used for NavProbe and conditional movements
        
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
            if(Physics.Raycast(rayC, out hitC, drawDis, Respawn))
            {
                Debug.DrawRay(rayC.origin,rayC.direction,Color.red);
                gameObject.transform.position = start;
            }
            else if (Physics.Raycast(rayC, out hitC, drawDis, Platform)) 
            {
                rb.useGravity = false;
                Debug.DrawRay(rayC.origin,rayC.direction,Color.green);
                Debug.Log("platform here!");
            }
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
    private void NavProbe()
    {
            // TO DO Check for transitional conditions for hopping and stepping up/down.
    }
    
    private void UpdateMoverPos()
    {
        // TO DO Using available player inputs and conditions based on grounding states and probe conditions, translate to Mover movement.
        rb.velocity = new Vector3(HorMov, VerMov, rb.velocity.z) * speed;
        moverPos = transform.position;
        // TO DO convert this vector to be dependent on Vehicle transformations to adjust for vehicle rotation in world.
    }

    private void UpdateJumpManPos()
    {
        // TO DO JumpMan position (JumpManPos) is always transformed along the probe.
            //If Grounded, JumpManPos is set to hit pos.
            //If NOT Grounded, JumpManPos is 1m from Camera. (Mover remains between and falls due to gravity, probe is 'scanning')
    }
}
