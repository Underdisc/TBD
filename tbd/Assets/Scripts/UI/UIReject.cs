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
		if(rejectEffect != null)
		{
			Destroy(rejectEffect);
		}
		rejectEffect = Instantiate(rejectEffectPrefab, gameObject.transform);
		rejectEffectTimeElapsed = 0.0f;
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
		percentage = ActionOperation.QuadIn(percentage);
		Image image_comp = rejectEffect.GetComponent<Image>();
		image_comp.material.SetFloat("_Percentage", percentage);
	}

	void Update()
	{
		UpdateRejectEffect();
	}
}
