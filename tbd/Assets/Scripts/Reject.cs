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

    public GameObject rejectBarrel;
    public GameObject rejectLaserEffect;

    public GameObject rejectorPrefab;
  
    private bool performReject = false;

    public void RejectInDirection(Vector3 direction)
    {
        direction.Normalize();
        Vector3 velocity = objectRigidbody.velocity;
        velocity += direction * rejectVelocity;
        objectRigidbody.velocity = velocity;
    }

    void OnReject(RaycastHit hit_information, Vector3 direction)
    {
        RejectInDirection(direction);
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

        Vector3 start_position = rejectBarrel.transform.position;
        Vector3 end_position;
        if(hit_occured)
        {
            OnReject(hit_information, -hitscan_ray.direction);
            end_position = hit_information.point;
        }
        else
        {
            end_position = position + forward * 100.0f;
        }
        GameObject laserEffect = Instantiate(rejectLaserEffect);
        LaserEffect effect_comp = laserEffect.GetComponent<LaserEffect>();
        effect_comp.SetPositions(start_position, end_position);
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
