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

    
    [SerializeField] float probeSeparation = 0.01f; //use this value with camPos to dertermine new origins for rays probeA, probeC, where probeB origin = camera position
    [SerializeField] float thresholdDis = 999f;
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

    void Start()
    {
        cam = GetComponent<Camera>();
        jumpMan0 = GameObject.Find("JumpMan0");
        jumpMan1 = GameObject.Find("JumpMan1");
        //jumpMan1.transform.position = (jumpMan0.transform.position+new Vector3(0,0,0.1f));
        //MOVE jM1 1mm (along z Value) away from jM0 and let fall - this will begin earching for horizontals to place jM1 into the landscape.
    }

    void Update()
    {
        //Make jM0 spawn between jM1 @1m away from Camera at all times.
            //find vectorline between jM1Pos and camPos, find vector 1m away from camPos along this vectorline.
        //jM0 position updated to match jM1: jumpMan0.transform.position == jumpMan1.transform.position-hitBinfo.point => need to consider this calculation
            //ADD probeSeperation so jM0/jM1 spawn on hitBInfo.point+probeseparation and probe still tests jM's feet.

        //update jM1 scale depending on jM0 position and forced perspective calculation.
        
        //if jM1 is falling set a bool to set jM1 into a quantum state ready to be teleported: portable = true/false
        
        jM0Pos = jumpMan0.transform.position;
        jM1Pos = jumpMan1.transform.position;
        camPos = gameObject.transform.position;
        rayAOrigin = camPos;
        rayCOrigin = camPos;
        rayAOrigin.y = camPos.y + probeSeparation;
        rayCOrigin.y = camPos.y - probeSeparation;
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

    void ProbeStatments()
    {
        
        if (probeADis == probeBDis && probeBDis == probeCDis && probeADis>thresholdDis)
        {
            //staring off into space (ACTION move jM0 down (with gravity in the future) to find obstructions) may have special conditions needed for future code
            Debug.Log("staring off into space");
        }
        else if (probeADis == probeBDis) //check A is equal to B (hitting a wall)
        {
            if (probeBDis == probeCDis)
            {
                //staring at a flat vertical plane (ACTION move jM0 down (with gravity in the future) to find obstructions)
                Debug.Log("This is a flat plane");
            }
            else if (probeBDis > probeCDis)
            {
                //staring at the botton wall edge (ACTION nothing, jM1 can exist but will likely get bumped forward a bit from wall collision(autoscaler should adjust size and rb conditions))    
                Debug.Log("This is the bottom of a wall");
            }
            else
            {
                //staring at the bottom edge of a cantalevered obstruction (ACTION move jM0 down (with gravity in the future) to find obstructions)
                Debug.Log("This is the bottom edge of a protrusion");
            }
        }
        else if (probeADis > probeBDis) //check A is father than B
        {
            if (probeBDis == probeCDis)
            {
                //staring at top edge of a wall (ACTION nothing, jM1 can exist here)
                Debug.Log("This is the top of a Wall or Object");
            }
            else if (probeBDis > probeCDis)
            {
                //staring at surface top (ACTION nothing, jM1 can exist here)
                Debug.Log("This is the top of a surface");
            }
            else
            {
                //staring at a line (ACTION nothing, jM1 can exist on a line)
                Debug.Log("This is a thin horizontal (a wire perhaps?)");
            }
        }
        else if (probeADis < probeBDis) //check A is closer than B
        {
            if (probeBDis == probeCDis)
            {
                //staring at top of wall adjoined to cantilever (ACTION move jM0 down (with gravity in the future) to find obstructions))
                Debug.Log("This is the top of a wall joined to a ceiling");
            }
            else if (probeBDis > probeCDis)
            {
                //this is a relief... literally. Not sure what to do with this yet but would be cool to allow jumpman to use negative space as a 'hardedge' or line to walk on
                //for now (ACTION nothing, assume obstructions below and let jM0 down)
                Debug.Log("This is a relief");
            }
            else
            {
                //staring at underside of surface (ACTION move jM0 down to find obstructions)
                Debug.Log("This is the bottom of a surface");
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
        //find new vector3 location to transform.position jM0PosTest to.
        scalingRatio = 1/(jM1Dis);
        Debug.Log("This is the scaling ratio" + scalingRatio);
        Debug.Log("measured from this distance" + jM1Dis);
        jumpMan0.transform.position = Vector3.Lerp(camPos,jM1Pos,scalingRatio); //find the scaling ratio to replace 0.5f. == 1/Vector3.distance(camPos,jM1Pos)
    }
    void castRayB()
    {
        if (Physics.Raycast(rayB, out hitBInfo, thresholdDis, Obstacles)) { //may need to modify the threshold dis calculation here
            Debug.DrawLine(rayB.origin, hitBInfo.point, Color.red);
            probeBDis = Vector3.Distance(rayB.origin, hitBInfo.point);
            Debug.Log("probe B Distance is " + probeBDis);
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
            Debug.Log("probe A Distance is " + probeADis);
        } else {
            Debug.DrawLine(rayA.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeADis = thresholdDis;
            Debug.Log("probe A Distance is " + probeADis);
        }
    }
    void castRayC()
    {
        if (Physics.Raycast(rayC, out hitCInfo, 1000, Obstacles))
        {
            Debug.DrawLine(rayC.origin, hitCInfo.point, Color.red);
            probeCDis = Vector3.Distance(rayC.origin, hitCInfo.point);
            Debug.Log("probe C Distance is " + probeCDis);
        } else {
            Debug.DrawLine(rayC.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeCDis = thresholdDis;
        }
    }
}
