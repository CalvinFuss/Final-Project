using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script rotates an arrow game object towards the closest enemy postion off screen
public class EnemyArrow : MonoBehaviour {

 


    Vector3 targetPos;

    PlayerController playerController;
    
    public Transform player;
    
    MeshRenderer[] arrow;
    RectTransform rectTransform;
    private Quaternion _lookRotation;


    void Start () {

        arrow = transform.GetComponentsInChildren<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        
    }
	

	void Update () {

        // Get's neareast game object from the 'PlayerController' class

        // if distance of closest object is less than 50 units
        if (Vector3.Distance(playerController.closest.transform.position, transform.position) < 50) 
        {          
            foreach (MeshRenderer renderer in arrow )
            {
                if (renderer.name == "Arrow")
                    renderer.enabled = false; // Don't render transform
            }       
       
        }
        // if distance of closest object is greater than 50 units
        else if (Vector3.Distance(playerController.closest.transform.position, transform.position) > 50)
        {         
            foreach (MeshRenderer renderer in arrow)
            {
                if (renderer.name == "Arrow")
                    renderer.enabled = true; // Render arror
            }           
            transform.position = player.transform.position;
            transform.LookAt(playerController.closest.transform.position); // Rotate arrow towards nearest game object
       
        }
       
    }

    
}


