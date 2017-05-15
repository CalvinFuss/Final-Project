using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles scenes
public class StateHandler : MonoBehaviour {

    public void StartGameBtn(string scene)
    {
        SceneManager.LoadScene(scene); // Loads Start Scene 

    }

    public void EndGameBtn(string scene)
    {
        SceneManager.LoadScene(scene); // Loads End Scene
    }



}
