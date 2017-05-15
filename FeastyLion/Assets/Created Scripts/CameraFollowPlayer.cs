using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    // Use this for initialization
    public GameObject player;

    
    float Speed = 0.8f;

    public void Start()
    {
    

    }




    public void Update()
    {
        Vector3 pos = player.transform.position;
      
       transform.position = new Vector3(pos.x, 115, pos.z-100);
       Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 26, Speed * Time.deltaTime); // Zooms Camera out at start of game

    }

}
