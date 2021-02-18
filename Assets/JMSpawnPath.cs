using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JMSpawnPath : MonoBehaviour
{
    GameObject JMSpawnMarker;
    GameObject Camera;
    GameObject JumpMan;
    Vector3 CamPos;
    Vector3 JMSMPos;
    Vector3 JMPos;
    Vector3 JMDirection;
    int ProbeA;
    int ProbeB;
    int ProbeC;
    Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        var Camera = GameObject.Find("Camera");
        var JMSpawnMarker = GameObject.Find("JMSpawnMarker");
        var JumpMan = GameObject.Find("JumpMan");
        
        
    }

    // Update is called once per frame
    void Update()
    {
        CamPos = GameObject.Find("Camera").transform.position;
        JMSMPos = GameObject.Find("JMSpawnMarker").transform.position;
        JMPos = GameObject.Find("JumpMan").transform.position;
        JMDirection = CamPos-JMSMPos;
        ray = new Ray(CamPos, JMDirection);
        Debug.Log("CamPos is"+ CamPos);
        Debug.Log("JMSMPos is" + JMSMPos);
        Debug.Log("JMPos is" + JMPos);
        RaycastHit ProbeDisA;

        if (Physics.Raycast (ray, out ProbeDisA))
        {
            Debug.Log("Drawing line");
            Debug.DrawLine(ray.origin, ProbeDisA.point, Color.red, 2, false);
        }
        else {
            Debug.Log("Not drawing Line");
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green,2, false);
        }
    }
}
