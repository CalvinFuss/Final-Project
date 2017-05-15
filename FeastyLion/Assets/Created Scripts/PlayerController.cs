    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{//Movement
    public float maxSpeed = 10; // Agent speed
    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;
    public float attackDistance, currentScore;
    float movementSpeed = 0;
    float acceleration = 45;
    float sprint, timeBetweenAttacks, nextAttackTime,minDistance, distanceToTarget, damage;

    int playAnimation;

    public bool lunge;
    public bool attacking = false;
    bool isCurrentlyMoving, isBoosting;

    Vector3 targetPosition, directionToTarget, moveVec;
    
    public enum State { Idle, Chasing, Attacking, Lunge}; // Used for animation of avatar
    State currentState;

    public GameObject enemyCheck, closest, player;
    public GameObject[] instantiateEnemies;
    GameObject cube;

    public List<GameObject> enemies;
               
    public Canvas CanvasUI;

    public Image tempPlayerHealth, arrow, bgImage;
    Image tempHealthBar, tempStaminaBar;
  
    public Text tempScoreValue;
    
    public Animator animations;

    public AudioClip humanBeingAttackedClip, animalBeingAttackedClip, LionRawrClip;

    [HideInInspector]
    public AudioSource humanBeingAttackedAudio, LionRawrAudio, animalBeingAttackedAudio;

    public AudioSource AddAudio(AudioClip clip)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        return newAudio;
    }


    void Start()
    {
        animations = transform.GetComponent<Animator>();
        instantiateEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Stores'Enemy' agents within array

        enemyCheck = null;
        timeBetweenAttacks = 1f; // Sets timeframe between successive attacks 
        attackDistance = 15f; // Distance from which attacks are initialised
        currentState = State.Idle;
        minDistance = 1000;

        // Initialises audio
        humanBeingAttackedAudio = AddAudio(humanBeingAttackedClip);
        animalBeingAttackedAudio = AddAudio(animalBeingAttackedClip);
        LionRawrAudio = AddAudio(LionRawrClip);
                          
        foreach (GameObject enemy in instantiateEnemies)
        {
            enemies.Add(enemy); // Adds enemies to list
        } 
            
        Stamina();
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);

            if (distance < minDistance)
            {

                closest = enemy;
                minDistance = distance;

            }
        }
    }


    void FixedUpdate()
    {
        tempScoreValue.text = currentScore.ToString(); // Score value - calculated by the boid base class           
                   
    }



    void Update()
    {
        AnimationStates();
        MobileInput();

        // Controls agent movement
        
        if (isCurrentlyMoving == true)  // If the agent is moving
        {
            movementSpeed += acceleration * Time.deltaTime; // Adds acceleration to agent 

            if (movementSpeed > maxSpeed)
            {
                movementSpeed = maxSpeed; // Accerleration stops when required speed is reached
            }
        }

        else
        {
            movementSpeed = 0;
        }
       
        AnimationStates();
        
        attackPosition();

       



        // Find nearest agent game object to player and stores this within the 'closest' variable
        minDistance = 1000; 
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);// Calculates distance each enemy

            if (distance < minDistance)
            {
                closest = enemy; // stores closest enemy in variable
                minDistance = distance; // Updates distance variable to search for closer enemies             
            }
        }

        animations.SetInteger("State", playAnimation); // Controls animations within the animation controller attached to game object     
    }

    // Player lunge attack function
    void attackPosition()
    {
        if (Time.time > nextAttackTime)// A time instance controls the number of attacks
        {
            foreach (GameObject enemy in enemies.ToArray())
            {

                directionToTarget = (enemy.transform.position - transform.position).normalized;  // Calculates direction to agent
                distanceToTarget = (enemy.transform.position - transform.position).sqrMagnitude; // Calculates distance to agent - more efficient than Vector3.Distance()
                // If player is facing target
                if (Vector3.Dot(directionToTarget, transform.forward) > 0.9 && Vector3.Dot(directionToTarget, transform.forward) < 1) 
                {
                    // If distance between target is less than attackDistance variable
                    if (distanceToTarget < Mathf.Pow(attackDistance, 2))
                    {
                        lunge = true; // Used within base class to deal damage to specific boid
                        
                        StartCoroutine(Attack(enemy));// Initialises player attack coroutene
                        nextAttackTime = Time.time + timeBetweenAttacks; // Resets variable controlling time between attacks

                        // plays audio for each respective enemy type
                        LionRawrAudio.Play();                       
                        if (enemy.transform.name == "Human(Clone)")
                        {
                            humanBeingAttackedAudio.Play();
                        }

                        if (enemy.transform.name == "Antelope(Clone)")
                        {
                            animalBeingAttackedAudio.Play();
                        }

                        if (enemy.transform.name == "Zebra(Clone)")
                        {
                            animalBeingAttackedAudio.Play();
                        }
                        
                    }                  
                }
                healthBar(enemy);
            }
        }
    }

    //Player lunge coroutene 
    IEnumerator Attack(GameObject enemy)
    {
        
        // currentState = State.Attacking;
        Vector3 origionalPosition = transform.position; // Stores the transform position (player game object)
        Vector3 attackPosition = enemy.transform.position; // Stores agent position
        float attackSpeed = 2;
       
        float percent = 0;
        while (percent <= 1)
        {
            percent += Time.deltaTime * attackSpeed;// increments percent variable value 
            float interpolation = (-percent * percent + percent) * 3; // Parabolar - increments value from [0 - 1] then decrements value from [1 - 0] 

            // Vector3.Lerp Interpolates game object from start position [a] to target position [b] by interpolant [t] - variable 'interpolation' 
            transform.position = Vector3.Lerp(origionalPosition, new Vector3(attackPosition.x, attackPosition.y+0.7f, attackPosition.z), interpolation); 

            yield return null;

        }
        lunge = false; // once lunge is complete, lunge = false;
        
    }

    // Mobile Control
    public void MobileInput()
    {
        moveVec = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, CrossPlatformInputManager.GetAxis("Vertical")); // Gets respective axes of joystick
     
        bool isBoosting = CrossPlatformInputManager.GetButton("Boost"); // Boost button within 'MobileSingleJoystickControl' canvas

          // Defines player sprint speed value
            if (moveVec.x != 0 && moveVec.z != 0) // If joystick is being moved
            {
                isCurrentlyMoving = true;
                Quaternion eulerRot = Quaternion.LookRotation(moveVec); // Rotates object towards direction the joystick is pointing
                // Controls roation speed of game object towards the target rotation defined by 'eulerRot'
                transform.rotation = Quaternion.Slerp(transform.rotation, eulerRot, Time.deltaTime * 5f); 
                                                                                                          
            if (tempStaminaBar.fillAmount < 0.05) // If the stamina bar value is below 0.05
                {
                    sprint = 1; // Stop sprinting
                    if (currentState != State.Attacking)
                    {
                        currentState = State.Chasing; 
                    }
                }

                else
                {
                    sprint = 1.5f;
                }

                currentState = State.Chasing; // If the joystick is moving
            }

        if(isBoosting == true && moveVec.x != 0 && moveVec.z != 0 && tempStaminaBar.fillAmount > 0.01) // If joystick is being moved and boost button is pressed
        {           
            tempStaminaBar.fillAmount -= 0.005f; // Decrease players stamina level
            currentState = State.Attacking;         
        }

        if(isBoosting == false && moveVec.x != 0 && moveVec.z != 0) // If joystick is being moved and boost button is not pressed
        {
            tempStaminaBar.fillAmount += 0.0008f; // Replenish stamina bar          
            currentState = State.Chasing;           
        }

     
        if(moveVec.x == 0 && moveVec.z == 0) // If player is not moving 
        {
            isCurrentlyMoving = false;
            currentState = State.Idle;
        }

        
        // Transforms game object on the respective axes by defined movement speed. 
        // If the boost button is active, then the movement speed is multiplied by the sprint value.
        transform.position += (moveVec *(movementSpeed * Time.deltaTime)*(isBoosting ? sprint : 1)); 
        transform.position = new Vector3(transform.position.x, 10.3f, transform.position.z); // Don't translate game object on 'Y' axis. 

    }

    //booleans control the animations of the game object
