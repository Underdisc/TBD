using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour 
{

    public float bounceSpeed;
    private Vector3 bounceVector;

    public Vector3 PerformBounce()
    {
        return bounceVector;
    }

    void Update()
    {
        Transform objectTransform = gameObject.GetComponent("Transform") as Transform;

        Vector3 start = objectTransform.position;
        Vector3 end = start + bounceVector;
        Debug.DrawLine(start, end, Color.green);
    }

    void Start()
    {
        Transform objectTransform = gameObject.GetComponent("Transform") as Transform;
        Quaternion object_quat = objectTransform.rotation;

        Vector3 bounceDirection = new Vector3(0.0f, 1.0f, 0.0f);
        bounceDirection = object_quat * bounceDirection;
        bounceVector = bounceSpeed * bounceDirection;
    }
}
