using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour 
{
    public GameObject uiObject;

    public float bulletTimeScale;
    public float maxEnergy;
    public float depletionRate;
    public float regenerationRate;
    public KeyCode bulletTimeKey;

    private float currentEnergy;
    private bool inBulletTime = false;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void OnTimeSwitch()
    {
        if(!inBulletTime)
        {
            Time.timeScale = bulletTimeScale;
            Time.fixedDeltaTime = 0.02f * bulletTimeScale;
        }
        else
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime /= bulletTimeScale;
        }
        inBulletTime = !inBulletTime;
    }

    void Update () 
    {
        if(Input.GetKeyDown(bulletTimeKey))
        {
            OnTimeSwitch();
        }

        if(inBulletTime)
        {
            currentEnergy -= depletionRate * Time.unscaledDeltaTime;
            if(currentEnergy <= 0.0f)
            {
                OnTimeSwitch();
                currentEnergy = 0.0f;
            }
        }
        else if(currentEnergy < maxEnergy)
        {
            currentEnergy += regenerationRate * Time.unscaledDeltaTime;
            if(currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
        }

        float energy_percentage = currentEnergy / maxEnergy;
        uiObject.SendMessage("OnEnergyUpdate", energy_percentage);
    }
}
