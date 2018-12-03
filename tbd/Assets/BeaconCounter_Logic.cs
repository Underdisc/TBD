using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BeaconCounter_Logic : MonoBehaviour {

	[SerializeField] List<Image> Counters = new List<Image>();
	int BeaconsCollected = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnBeaconCollected()
	{
		Counters[BeaconsCollected].color = new Color(0,0,0,0.35f);
		BeaconsCollected +=1;
	}
}
