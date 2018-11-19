using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

	public float waitTime;
	public bool useScaledTime;

	private float waitTimePassed = 0.0f;
	
	void Update () 
	{
		// Choose the appropriate dt.
		float dt;
		if(useScaledTime)
		{
			dt = Time.deltaTime;
		}
		else
		{
			dt = Time.unscaledDeltaTime;
		}

		// Destroy this game object after passing the wait time.
		waitTimePassed += dt;
		if(waitTimePassed >= waitTime)
		{
			Destroy(gameObject);
		}
	}
}
