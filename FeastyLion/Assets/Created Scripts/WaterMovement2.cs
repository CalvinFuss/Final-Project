using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Animates the water within the scene in the opposing direction

public class WaterMovement2 : MonoBehaviour {
    float height = 0;
    bool move = false;
    float yPos;

    void Start()
    {
        yPos = transform.position.y;
        height = yPos;
    }

    // Moves water up and down on the Y coordinate 
    void Update()
    {


        if (height >= yPos + 1f)
        {
            move = true;
        }

        if (height <= yPos - 1f)
        {
            move = false;
        }


        if (move == true)
        {
            height -= 0.7f * Time.deltaTime;
        }

        if (move == false)
        {

            height += 0.7f * Time.deltaTime;
        }


        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
