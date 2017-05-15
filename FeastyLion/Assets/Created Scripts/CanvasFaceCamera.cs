using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour {
    public Camera cameraPos;

    void Start()
    {
        cameraPos = Camera.main;
       
    }

    void Update()
    {
        Vector3 UI = cameraPos.transform.position - transform.position;
        UI.x = UI.z = 0.0f;
        transform.LookAt(cameraPos.transform.position - UI);
        transform.Rotate(0, 0, 0);
       
    }
}
