using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateHandlerMain : MonoBehaviour {
    PlayerController playerController;
    public GameObject player;
    // Use this for initialization
    void Start () {
        playerController = player.GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (playerController.tempPlayerHealth.fillAmount <= 0)
        {
            SceneManager.LoadScene("End");
        }
    }
}
