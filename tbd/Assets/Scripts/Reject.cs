using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reject : MonoBehaviour 
{
    public Transform cameraTransform;
    public Camera cameraCamera;
    public Rigidbody objectRigidbody;
    public float rejectVelocity;
    public float rightEmissionOffset;
    public float downEmissionOffset;
    public int rejectButton = 1;
  
    private bool performReject = false;

    void OnReject(RaycastHit hit_information, Vector3 ray_vector)
    {
        Vector3 velocity = objectRigidbody.velocity;
        Vector3 reject_direction = -ray_vector.normalized;
        velocity += reject_direction * rejectVelocity;
        objectRigidbody.velocity = velocity;
    }

    void TryRaycast()
    {
        bool hit_occured;
        RaycastHit hit_information;
        Vector3 position = cameraTransform.position;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        Vector3 up = cameraTransform.up;

        int layer_mask = 1 << 8;
        layer_mask = ~layer_mask;

        Vector3 center = new Vector3(0.5f, 0.5f, cameraTransform.position.z);
        Ray hitscan_ray = cameraCamera.ViewportPointToRay(center);
        
        hit_occured = Physics.Raycast(hitscan_ray,
                                      out hit_information);

        Vector3 start = position + right * rightEmissionOffset;
            start -= up * downEmissionOffset;
        Vector3 end;

        if(hit_occured)
        {
            OnReject(hit_information, hitscan_ray.direction);
            end = hit_information.point;
        }
        else
        {
            end = position + forward * 100.0f;
        }
        
        Debug.DrawLine(start, end, Color.red, 0.5f);

    }

    void FixedUpdate()
    {
        if(performReject)
        {
            TryRaycast();
            performReject = false;
        }
    }
  
    void Update () 
    {
        if(Input.GetMouseButtonDown(rejectButton))
        {
            performReject = true;
        }
    }
}
