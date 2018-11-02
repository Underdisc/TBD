using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRequest : MonoBehaviour {

    public float destroyWaitTime;
    private float destoryWaitTimeLeft;
    private bool destoying;

    void Update () 
    {
        if(destoying)
        {
            destoryWaitTimeLeft -= Time.deltaTime;
            if(destoryWaitTimeLeft <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Destroy()
    {
        destoryWaitTimeLeft = destroyWaitTime;
        destoying = true;
    }
}
