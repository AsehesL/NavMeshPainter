using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    Debug.Log(transform.localToWorldMatrix);
	    Debug.Log(transform.right);
	    Debug.Log(transform.up);
	    Debug.Log(transform.forward);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
