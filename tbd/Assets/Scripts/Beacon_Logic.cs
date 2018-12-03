﻿using System.Collections;
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
		Debug.Log("Hellow yes");
		if(col.gameObject.tag == "Player")
		{
			Debug.Log("Player Touched Beacon. Beacon Now Deactivated.");
			Beam.SetActive(false);
			GetComponent<Collider>().enabled = false;
			BeaconCounter.SendMessage("OnBeaconCollected");
			//Send Beacon Touched event
		}
	}
}
