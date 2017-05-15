using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This is a derived class for the 'Antelope' agent
public class Antelope : Boid {

    
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


    float walkAnimationMultiplier;
    float runAnimationMultiplier;

    float alertLevelRate;
    float panicLevelRate;
    float staminaLevelRate;

    float distanceVisible;

  
    void Awake()
    {
        alertLevelRate = 10f;
        panicLevelRate = 0.2f;
        staminaLevelRate = 0.1f;
        distanceVisible = 120;
        health = 20;
        speedZebra = 20;
        boidBehaviour(player, speedZebra, animalZebra, 8, startGameObject, "AntelopeWaypoint", health); // Zebra, number of Zebras, Object it moves to in order to initialise 'wander state', tag to wander to 

    }

    void Start()
    {
        walkAnimationMultiplier = 2;
        runAnimationMultiplier = 3;
    }


    void Update()
    {
        damage = Random.Range(10, 20);
        boidMove(player, startGameObject, blood, bloodSplat, combatText, healthBar, damage, alertLevelRate, staminaLevelRate, panicLevelRate, distanceVisible, walkAnimationMultiplier, runAnimationMultiplier); // Move speed, Player game object, Object it moves to when switching to 'wander state', blood particle, blood png 
    }

 
}
