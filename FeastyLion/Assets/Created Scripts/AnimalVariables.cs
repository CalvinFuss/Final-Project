using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is attached to each agent. 
// It facilitates the storage of a unique set of variables attached to each agent
// These variables are called and manipulated within the 'Boid' base class
public class AnimalVariables : MonoBehaviour
{ 

    [HideInInspector]
    public float timer = 0, animationSpeed, timeBetweenAttacks, nextAttackTime, previousDest;

    [HideInInspector]
    public bool sprint = false, coolDownTime = false, attack = false, fleeBack, fleeForward, addForceActive, fleeing;  

    [HideInInspector]
    public enum State { Wander, Run, CoolDown};

    [HideInInspector]
    public State state;

    [HideInInspector]
    GameObject player;

    [HideInInspector]
    public Vector3 localVelocity;

    [HideInInspector]
    public AudioClip humanGruntClip;

    [HideInInspector]
    public AudioSource humanGruntAudio;

    public AudioSource AddAudio(AudioClip clip)
    {

        AudioSource newAudio = gameObject.AddComponent<AudioSource>();

        newAudio.clip = clip;


        return newAudio;
    }

    public void Awake()
    {
        timeBetweenAttacks = 1f; 
        humanGruntAudio = AddAudio(humanGruntClip);
    }



    // Use this for initialization
    void Start()
    {      
        state = State.Wander;
        coolDownTime = false;
        previousDest = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        addForceActive = false;

        if (transform.name == "Human(Clone)") // Addresses humanoid agents only
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 15)
            {
                attack = true;               
            }
            else
            {
                attack = false;
            }
        }
    }

    // Flocking rule # 3 - Seperation - An alternative method to that within the base class
    // When agents collide, the direction of movement is calculated
    // This direction of movement is to transform agents in the oposite direction in which they are moving
    /*
    void OnCollisionEnter(Collision collision)
    {        
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ignore")) // Checks collisions between objects on the 'ignore' layer
        {
           localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);// Calculates direction of agent
           addForceActive = true;            
        }               
    }
    */
}
        




