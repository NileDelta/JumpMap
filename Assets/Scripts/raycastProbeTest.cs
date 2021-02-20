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
    Vector3 jMPos;
    Vector3 camPos;
    GameObject jumpMan0;
    GameObject jumpMan1;
    Ray rayA;
    Ray rayB;
    Ray rayC;
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
        jumpMan0 = GameObject.Find("JumpMan");
        
    }

    void Update()
    {

        jMPos = jumpMan0.transform.position;
        camPos = gameObject.transform.position;
        rayAOrigin.y = camPos.y + probeSeparation;
        rayCOrigin.y = camPos.y - probeSeparation;
        UpdateRays();
        DebugRays();
        
        ProbeStatments();

        //Debug.DrawRay(camPos, (jMPos-camPos)*1, Color.yellow); //using this direction create 3 probes, A, B, and C which offset along y 0.01 and condition them so that:
        //if distance of rayA,B,and C are all equal and or all hitting obstacles then move jMPos up along y axis. (move probe up to search for obstacle edge)
        //make rays ignore player somehow...
        //if A/=B and B=C then jMPos is good and placed on an edge
        //if A B and C are infinite (or really really large) then ??? rely on physics body to bring jumpman down? consider this T=Threshold
        // PROBLEM WITH PREVIOUS CONDITION -> what if jumpMan jumps above the horizon line. will be a challenge to condition.
    }

    void ProbeStatments()
    {
        
        if (probeADis == probeBDis || probeBDis == probeCDis || probeADis>thresholdDis)
        {
            //staring off into space (ACTION move jM0 down (with gravity in the future) to find obstructions) may have special conditions needed for future code
            Debug.Log("staring off into space");
        }
        else if (probeADis == probeBDis) //check A is equal to B (hitting a wall)
        {
            if (probeBDis == probeCDis)
            {
                //staring at a flat vertical plane (ACTION move jM0 down (with gravity in the future) to find obstructions)
            }
            else if (probeBDis > probeCDis)
            {
                //staring at the botton wall edge (ACTION nothing, jM1 can exist but will likely get bumped forward a bit from wall collision(autoscaler should adjust size and rb conditions))    
            }
            else
            {
                //staring at the bottom edge of a cantalevered obstruction (ACTION move jM0 down (with gravity in the future) to find obstructions)
            }
        }
        else if (probeADis > probeBDis) //check A is father than B
        {
            if (probeBDis == probeCDis)
            {
                //staring at top edge of a wall (ACTION nothing, jM1 can exist here)
            }
            else if (probeBDis > probeCDis)
            {
                //staring at surface top (ACTION nothing, jM1 can exist here)
            }
            else
            {
                //staring at a line (ACTION nothing, jM1 can exist on a line)
            }
        }
        else if (probeADis < probeBDis) //check A is closer than B
        {
            if (probeBDis == probeCDis)
            {
                //staring at top of wall adjoined to cantilever (ACTION move jM0 down (with gravity in the future) to find obstructions))
            }
            else if (probeBDis > probeCDis)
            {
                //this is a relief... literally. Not sure what to do with this yet but would be cool to allow jumpman to use negative space as a 'hardedge' or line to walk on
                //for now (ACTION nothing, assume obstructions below and let jM0 down)
            }
            else
            {
                //staring at underside of surface (ACTION move jM0 down to find obstructions)
            }
        }
    }



    void UpdateRays()
    {
        rayB = new Ray(camPos, (jMPos - camPos));
        rayA = new Ray(rayAOrigin, (jMPos - camPos));
        rayC = new Ray(rayCOrigin, (jMPos - camPos));
    }
    
    void DebugRays()
    {
        if (Physics.Raycast(rayB, out hitBInfo, 1000, Obstacles)) { //Ray is limited to 1000(1km), maybe make this serialized? 
            Debug.DrawLine(rayB.origin, hitBInfo.point, Color.red);
            probeADis = Vector3.Distance(rayA.origin, hitAInfo.point);
            Debug.Log("probe A Distance is " + probeADis);
        } else { //probe only searches for objects on the Obstruction LayerMask. 
            Debug.DrawLine(rayB.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeADis = thresholdDis;
        }//ray probe references jumpMan0, jumpMan1 references probe, player input alters jumpMan1 position, jumpMan0 references jumpMan1Pos

        if (Physics.Raycast(rayA, out hitAInfo, 1000, Obstacles))
        {
            Debug.DrawLine(rayA.origin, hitAInfo.point, Color.red);
            probeBDis = Vector3.Distance(rayB.origin, hitBInfo.point);
            Debug.Log("probe B Distance is " + probeBDis);
        }
        else
        {
            Debug.DrawLine(rayA.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeBDis = thresholdDis;
        }

        if (Physics.Raycast(rayC, out hitCInfo, 1000, Obstacles))
        {
            Debug.DrawLine(rayC.origin, hitCInfo.point, Color.red);
            probeCDis = Vector3.Distance(rayC.origin, hitCInfo.point);
            Debug.Log("probe C Distance is " + probeCDis);
        }
        else
        {
            Debug.DrawLine(rayC.origin, rayB.origin + rayB.direction * 1000, Color.green);
            probeCDis = thresholdDis;
        }
    }
}
