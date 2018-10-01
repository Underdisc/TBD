using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tron amnesia

public class Teleport : MonoBehaviour 
{
    private delegate void RunDelegate();
    private RunDelegate run;

    public float rightEmissionOffset;
    public float downEmissionOffset;

    public Transform cameraTransform;
    public Camera cameraCamera;
    public bool isPlayer = false;
    public int teleportButton = 0;

    private int clickCount = 0;
    private bool performTeleport = false;

    void OnTeleport(GameObject other)
    {
        Vector3 old_position = this.transform.position;
        this.transform.position = other.transform.position;
        other.transform.position = old_position;
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
            GameObject hit_object = hit_information.collider.gameObject;
            if(hit_object.CompareTag("Enemy"))
            {
                OnTeleport(hit_object);
            }
            end = hit_information.point;
        }
        else
        {
            end = position + forward * 100.0f;
        }
        
        Debug.DrawLine(start, end, Color.blue, 0.5f);
    }

    void FixedUpdate () 
    {
        if(performTeleport)
        {
            TryRaycast();
            performTeleport = false;
        }
    }

    void PlayerUpdate()
    {
        if(Input.GetMouseButtonDown(teleportButton))
        {
            performTeleport = true;
        }
    }

    void OtherUpdate()
    {}

    void Update()
    {
        run();
    }

    void OnEnable()
    {
        if(isPlayer)
        {
            run = PlayerUpdate;
        }
        else
        {
            run = OtherUpdate;
        }
    }
}

