using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerLogic : MonoBehaviour {

	Text TimerText = null;	
	bool TimerActive = false;
	float hours = 0.00f;
	float minutes = 0.00f;
	float seconds = 0.00f;
	float milliseconds = 0.0f;
	[SerializeField] Text MilliText;
	// Use this for initialization
	void Start () {
		TimerText = GetComponent<Text>();
		
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.T))
		{
			TimerActive = true;
		}
		if(!TimerActive)
			return;
		milliseconds += Time.deltaTime * 1000;
		
		if(milliseconds >= 1000)
		{
			seconds += 1;
			milliseconds = 0;

			if(seconds >= 60)
			{
				minutes += 1;
				seconds = 0;
			}

			if(minutes >= 60)
			{
				hours += 1;
				minutes = 0;
			}
		}
		
		string hr = hours.ToString();
		string min = minutes.ToString();
		string sec = seconds.ToString();
		float millisecondsRounded = Mathf.Round(milliseconds);
		string mil = millisecondsRounded.ToString();
		if(millisecondsRounded < 10){mil = string.Concat("000",millisecondsRounded);}
		else if(millisecondsRounded < 100){mil = string.Concat("00",millisecondsRounded);}

		if(seconds < 10){sec = string.Concat("0", seconds);}
		if(minutes < 10){min = string.Concat("0", minutes);}
		if(hours < 10){hr = string.Concat("0", hours);}

		//TimerText.text = string.Concat(hr, ":", min, ".", sec, ".", mil, " ms");
		TimerText.text = string.Concat(hr, ":", min, ".", sec);
		MilliText.text = string.Concat(mil, " ms");
	}
	void StartTimer()
	{
		TimerActive = true;

	}
	void StopTimer()
	{
		TimerActive = false;
	}
	void UpdateTimer()
	{
		string hr = hours.ToString();
		string min = minutes.ToString();
		string sec = seconds.ToString();
		string mil = milliseconds.ToString();
		if(milliseconds < 10){mil = string.Concat("0",milliseconds);} 
		if(seconds < 10){sec = string.Concat("0", seconds);}
		if(minutes < 10){min = string.Concat("0", minutes);}
		if(hours < 10){hr = string.Concat("0", hours);}

		TimerText.text = string.Concat(hr, ":", min, ".", sec, ".", mil, " ms");
	}
}
