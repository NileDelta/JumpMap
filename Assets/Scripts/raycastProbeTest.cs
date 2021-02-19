using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastProbeTest : MonoBehaviour
{

    //  consider 2 jumpMan exist at all times, jumpMan0 and jumpMan1
    // jumpMan0 is placed 1m from camera and acts as a guide for the probes needed to analyse depth and is also where jumpMan animations 'render'
        //jumpMan0 is locked to camera's local XY axis
    // jumpMan1 is the physics body that has mass and gravity that react proportionally to where he's spawned.
        // his spawn is dependent on the probe mechanics that use the rays calculated by jumpMan0's vector.
        // jM1 will be repositioned according to probe rules defined bellow, jM0 will have his position updated to jM1's screen position in realtime.

    

    Camera cam;
    Vector3 jMPos;
    Vector3 camPos;
    GameObject jumpMan0;

    void Start()
    {
        cam = GetComponent<Camera>();
        jumpMan0 = GameObject.Find("JumpMan");
    }

    void Update()
    {
        
        jMPos = jumpMan0.transform.position;
        camPos = gameObject.transform.position;
        Debug.DrawRay(camPos, (jMPos-camPos)*1, Color.yellow); //using this direction create 3 probes, A, B, and C which offset along y 0.01 and condition them so that:
        //if distance of rayA,B,and C are all equal and or all hitting obstacles then move jMPos up along y axis.
        //make rays ignore player somehow...
        //if A/=B and B=C then jMPos is good and placed on an edge
        //if A B and C are infinite (or really really large) then ??? rely on physics body to bring jumpman down?
        //
    }

}
