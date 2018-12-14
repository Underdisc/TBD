using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SplashController : MonoBehaviour 
{

	public string nextScene;
	public GameObject[] splash;

	public float initTime;
	public float fadeTime;
	public float sustainTime;
	public float switchTime;
	private float timeElapsed;

	private int cSplash;

	private delegate void VoidVoidDelegate();
	private VoidVoidDelegate stage;

	void Start () 
	{
		if(splash.Length <= 0)
		{
			SceneManager.LoadScene(nextScene);
		}
		for(int i = 0; i < splash.Length; ++i)
		{
			SetAlpha(splash[cSplash], 0.0f);
		}
		cSplash = 0;
		stage = Init;
	}

	void SetAlpha(GameObject ui_object, float alpha)
	{
		Image image_comp = ui_object.GetComponent<Image>();
		Vector4 c_color = image_comp.color;
		c_color[3] = alpha;
		image_comp.color = c_color;
	}

	float GetPerc(float total_time, VoidVoidDelegate next_stage)
	{
		float perc;
		if(timeElapsed >= total_time)
		{
			perc = 1.0f;
			timeElapsed = 0.0f;
			stage = next_stage;
		}
		else
		{
			perc = timeElapsed / total_time;
		}
		return perc;
	}

	void Switch()
	{
		if(timeElapsed >= switchTime)
		{
			cSplash++;
			timeElapsed = 0.0f;
			stage = FadeIn;
		}
	}

	void FadeOut()
	{
		float perc = GetPerc(fadeTime, Switch);
		perc = ActionOperation.QuadOut(perc);
		SetAlpha(splash[cSplash], 1.0f - perc);
	}

	void Sustain()
	{
		if(timeElapsed >= sustainTime)
		{
			if((cSplash + 1) >= splash.Length)
			{
				SceneManager.LoadScene(nextScene);
			}
			timeElapsed = 0.0f;
			stage = FadeOut;
		}
	}

	void FadeIn()
	{
		float perc = GetPerc(fadeTime, Sustain);
		perc = ActionOperation.QuadIn(perc);
		SetAlpha(splash[cSplash], perc);
	}

	void Init()
	{
		if(timeElapsed >= initTime)
		{
			timeElapsed = 0.0f;
			stage = FadeIn;
		}
	}

	void UpdateSplash()
	{
		timeElapsed += Time.deltaTime;
		stage();
	}
	
	void Update () 
	{
		UpdateSplash();
	}
}
