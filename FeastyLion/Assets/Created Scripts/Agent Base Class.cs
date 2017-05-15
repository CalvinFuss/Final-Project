using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class Boid : MonoBehaviour
{


    float timerIncrement = 1;
    float timerTarget = 1;
    float distanceWanted, numberOfAnimals, healthMain, startingHealth, damageMain, panicSpeed, wanderSpeed, speed, speedMain, alertLevelRateMain, score, playerHealth;

    int frameNumer, numberOfAnimalsPresent;

    bool checkWanderSpeed = false;
    bool takeDamage, wanderIsTrue, sprint;

    public Vector3 destination, tempFleeDest;
    Vector3 fleeDestination, wanderDestination, previousDest, averageCenter, centre;

    Image temphealthBarImage, panicImg, staminaImg, alertLevelImg;

    public List<GameObject> listOfBoids = new List<GameObject>();
    GameObject[] Waypoints;
    GameObject animalObjectForRespawn;


    PlayerController playerController;
    Connectors connector;
    public AnimalVariables animalVariables;
       
    NavMeshAgent agent;

    LayerMask mask;
    // Start Function
    public virtual void boidBehaviour(GameObject player, float speedAnimal, GameObject animal, int numberOfBoids, GameObject startGameObject, string tag, float health)//  Virtual void used for inheritance. Enemy type, number of enemys to be instantiated, 
                                                                                                                                                    //...initial game object to move to, tag used for waypoints
    {   // Vector 3
        averageCenter = new Vector3(0, 11, 0); // reset value
        centre = new Vector3(0, 11, 0);
        wanderSpeed = 0.05f;
        previousDest = new Vector3(0, 0, 0);
        wanderDestination = startGameObject.transform.position; // sets inital destination 
        tempFleeDest = startGameObject.transform.position; // Used to set the current flee destination

        // GameObject
        animalObjectForRespawn = animal; // Variables used for respawning objects
        Waypoints = GameObject.FindGameObjectsWithTag(tag); // Gets waypoint game objects with specific tag and stores them within an array      

        // Integer
        numberOfAnimalsPresent = numberOfBoids; // Stores global variable of number of animals present

        // Float
        startingHealth = health; // Stores 'health' value within a global variable
        speedMain = speedAnimal; // Stores 'speedAnimal' within global variable
        speed = speedMain; // Used for normal speed of movement
        panicSpeed = speedMain * 3; // Used for sprinting
        numberOfAnimals = numberOfBoids;// used for average position 
        
        // Classes
        playerController = player.GetComponent<PlayerController>();

        // Mask
        mask = ~(1 << LayerMask.NameToLayer("Ignore")); // Store objects on ignore layer

        for (int i = 0; i < numberOfBoids; i++) // used to create agents
        {
            GameObject clone;
            // Instantiates enemy game object in random position 
            clone = Instantiate(animal, new Vector3(Random.Range(startGameObject.transform.position.x, startGameObject.transform.position.x + 30), 11, Random.Range(startGameObject.transform.position.z, startGameObject.transform.position.z + 30)), Quaternion.identity);
            listOfBoids.Add(clone); // Adds instantiated enemy to list
        }                                                                     
    }

    // Update function
    //Flocking Algorithm 
    public virtual void boidMove(GameObject player, GameObject startGameObject, GameObject blood, GameObject bloodSplat, GameObject combatText, GameObject healthBar, float damage, float alertLevelRate, float staminaLevelRate, float panicLevelRate, float distanceVisible, float walk, float run)// agent speed, player, Gameobject to move to when reverting to wander, blood particle, blood png
    {
       // Respawn of agents once count reaches 0
       if(listOfBoids.Count == 0)// If the number of a certain type of animal is 0
        {
            for (int i = 0; i < numberOfAnimalsPresent; i++) // Respawns animals if number of animals present is 0;
            {
                GameObject clone;
                clone = Instantiate(animalObjectForRespawn, new Vector3(Random.Range(50, 80), 11, Random.Range(50, 80)), Quaternion.identity); //Instantiates a new set of agents
                listOfBoids.Add(clone); // Adds these agents to the list
                playerController.enemies.Add(clone); // Adds respawned agents to the enemy list within the player controller class
            }      
        }


        playerController.tempPlayerHealth.fillAmount -= 0.005f * Time.deltaTime; // Player health steadily decreases
        frameNumer++;
        timerIncrement += Time.deltaTime;
        distanceWanted = 5;// Distance wanted between gameobjects

        wanderSpeedController();
       
        foreach (GameObject boids in listOfBoids.ToArray()) // 'To array' - enables change in list size while the loop is running
        {
            animalVariables = boids.GetComponent<AnimalVariables>(); // Gets the 'AnimalVariables' script component attached to each agent
            agent = boids.GetComponent<NavMeshAgent>(); // Gets navmesh agent component 
            float fleeProximity = Vector3.Distance(boids.transform.position, player.transform.position); // Calculates distance between agents and player

            animalStateParameters(fleeProximity, boids, player, alertLevelRate, staminaLevelRate, panicLevelRate, distanceVisible);

            // Flocking Rule # 1 - Alignment  
            agent.SetDestination(destination);// Transforms the flock of agents to a walkable position on the navmesh
            agentSpeedController(boids); // Controls the speed of agents

            boids.transform.LookAt(destination);// Rotate agent to look at the player
            boids.transform.Rotate(0, 180, 0); // Correct the rotation 
            averageCenter.y = 11;

            //Flocking Rule # 2 - Cohesion 
            if (wanderSpeed >= 0.5)
            {
                boids.transform.position = Vector3.MoveTowards(boids.transform.position, centre, Time.deltaTime * speed /10); // Transforms game objects towards centrepoint of flock
            }


            //Flocking Rule # 3 - Seperation (This was replaced by a more efficient process using Unity's OnCollisionEnter feature in the 'AnimalVariables' script)

            /*
                        foreach (GameObject relativeBoid in listOfBoids.ToArray())// compares agents to each other
                        {
                            float distance = Vector3.Distance(boids.transform.position, relativeBoid.transform.position); // Gets distances between an agent and all its neighbours
                    
                                if (distance < distanceWanted) // If the distance between agents is less than the distance wanted
                                {
                                    Vector3 difference = relativeBoid.transform.position - boids.transform.position; // Gets the difference in position 
                                    difference.y = 11; // sets y axis to 0 as we do not want to adress that axis

                                    difference = difference.normalized; // Normalise difference

                                }
                            }
                       
            */

            //Flocking Rule # 3 - Seperation - Continued - Currently references to calculated varriable form the 'animalVariables' class
            if (wanderSpeed >= 0.5 && animalVariables.addForceActive == true)
            {
                boids.GetComponent<Rigidbody>().AddForce(animalVariables.localVelocity*5, ForceMode.Force);// add force in the opposite direction to where the agents are moving towards. 
            }


            averageCenter += boids.transform.position; // Stores positions of agents            
            StartCoroutine(DestroyPrey(boids, player, blood, bloodSplat, combatText, healthBar, damage)); // Function - Handles Destory of agents
                                 
            if (timerIncrement > timerTarget) // Prevents multiple hits of damage being dealt to the agent by the player
            {
                takeDamage = true;
            }
            if (timerIncrement < timerTarget)
            {
                takeDamage = false;
            }
                        
            animatingAnimals(boids, walk, run); // Function - animation controller 
        } 


        centre = new Vector3(0, 0, 0); 
        averageCenter = averageCenter / numberOfAnimals;// calculates average centre of flock agent
        centre = averageCenter;

      // End of Flocking Alogrithm

    } // End of Update Function




    // Functions

    // Controls speed of agents
    void agentSpeedController(GameObject boids)
    { // Attacking is only set to true if agent is a humanoid
        if (animalVariables.attack == true) // if a humanoid agent is attacking the player
        {
            agent.speed = 0;
            if (Time.time > animalVariables.nextAttackTime) // References to variable in AnimalVariables class - Controls attack speed of agents
            {
                StartCoroutine(attackSpeed(boids));
                animalVariables.nextAttackTime = Time.time + animalVariables.timeBetweenAttacks; // Ensures that enemy attacks are only possible once certain time has passed
            }
        }
        // Sets speed depending on various states
        else
        {

            if (sprint == true && wanderIsTrue == false) // If sprinting is true
            {
                agent.speed = panicSpeed; // Agent speed is = panic speed
                animalVariables.animationSpeed = panicSpeed; // Sets run animation to true
            }


            if (sprint == false && wanderIsTrue == false) // If both sprinting and wandering is not true
            {
                agent.speed = speed; // Agent speed is = speed variable
                animalVariables.animationSpeed = speed;
                //Debug.Log(speed);
            }

            if (wanderIsTrue == true)// If wander is true 
            {
                agent.speed = wanderSpeed; // agent speed is = wanderspeed
                animalVariables.animationSpeed = wanderSpeed;
            }
        }
    }

    
   void wanderSpeedController()
    {
        // Controls the wander speed on agents
        if (wanderSpeed <= speed && checkWanderSpeed == false) // Moves wander speed up to normal speed variable
        {
            wanderSpeed += 5f * Time.deltaTime; // Speed increases if above 0 and below the normal speed
        }

        if (wanderSpeed >= speed) // Idle speed. Increases and decreases animal speed
        {
            checkWanderSpeed = true;

        }

        if (wanderSpeed <= 0.5)
        {

            StartCoroutine(wanderSpeedCalcluator());// if wanderSpeed is less than 0.5, a coroutene sets speed to 0 for 3 seconds


        }

        if (checkWanderSpeed == true)
        {
            wanderSpeed -= 5f * Time.deltaTime; // Wander speed decreases
            if (wanderSpeed < 0)
            {
                wanderSpeed += 5 * Time.deltaTime;
            }
        }


    }
    //Behavioural State - Agent Wander
    void wander(GameObject boids) // When called, sets the waypoint stored within the script component of the current waypoint as the destination
    {
        foreach (GameObject waypoint in Waypoints) // Wander script
        {

            if (waypoint.transform.position == wanderDestination)
            { // if the destination of waypoint is equal 

                connector = waypoint.GetComponent<Connectors>();  // Gets the script component connected to current waypoint. 
                                                                  // This script contains connecting waypoints
                wanderDestination = connector.connectors[Random.Range(0, connector.connectors.Count - 1)].transform.position; // Chooses next waypoint from current waypoint script

            }
        }
    }

    //Behavioural State - Agent Fleeing
   public virtual void fleeForward(GameObject player, GameObject boids) // Flee function
    {
        int multiplyBy = 100;
        boids.transform.rotation = Quaternion.LookRotation(boids.transform.position - player.transform.position);// Turns away from player position

        Vector3 runTo = boids.transform.position + player.transform.forward * multiplyBy; // sets a position 100 units in the direction to where the player is facing

        NavMeshHit hit;
        NavMesh.SamplePosition(runTo, out hit, 200, 1 << NavMesh.GetAreaFromName("Walkable")); // choose a walkable position within a certain distance of the agents

        tempFleeDest = hit.position;

        if (Vector3.Distance(tempFleeDest, previousDest) < 30) // if previous destination is less than 30 from current destiona
        {
            // Recalculate destination by 90 degrees - This prevents agents being unable to move at map bounds
            int multiplyByyy = 100;
            boids.transform.rotation = Quaternion.LookRotation(boids.transform.position - player.transform.position);
            Vector3 runToo = boids.transform.position + player.transform.right * multiplyByyy; // sets a position 100 units, 90 degrees to the right from where the player is facing

            NavMeshHit hitt;
            NavMesh.SamplePosition(runToo, out hitt, 200, 1 << NavMesh.GetAreaFromName("Walkable")); // choose a walkable position within a certain distance of the agents

            tempFleeDest = hitt.position;
        }
        previousDest = tempFleeDest;     
    }




  


    IEnumerator DestroyPrey(GameObject boid, GameObject player, GameObject bloodParticles, GameObject bloodSplat, GameObject combatText, GameObject healthBar, float damage)
    {
       
        healthbar(player, boid, healthBar, damage, combatText);



        if (healthMain <= 0)// If health of agent is below or equal to 0
        {            
            bloodParticles.transform.Rotate(player.transform.forward);// Rotates particle effect
            Instantiate(bloodParticles, new Vector3(boid.transform.position.x, boid.transform.position.y+5, boid.transform.position.z), Quaternion.identity);// Instantiate blood particle effect

           
            CBT("DESTROYED", boid, combatText); // Display text 
            
            Instantiate(bloodSplat, new Vector3(boid.transform.position.x, 10.01f, boid.transform.position.z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));// Instantiate bloodsplat particle effect

            foreach (Renderer r in boid.GetComponentsInChildren<Renderer>())// Do not render agent
                r.enabled = false;

            listOfBoids.Remove(boid); // Removes agent from list
            
            yield return new WaitForSeconds(2); // Wait 2 seconds...
            Destroy(boid); // ... then destroy agent

        }
    }

    public void CBT(string damage, GameObject boid, GameObject combatText) // Text UI when player attacks enemy
    {
        GameObject temp = Instantiate(combatText) as GameObject; // Instantiated the text prefab from the specific enemy type
        RectTransform tempRect = temp.GetComponent<RectTransform>(); // Gets the 'RectTransform' Component of the text object - responsible for the transform values of the text prefab
        temp.transform.SetParent(boid.transform.FindChild("Canvas")); // Assigns the Canvas attached to the enemy prefab and sets text prefab as the child of the canvas
        tempRect.transform.localPosition = combatText.transform.localPosition; // Sets the prefabs position relative to the canvas position.
        tempRect.transform.localScale = combatText.transform.localScale;//""
        tempRect.transform.localRotation = combatText.transform.localRotation;//""

        temp.GetComponent<Text>().text = damage; // Gets text value and assigns a string specified when the function is called
        temp.GetComponent<Animator>().SetTrigger("Hit"); // Initated the animation with the specific trigger/
        Destroy(temp.gameObject, 2); // Destroys text gameobejct after two seconds. 
        temp.transform.Rotate(0, 180, 0);
    }

    public void healthbar(GameObject player, GameObject boid, GameObject healthBar, float damage, GameObject combatText)
    {

        Image[] healthImage = boid.GetComponentsInChildren<Image>();
        foreach (Image image in healthImage)
        {
            if (image.name == "Health")
                temphealthBarImage = image;
        }

        damageMain = damage; // The amount of damage done to each agent
        if (playerController.lunge == true)
        {
            if (takeDamage == true && Vector3.Distance(player.transform.position, boid.transform.position) <15)
            {

               
               CBT(damageMain.ToString(), boid, combatText); // Instantiate combat text UI above agent
                
                playerController.tempPlayerHealth.fillAmount += (damageMain / 100);  // adds value to player health when enemy is attacked
                playerController.currentScore += damageMain; // Adds this to the score variable within the PLayerController script
                //Calculates the amount of health to be removed from agent when player attacks agent
                float currentHealth = startingHealth * temphealthBarImage.fillAmount; // Get's health UI image value attached to each agent
                float subtractHealthBy = damage / currentHealth;
                temphealthBarImage.fillAmount -= subtractHealthBy;

                timerIncrement = 0;

            }

          

        }


        healthMain = temphealthBarImage.GetComponent<Image>().fillAmount; //health image bar attached to each agent

        if (healthMain <= 0) 
        {
            foreach (Image image in healthImage)
            {
                if (image.name == "HealthBG")
                {
                    Destroy(image);
                }

            }
        }




    }


    // Finite State Machine - Advanced Behavioural States
    public virtual void animalStateParameters(float proximity, GameObject boids, GameObject player, float alertLevelRate, float staminaLevelRate, float panicLevelRate, float distanceVisible)
    {
        Image[] images = boids.GetComponentsInChildren<Image>();

       // Gets Images attached to each agent - Each representing an emotional state
       // If the player is within a certain porximity to agents, the alert state level rises
       // if the alert state level is full, the panic state rises. 
       // Within the panic state, agents increase in speed and the stamina level depletes
       // If the stamina level reaches 0, the agent speed reverts back to normal
        foreach (Image image in images)
        {
            if (image.name == "Panic") // Panic State
                panicImg = image;


            if (image.name == "Stamina") // Stamina
                staminaImg = image;

            if (image.name == "AlertLevel") // Alert State
                alertLevelImg = image;
        }



        RaycastHit hit = new RaycastHit(); // Initialisation of raycast        
        bool fleeing = false;

        Vector3 heading = boids.transform.position - player.transform.position; 
        float dot = Vector3.Dot(heading, player.transform.forward); // Calculates the direction the player is facing

        Vector3 distance;


        if (staminaImg.fillAmount <= 0) // Stops agent sprinting if the stamina has depleted
        {
            sprint = false;
        }

        if (alertLevelRateMain > 0.8f) // Caps the rate the alert level rises each frame
        {
            alertLevelRateMain = 0.8f;
        }

        else
            alertLevelRateMain = alertLevelRate / (proximity); // Alert level rises at variable speed depending on the proximity of player 

        distance = boids.transform.position - player.transform.position;

       



        //Racast - Points from each agent towards player

        if (Physics.Raycast(boids.transform.position, -distance, out hit, distanceVisible, mask)) // if the raycast hits the player gameObject. 
                                                                                                  //Ignores objects on layer stored within 'mask' variable

        {            
            if (hit.collider.gameObject.tag == "Player") // if agent ray hits player
            {              
                alertLevelImg.fillAmount += alertLevelRateMain * Time.deltaTime; // Fills the alert level UI attached to agent by 'alartLevelMain' variable
            }


            if (hit.collider.gameObject.tag == "Player" && alertLevelImg.fillAmount >= 1 && staminaImg.fillAmount > 0) // if agent ray hits player and alert level is full
            {// Agent sprints
                sprint = true; 
                // Panic level
                panicSpeed = speed * 2 + (panicLevelRate * 10); // Adjust speed based on panic level - The higher the panic level the higher the speed
                panicImg.fillAmount += panicLevelRate * Time.deltaTime; // panicLevelRate value defined in derived class
                staminaImg.fillAmount -= staminaLevelRate * Time.deltaTime; // Reduces agent stamina when sprinting
            }
            else
            {
                sprint = false;
            }      
        }


//        Debug.DrawRay(boids.transform.position, -distance, Color.red);  -- Draws Raycast
//        Debug.DrawLine(boids.transform.position, destination, Color.blue); -- Draws line towards current destination of agent
        if (Physics.Raycast(boids.transform.position, -distance, out hit, distanceVisible) == false) // If ray is not hitting player
        {
            staminaImg.fillAmount += staminaLevelRate * Time.deltaTime; // Replenishes stamina
            panicImg.fillAmount -= panicLevelRate * Time.deltaTime; // Reduces panic level rate

            if (panicImg.fillAmount <= 0)
            {
                alertLevelImg.fillAmount -= (panicLevelRate / 100); // If panic level rate is 0, reduces alert level rate
            }

            // Back to agent wander state
            fleeing = false;
            wanderIsTrue = true;
            destination = wanderDestination;

            if (animalVariables.state == AnimalVariables.State.Run)
            {
                animalVariables.state = AnimalVariables.State.CoolDown;
            }

        }

        if (Vector3.Distance(boids.transform.position, wanderDestination) < 5)    // If distance betweeen waypoint and agent is below 2 units                                       
        {
            wander(boids); // move to next waypoint
            destination = wanderDestination;        
        }
        

       
        if (Vector3.Distance(boids.transform.position, player.transform.position) < 120 && animalVariables.state == AnimalVariables.State.Wander|| animalVariables.state == AnimalVariables.State.Run) // If the player come within a distance of 80 units
        {           
            animalVariables.state = AnimalVariables.State.Run;
            
            fleeing = true;
            wanderIsTrue = false;

            if(dot < 0) // If agents are behind the player
            {
                animalVariables.fleeBack = true;
                int multiplyBy = 100;
                boids.transform.rotation = Quaternion.LookRotation(boids.transform.position - player.transform.position);// Turns away from player position

                Vector3 runTooo = boids.transform.position - player.transform.forward * multiplyBy; // sets a position 100 units in the oposite direction to where the player is facing

                NavMeshHit hittt;
                NavMesh.SamplePosition(runTooo, out hittt, 200, 1 << NavMesh.GetAreaFromName("Walkable")); // choose a walkable position within a certain distance of the agents

                destination = hittt.position;
            }

            else // If agents are not behind player
            {
                destination = tempFleeDest;

                if(animalVariables.fleeBack == true)
                {
                    fleeForward(player, boids);
                    animalVariables.fleeBack = false;
                }
            }
        }


        if (animalVariables.state == AnimalVariables.State.CoolDown) // When the player is out of range after agents have fleed..
        {                                                            // ..agents move at a set speed until 5 seconds have passed                                                                     // .. after 5 seconds, agents revert to the wander speed
            animalVariables.timer += 1 * Time.deltaTime;
        }

        if (animalVariables.timer > 5) 
        {
            animalVariables.state = AnimalVariables.State.Wander;
            animalVariables.timer = 0;
        }


        if (Vector3.Distance(boids.transform.position, tempFleeDest) < 10 && fleeing == true) // Recalculates flee destination once it has been reached
        {
            fleeForward(player, boids);           
        }
    }


    // Controls agent animations within the agent's animation controller
    void animatingAnimals(GameObject boid, float walk, float run)
    {
        Animator anim;
        anim = boid.GetComponent<Animator>();

        anim.SetFloat("run", animalVariables.animationSpeed);

    }

    // Replenishes agent stamina after 2 seconds when called
    IEnumerator refreshStmaina()
    {
        if (staminaImg.fillAmount <= 0)
        {
            yield return new WaitForSeconds(2);
            staminaImg.fillAmount = 1;
        }
    }

    // Controls the agents behavioural state speed
    IEnumerator wanderSpeedCalcluator()
    {
        yield return new WaitForSeconds(3);
        checkWanderSpeed = false;
    }

    IEnumerator attackSpeed(GameObject boid)
    {
        animalVariables.humanGruntAudio.Play();
        animalVariables.humanGruntAudio.volume = 0.3f;
        playerController.tempPlayerHealth.fillAmount -= 0.05f;
        
        animalVariables.animationSpeed = -5;
        yield return null;
    }
}
