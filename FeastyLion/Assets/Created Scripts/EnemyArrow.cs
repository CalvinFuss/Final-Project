using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This 
public class EnemyArrow : MonoBehaviour {

 


    Vector3 targetPos;

    PlayerController playerController;
    
    public Transform player;
    //  public RectTransform arrow;
    MeshRenderer[] arrow;
    RectTransform rectTransform;
    private Quaternion _lookRotation;

    // Use this for initialization
    void Start () {

        arrow = transform.GetComponentsInChildren<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        
    }
	
	// Update is called once per frame
	void Update () {
      
        
        if (Vector3.Distance(playerController.closest.transform.position, transform.position) < 50)
        {          
            foreach (MeshRenderer renderer in arrow )
            {
                if (renderer.name == "Arrow")
                    renderer.enabled = false;
            }       
        //    Debug.Log("Visible");
        }

        else if(Vector3.Distance(playerController.closest.transform.position, transform.position) > 50)
        {         
            foreach (MeshRenderer renderer in arrow)
            {
                if (renderer.name == "Arrow")
                    renderer.enabled = true;
            }           
            transform.position = player.transform.position;
            transform.LookAt(playerController.closest.transform.position);
        //    Debug.Log("Not visible");
        }
       
    }

    
}


