using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReject : MonoBehaviour {

	public GameObject rejectRadialBar;
	public GameObject rejectEffectPrefab;
	public float rejectEffectTime;

	private GameObject rejectEffect;
	private float rejectEffectTimeElapsed;

	void OnReject()
	{
		rejectEffect = Instantiate(rejectEffectPrefab, gameObject.transform);
		rejectEffectTimeElapsed = 0.0f;
	}

	void OnRejectUpdate(float percentage)
	{
		// Update the UI to reflect how much energy the player has.
		Image image_comp = rejectRadialBar.GetComponent<Image>();
		image_comp.material.SetFloat("_Percentage", percentage);
	}

	void UpdateRejectEffect()
	{
		if(!rejectEffect)
		{
			return;
		}
		rejectEffectTimeElapsed += Time.deltaTime;
		if(rejectEffectTimeElapsed > rejectEffectTime)
		{
			Destroy(rejectEffect);
			rejectEffect = null;
			return;
		}
		float percentage = rejectEffectTimeElapsed / rejectEffectTime;
		percentage = Action.QuadIn(percentage);
		Image image_comp = rejectEffect.GetComponent<Image>();
		image_comp.material.SetFloat("_Percentage", percentage);
	}

	void Update()
	{
		UpdateRejectEffect();
	}
}
