using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTracker : MonoBehaviour 
{
	public GameObject player;
	public GameObject playerCameraObject;
	
	private GameObject[] teleportersInViewport;
	private Camera playerCamera;
	private int numTeleporters;
	private int inViewportCount;

	public GameObject Raycast(Ray ray)
	{
		bool hit_occured;
		RaycastHit hit_info;
		hit_occured = Physics.Raycast(ray, out hit_info);
		if(hit_occured)
		{
			return hit_info.collider.gameObject;
		}
		return null;
	}

	public bool RaycastTest(GameObject teleporter)
	{
		Vector3 origin = playerCamera.transform.position;
		Vector3 direction = teleporter.transform.position - origin;
		Ray ray = new Ray(origin, direction);
		GameObject hit_object = Raycast(ray);
		if(hit_object == teleporter)
		{
			return true;
		}
		return false;
	}
	

	public GameObject ChooseBestTeleporter(float reticule_radius)
	{
		GameObject best = null;
		float best_dist = float.MaxValue;
		for(int i = 0; i < inViewportCount; ++i)
		{
			GameObject teleporter = teleportersInViewport[i];
			Vector3 location
				= playerCamera.WorldToViewportPoint(teleporter.transform.position);
			Vector2 screen_point = new Vector2(location.x, location.y);
			Vector2 center_delta = (screen_point - new Vector2(0.5f, 0.5f)) * 2.0f;
			center_delta.x *= playerCamera.aspect;
			float center_distance = center_delta.magnitude;
			if(center_distance > reticule_radius)
			{
				continue;
			}
			Vector3 player_pos = player.transform.position;
			Vector3 teleporter_pos = teleporter.transform.position;
			Vector3 p_t_d = player_pos - teleporter_pos;
			float dist = p_t_d.sqrMagnitude;
			if(dist > best_dist)
			{
				continue;
			}
			best_dist = dist;
			if(!RaycastTest(teleporter))
			{
				continue;
			}
			best = teleporter;
		}
		return best;
	}

	void Start()
	{
		playerCamera = playerCameraObject.GetComponent<Camera>();
		numTeleporters = gameObject.transform.childCount;
		teleportersInViewport = new GameObject[numTeleporters];
	}

	void Update () 
	{
		inViewportCount = 0;
		for(int i = 0; i < numTeleporters; ++i)
		{
			GameObject teleporter = gameObject.transform.GetChild(i).gameObject;
			Renderer renderer = teleporter.GetComponent<Renderer>();
			if(renderer.isVisible)
			{
				teleportersInViewport[inViewportCount] = teleporter;
				inViewportCount++;
			}
		}
	}
}
