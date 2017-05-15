using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanChase : Boid
{
    public float speedZebra;
    public GameObject animalZebra;
    public GameObject startGameObject;
    public GameObject player;

    public GameObject combatText;
    public GameObject blood;
    public GameObject bloodSplat;
    public float health;
    float damage;
    public GameObject healthBar;
    public Animator anim;

    float walkAnimationMultiplier;
    float runAnimationMultiplier;

    float alertLevelRate;
    float panicLevelRate;
    float staminaLevelRate;

    float distanceVisible;


    public float attackDistance;
    float timeBetweenAttacks;
    float nextAttackTime;
    Vector3 directionToTarget;
    float distanceToTarget;

    // Use this for initialization
    void Awake()
    {
        alertLevelRate = 10f;
        panicLevelRate = 0.2f;
        staminaLevelRate = 0.1f;
        distanceVisible = 80;
        health = 20;
        speedZebra = 20;
        boidBehaviour(player, speedZebra, animalZebra, 3, startGameObject, "HumanWaypoint", health); // Zebra, number of Zebras, Object it moves to in order to initialise 'wander state', tag to wander to 

    }

    void Start()
    {
        anim = animalZebra.GetComponent<Animator>();
        walkAnimationMultiplier = 2;
        runAnimationMultiplier = 3;


    }



    // Update is called once per frame
    void Update()
    {

        damage = Random.Range(10, 15);

        boidMove(player, startGameObject, blood, bloodSplat, combatText, healthBar, damage, alertLevelRate, staminaLevelRate, panicLevelRate, distanceVisible, walkAnimationMultiplier, runAnimationMultiplier); // Move speed, Player game object, Object it moves to when switching to 'wander state', blood particle, blood png

        // 
        
    }

    
    public override void fleeForward(GameObject player, GameObject boids)
    {
        base.fleeForward(player, boids);
        tempFleeDest = player.transform.position;
    }
    
    
        

    public override void animalStateParameters(float proximity, GameObject boids, GameObject player, float alertLevelRate, float staminaLevelRate, float panicLevelRate, float distanceVisible)
    {
        base.animalStateParameters(proximity, boids, player, alertLevelRate, staminaLevelRate, panicLevelRate, distanceVisible);

        if (Vector3.Distance(boids.transform.position, player.transform.position) < 120 && animalVariables.state == AnimalVariables.State.Wander || animalVariables.state == AnimalVariables.State.Run) // If the player come within a distance of 80 units
        {
            destination = player.transform.position;
        }
    }




   

}
