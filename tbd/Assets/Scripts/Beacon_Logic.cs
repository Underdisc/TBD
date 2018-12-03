using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//brief: logic for beacon to trigger when touched by player
public class Beacon_Logic : MonoBehaviour {

	[SerializeField] GameObject Beam = null;
	[SerializeField] GameObject BeaconCounter = null;
	void Update()
	{
	}
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Player")
		{
			Beam.SetActive(false);
			GetComponent<Collider>().enabled = false;
			BeaconCounter.SendMessage("OnBeaconCollected");
			//Send Beacon Touched event
		}
	}
}
