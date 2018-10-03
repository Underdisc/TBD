using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour 
{

    public float bounceSpeed;
    public float maxBounceSpeed;

    private Vector3 bounceDirection;

    Vector3 PerformBounce(Vector3 velocity)
    {
        velocity = bounceDirection * bounceSpeed;
        return velocity;
    }

    void Update()
    {
        Transform objectTransform = gameObject.GetComponent("Transform") as Transform;

        Vector3 start = objectTransform.position;
        Vector3 end = start + bounceDirection * 5.0f;
        Debug.DrawLine(start, end, Color.green);
    }

    void Start()
    {
        Transform objectTransform = gameObject.GetComponent("Transform") as Transform;
        Quaternion object_quat = objectTransform.rotation;

        bounceDirection = new Vector3(0.0f, 1.0f, 0.0f);
        bounceDirection = object_quat * bounceDirection;
    }
}
