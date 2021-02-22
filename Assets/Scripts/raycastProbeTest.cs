using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastProbeTest : MonoBehaviour
{

    //  consider 2 jumpMan exist at all times, jumpMan0 and jumpMan1
    // jumpMan0 is placed 1m from camera and acts as a guide for the probes needed to analyse depth and is also where jumpMan animations 'render'
        //jumpMan0 is locked to camera's local XY axis
    // jumpMan1 is the physics body that has mass and gravity that react proportionally to where he's spawned.
        // his spawn is dependent on the probe mechanics that use the rays calculated by jumpMan0's vector relating to the Camera vector.
        // jM1 will be repositioned according to probe rules defined bellow, jM0 will have his position updated to jM1's screen position in realtime.

    
    [SerializeField] float probeSeparation = 0.01f;
    [SerializeField] float thresholdDis = 999f;
    [SerializeField] float jMSpeed = 5f;
    [SerializeField] float physicsScaling = 2f;
    Camera cam;
    Vector3 jM0Pos;
    Vector3 jM0Postest;
    Vector3 jM1Pos;
    float jM1Dis;
    Vector3 camPos;
    GameObject jumpMan0;
    GameObject jumpMan1;
    Ray rayA;
    Ray rayB;
    Ray rayC;
    Ray jM1Ray;
    float scalingRatio;
    public LayerMask Obstacles;
    Vector3 rayAOrigin;
    Vector3 rayCOrigin;
    float probeADis;
    float probeBDis;
    float probeCDis;
    int probeDirection;
    RaycastHit hitBInfo;
    RaycastHit hitAInfo;
    RaycastHit hitCInfo;
    float jMxMov;
    float jMzMov;
    Rigidbody jMrb;
    bool jMFalling;
    bool fallingCheck; //use this bool at the if statements to condition if jM is in the middle of falling or not. to make him teleportable.


    void Start()
    {
        cam = GetComponent<Camera>();
        jumpMan0 = GameObject.Find("JumpMan0");
        jumpMan1 = GameObject.Find("JumpMan1");
        jMrb = jumpMan1.GetComponent<Rigidbody>();
        jumpMan1.transform.position = (jumpMan0.transform.position+new Vector3(0,0,0.1f));
        jMFalling = true;
        //jM1 will spawn 1cm behind jM0 and begin searching for new landings.
    }

    void Update()
    {
        
        MoveJumpMan();
        UpdatePositions();
        CreateRays();
        LookAtCam();

        UpdateRays();
        PlaceJM0();
        ProbeStatments();
        //if Probe finds valid position for jM1 teleport him to (hitBInfo.point + probeseparation in y Axis) and scale appropriately to the landscape. flip portable bool to give control to player.
        //if probe state changes due to player input, flip portable state and VERIFY with probe if new walkable surface is nearby (for now let jumpMan fall)
        //if player input is Jump apply force to jM1 upward - condition in future to scan for available platforms on horizontals along jM1 vector.
        //if player input is DOWN+JumpButton, teleport jM1 forward to hitBInfo.point and let fall.
        //if player input is DOWN+JumpButton while falling ignore found walkable surfaces detected - allows player to choose platforms vertically.
        //if Player input is left/right, move jM1 left or right using forces applied perpendictable to hibBInfo.normal direction
        //previous function keeps jumpman from prematurely walking off a surface edge. (may have to create a second probe projecting from jM1 to find 3D edges and keep him affixed to the surface)

    }

    private void CreateRays()
    {
        rayAOrigin = camPos;
        rayCOrigin = camPos;
        rayAOrigin.y = camPos.y + probeSeparation;
        rayCOrigin.y = camPos.y - probeSeparation;
    }

    private void UpdatePositions()
    {
        jM0Pos = jumpMan0.transform.position;
        jM1Pos = jumpMan1.transform.position;
        camPos = gameObject.transform.position;
    }

    void LookAtCam()
    {
        Vector3 camLookAt0 = new Vector3(cam.transform.position.x, jumpMan0.transform.position.y, cam.transform.position.z);
        Vector3 camLookAt1 = new Vector3(cam.transform.position.x, jumpMan1.transform.position.y, cam.transform.position.z);
        jumpMan0.transform.LookAt(camLookAt0);
        jumpMan1.transform.LookAt(camLookAt1);
    }

    void MoveJumpMan()
    {
        jMxMov = Input.GetAxisRaw("Horizontal"); //Consider these as multipliers for real 3D world directions
        jMzMov = Input.GetAxisRaw("Vertical");
        jMrb.velocity = (new Vector3(jMxMov, jMrb.velocity.y, jMzMov) * jMSpeed * Mathf.Pow(scalingRatio,physicsScaling)) * Time.deltaTime;
        //TO DO: Change vectors to ?perpendicular? to the normal provided by surface hitBInfo
        if (jMFalling == true) {
            jMrb.useGravity = true; 
        } else {jMrb.useGravity = false; }
    }

    void ProbeStatments()
    {
        //ADD IF jMFalling = True, set gravity on and allow JM1 to be teleported (also referred to 'portable' later in notes)

                //****Flipping GRAVITY is NOT the solution. Lets create a game skeleton after teleporting state works.****

        //ADD Deltas to distances so we can compare AB to BC. if deltas are equal, staring at a flat surface(face)

        if (probeADis == probeBDis && probeBDis == probeCDis && probeADis>thresholdDis) {
            jMFalling = true; // may have special conditions needed for future code
            Debug.Log("staring off into space");
        } else if (probeADis == probeBDis) { //check A is equal to B (hitting a wall)
            if (probeBDis == probeCDis) {
                Debug.Log("This is a flat plane");
                jMFalling = true; 
            } else if (probeBDis > probeCDis) {
                Debug.Log("This is the bottom of a wall");
                jMFalling = false;
            } else { //probeBDis < probeCDis
                Debug.Log("This is the bottom edge of a protrusion");
                jMFalling = true; 
            }
        } else if (probeADis > probeBDis) {
            if (probeBDis == probeCDis) {
                Debug.Log("This is the top of a Wall or Object");
                jMFalling = false;
            } else if (probeBDis > probeCDis) {
                Debug.Log("This is the top of a surface"); //walking condition change in this statement so rb vector forces apply perpendicular to dumpman orientation (=world.y)
                jMFalling = false;
            } else { //probeBDis < probeCDis
                Debug.Log("This is a thin horizontal (a wire perhaps?)");
                jMFalling = false;
            }
        } else if (probeADis < probeBDis) {
            if (probeBDis == probeCDis) {
                Debug.Log("This is the top of a wall joined to a ceiling");
                jMFalling = true;
            } else if (probeBDis > probeCDis) {
                Debug.Log("This is a relief");
                jMFalling = true; //TO DO: how can JumpMan exist in negative space? is there a third 2D JumpMan that the user can play with in special conditions?
            } else {//probeBDis < probeCDis
                Debug.Log("This is the bottom of a surface");
                jMFalling = true;
            }
        }
    }



    void UpdateRays()
    {
        rayB = new Ray(camPos, (jM0Pos - camPos));
        rayA = new Ray(rayAOrigin, (jM0Pos - camPos));
        rayC = new Ray(rayCOrigin, (jM0Pos - camPos));
        jM1Ray = new Ray(camPos, (jM1Pos - camPos));
        castRayB();
        castRayA();
        castRayC();
    }
    
    void PlaceJM0()
    {
        jM1Dis = Vector3.Distance(camPos,jM1Pos);
        scalingRatio = 1/(jM1Dis);
        jumpMan0.transform.position = Vector3.Lerp(camPos,jM1Pos,scalingRatio); 
        jumpMan1.transform.localScale = new Vector3(1/scalingRatio,1/scalingRatio,1/scalingRatio);
    }
    void castRayB()
    {
        if (Physics.Raycast(rayB, out hitBInfo, thresholdDis, Obstacles)) { 
            Debug.DrawLine(rayB.origin, hitBInfo.point, Color.red);
            probeBDis = Vector3.Distance(rayB.origin, hitBInfo.point);
        } else { //probe only searches for objects on the Obstruction LayerMask. 
            Debug.DrawLine(rayB.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeBDis = thresholdDis;
        }//ray probe references jumpMan0, jumpMan1 references probe, player input alters jumpMan1 position, jumpMan0 references jumpMan1Pos
    }
    void castRayA()
    {
        if (Physics.Raycast(rayA, out hitAInfo, 1000, Obstacles))
        {
            Debug.DrawLine(rayA.origin, hitAInfo.point, Color.red);
            probeADis = Vector3.Distance(rayA.origin, hitAInfo.point);
        } else {
            Debug.DrawLine(rayA.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeADis = thresholdDis;
        }
    }
    void castRayC()
    {
        if (Physics.Raycast(rayC, out hitCInfo, 1000, Obstacles))
        {
            Debug.DrawLine(rayC.origin, hitCInfo.point, Color.red);
            probeCDis = Vector3.Distance(rayC.origin, hitCInfo.point);
        } else {
            Debug.DrawLine(rayC.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeCDis = thresholdDis;
        }
    }
}
