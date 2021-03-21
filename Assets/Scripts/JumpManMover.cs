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

    Rigidbody rb;
    float HorMov;
    float VerMov;
    public enum States
    {
        Rising,
        Falling,
        Grounded,
        Crouched

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // TO DO assign component variables for:
            //Vehicle transform
            //Camera transform
    }

    void FixedUpdate()
    {
        MoverInput();
    }

    void Update() 
    {
        GroundingProbe();
        NavProbe(); //Used to evaluate the immediate conditions that the player can use to change jumpman pos, EX: moving up and down stairs, taking large steps or hops laterally or vertically
        UpdateMoverPos();
        UpdateJumpManPos();
        
       
    }
    private void MoverInput()
    {
        HorMov = Input.GetAxisRaw("Horizontal");
        VerMov = Input.GetAxisRaw("Vertical"); //will be used for NavProbe and conditional movements
        
        // TODO use jumping tutorial to do press button trick. use value for Y Velocity    
    }
    private void GroundingProbe()
    {
        // TODO
            //cast ray from cam origin to JumpManMover
            //cast 2 parallel rays, slightly above and slightly below
            //compare hit values and object layers to approve jumpman grounding.
            //if probe is invalid, jumpman is NOT grounded and Mover has gravity.
            //JumpMan DOES NOT HAVE PHYSICS! probes indicate his behaviour.
            
    }
    private void NavProbe()
    {

    }
    
    private void UpdateMoverPos()
    {
        // TO DO Using available player inputs and conditions based on grounding states and probe conditions, translate to Mover movement.
        rb.velocity = new Vector3(HorMov, VerMov, rb.velocity.z) * speed;
        // TO DO convert this vector to be dependent on Vehicle transformations to adjust for vehicle rotation in world.
    }

    private void UpdateJumpManPos()
    {
        // TO DO JumpMan position (JumpManPos) is always transformed along the probe.
            //If Grounded, JumpManPos is set to hit pos.
            //If NOT Grounded, JumpManPos is 1m from Camera. (Mover remains between and falls due to gravity, probe is 'scanning')
    }
}
