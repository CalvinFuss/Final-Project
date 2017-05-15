using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This script is attached to the main camera. It follows the player

public class CameraFollowPlayer : MonoBehaviour {

    public GameObject player;  
    float Speed = 0.8f;

    public void Start()
    {
    

    }

    public void Update()
    {
        Vector3 pos = player.transform.position;
      
       transform.position = new Vector3(pos.x, 115, pos.z-100);
        // Zooms Camera out at start of game
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 26, Speed * Time.deltaTime); 

    }

}
