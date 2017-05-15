using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Attached to the canvas within the agent prefab
// Rotates canvas attached to agent to face Main Camera
public class CanvasFaceCamera : MonoBehaviour {
    public Camera cameraPos;

    void Start()
    {
        cameraPos = Camera.main;
       
    }

    void Update()
    {
        Vector3 UI = cameraPos.transform.position - transform.position; // Gets direction
        UI.x = UI.z = 0.0f;
        transform.LookAt(cameraPos.transform.position - UI); // Rotates canvas
        transform.Rotate(0, 0, 0); // Corrects rotation 
       
    }
}
