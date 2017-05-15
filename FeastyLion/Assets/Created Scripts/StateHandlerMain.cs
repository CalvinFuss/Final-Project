using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateHandlerMain : MonoBehaviour {
    PlayerController playerController;
    public GameObject player;

    void Start () {
        playerController = player.GetComponent<PlayerController>(); // script
    }
	

	void Update () {
        // If the player's health bar reaches a value of 0 
        if (playerController.tempPlayerHealth.fillAmount <= 0) 
        {
            SceneManager.LoadScene("End"); // Load scene
        }
    }
}
