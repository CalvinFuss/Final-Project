using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StateHandler : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void StartGameBtn(string scene)
    {
        SceneManager.LoadScene(scene);

    }

    public void EndGameBtn(string scene)
    {
        SceneManager.LoadScene(scene);
    }



}
