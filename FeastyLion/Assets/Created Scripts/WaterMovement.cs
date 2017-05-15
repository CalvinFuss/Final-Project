using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour {
    float height = 0;
    bool move = true;
    float yPos;
	// Use this for initialization
	void Start () {
        yPos = transform.position.y;
        height = yPos;
	}
	
	// Update is called once per frame
	void Update () {


        if (height >= yPos + 1f)
        {
            move = true;
        }

        if(height <= yPos - 1f)
        {
            move = false;
        }


        if (move == true)
        {
            height -= 0.7f * Time.deltaTime;
        }

        if(move== false)
        {

            height += 0.7f * Time.deltaTime;
        }


            transform.position = new Vector3(transform.position.x, height, transform.position.z);
	}
}
