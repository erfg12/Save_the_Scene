using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchSkipToIntro : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
	{
		Debug.Log("skipping to intro");
		//SceneManager.LoadScene("intro");
		Time.timeScale = 1;
	}
}
