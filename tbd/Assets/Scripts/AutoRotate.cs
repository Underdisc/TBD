using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

	[SerializeField] Vector3 RotationVector = new Vector3(0,0,1);
	[SerializeField] float RotationSpeed = 1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(RotationVector * RotationSpeed * Time.unscaledDeltaTime, Space.Self);
	}
}
