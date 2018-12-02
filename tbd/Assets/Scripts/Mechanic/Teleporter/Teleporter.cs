using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour 
{
	private bool reappearing = false;
	public float reappearTime;
	private float reappearTimeElapsed;

	private bool disappearing = false;
	private float disappearTime;
	private float disappearTimeElapsed;

	private Collider collider;
	private MeshRenderer renderer;

	public void OnBash()
	{
		reappearTimeElapsed = 0.0f;
		reappearing = true;
	}

	public void OnTeleport(float teleport_time)
	{
		disappearTime = teleport_time / 2.0f;
		disappearTimeElapsed = 0.0f;
		disappearing = true;
	}

	void Reappear()
	{
		reappearTimeElapsed += Time.unscaledDeltaTime;
		if(reappearTimeElapsed >= reappearTime)
		{
			collider.enabled = true;
			renderer.enabled = true;
			reappearing = false;
		}
	}

	void Disappear()
	{
		disappearTimeElapsed += Time.unscaledDeltaTime;
		if(disappearTimeElapsed >= disappearTime)
		{
			collider.enabled = false;
			renderer.enabled = false;
			disappearing = false;
		}
	}

	void Update () 
	{
		if(disappearing)
		{
			Disappear();
		}
		else if(reappearing)
		{
			Reappear();
		}
	}

	void Start()
	{
		collider = gameObject.GetComponent<Collider>();
		renderer = gameObject.GetComponent<MeshRenderer>();
	}
}
