using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScreen : MonoBehaviour {

	public int button = 0; // 0 = x, 1 = a, 2 = b, 3 = y
	// Use this for initialization
	void Start () {
	}

	void OnMouseDown()
    {
        Debug.Log("Button " + button + " clicked on");
		Controlador.TouchedBtn = button;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
