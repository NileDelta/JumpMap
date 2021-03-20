using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpManMover : MonoBehaviour
{
    // Mover Architecture Layout

    // 1. Place jumpMan to center of CamView, 1m from MainCamera.
    // 2. Set jumpMan 'mState' to "Falling" (has gravity).
    // 3. Extend JProbe through jumpManPos.
    //      1.A. Condition death, if JProbe hits Sill object/layer.
    //      1.B. Respawn jumpMan to center, set mState to "Falling", and add to death counter.
    // 3. If JProbe condition = Edge, set Landed to True. Await Player Input.
    //      3.A. Create conditional statements from probe to determine local conditions around jumpMan.
    // 4. Capture player input.
    //      4.A. Horizontal value (+/-)
    //      4.B. Vertical Value (+/-)
    //      4.C. Primary Button (A)
    //      4.D. Secondary Button (B)
    // 5. mState = "RISING" (from Jump or otherwise) IF Vertical (++)
    //      5.A. Gravity is active, Jprobe is ignored without player input.
    //      I. Horizontal (+/-) : airbrake left/right
    //      II. Vert (+) : Add momentum up
    //      III. Vert (-) : Slow Momentum up
    //      IV. [Hold (B)] : Land on next available surface found by JProbe
    // 6. mState = "FALLING" (from Jump, Spawn, or otherwise) IF Vertical (--)
    //      6.A. Gravity is active, Jprobe will Land player on next "edge" without input.
    //          I. Horizontal (+/-) : airbrake left/right
    //          II. Vert (+) : Slow momentum down
    //          III. Vert (-) : Add Momentum Down
    //          IV. [Hold (A)] : Phase through available landings/edges.
    // 7. mState = "Landed" (idle, walking, or running) JProbe is active and locked on an applicable edge.
    //      7.A. Gravity is inactive, jumpMan0 Pos translates toward NavProbe along Projection Plane.
    //          I. Horizontal (+/-) : move NavProbe along projection Plane and update FocusPos (JumpMan Head Tilt).
    //          II. Vert (+) : NavProbe searches for available "steps" further in the landscape, if available NavProbe moves up and JumpMan animates walking up stairs (away from camera).
    //          III. Vert (-) : NavProbe searches for available "steps" closer in the landscape, if available NavProbe moves down and JumpMan animates walking down stairs (towards camera). 
    //          IV. [Tap (A)] : small jump force applied, set mState to "Rising"
    //          V. [HOLD (A)] : Large jump force applied dependent on length of press up to a threshold amount, set mState to "Rising"
    //          VI. [Vert (-) + (A)] : Phase down through current platform, set mState to "FALLING" see 6.A.IV.
    //          VII. [Hold B] : Crouch OR Slide IF JumpMan has momentum above threshold. Crouch reduces speed so that full 'run' speed is below slide threshold.
    //          VIII. [Tap (B) + Hold (A)] : Huge jump force applied with additional conditions found in 7.A.V.


    //Steps
    // 1. Create JumpMan, lock position to 1m away from camview.
    // 2. set jumpManPos to center of camview.
    // 3. copy probe script to create jProbe through jumpManPos.
    // 4. turn jumpMan gravity on. (turn air friction lower?)
    // 5. Create Sill object
    // 5. If JProbe hits Sill set jumpManPos to center of camview.
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