void AnimationStates()
    {   // Controls animations of game object within the scene
        // Each state sets a different 'playAnimation' value
        // The 'playAnimation' value controls the value of a variable within the game object's animation controller
        // Each animation is set to play when a specific 'playAnimation' value is true
        if (currentState == State.Idle) 
        {
            playAnimation = 0;
        }

        if (currentState == State.Chasing)
        {
            playAnimation = 1;
        }

        if (currentState == State.Attacking)
        {
            playAnimation = 2;
        }
        if (lunge == true)
        {
            playAnimation = 3;
        }       
    }
   
    // These functions assign the UI bar values to variables
    void healthBar(GameObject enemy) // Gets healthbar of each agent
    {
        Image[] healthImage = enemy.GetComponentsInChildren<Image>();
        foreach (Image image in healthImage)
        {
            if (image.name == "Health")
                tempHealthBar = image;
        }

        if(tempHealthBar.fillAmount <= 0) // Destroys agent if its healthbar has reached 0
        {
            enemies.Remove(enemy);
            Destroy(enemy);
        }
    }


    void Stamina() // Player stamina UI
    {
        Image[] staminaImage = CanvasUI.GetComponentsInChildren<Image>(); // Stores children of prefab in array
        foreach (Image image in staminaImage) // Searches for gameobject with specific name from array
        {
            if (image.name == "Stamina")
                tempStaminaBar = image;
        }
    }

    void Score() // Player Score UI
    {
        Text[] scoreText = CanvasUI.GetComponentsInChildren<Text>();
        foreach (Text text in scoreText)
        {
            if (text.name == "Score")
                tempScoreValue = text;
        }
    }

   
}


