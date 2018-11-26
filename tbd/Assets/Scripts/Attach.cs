using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour {

	public bool attachPosition;
	public GameObject attachedToObject;
	
	void LateUpdate () 
	{
		if (attachPosition)
		{
			this.transform.position = attachedToObject.transform.position;
		}
	}
}
