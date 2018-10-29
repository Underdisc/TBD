using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tron amnesia

// teleport will set time scale to zero

public class Teleport : MonoBehaviour 
{
    // Player and Other teleport functions
    private delegate void VoidDelegateVoid();
    private VoidDelegateVoid run;
    private VoidDelegateVoid teleport;

    public float rightEmissionOffset;
    public float downEmissionOffset;

    public Transform cameraTransform;
    public Camera cameraCamera;
    public bool isPlayer = false;
    public int teleportButton = 0;


    public static float teleportTime = 0.1f;
    private bool testTeleport = false;
    private bool performingTeleport = false;
    private float originalTimeScale;
    private float teleportTimeRemaining;
    private Vector3 startPosition;
    private Vector3 teleportDestination;
    private bool otherObjectTeleported = false;
    private Transform teleportObjectTransform;

    void StartTeleport(GameObject other)
    {
        performingTeleport = true;
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        teleportTimeRemaining = teleportTime;
        startPosition = transform.position;
        teleportObjectTransform = other.GetComponent<Transform>();
        teleportDestination = teleportObjectTransform.position;
    }

    void EndTeleport()
    {
        performingTeleport = false;
        Time.timeScale = originalTimeScale;
    }

    void PlayerTeleport()
    {
        // This should be exchanged for a quadratic in and out linear interpolation
        // function.

        // We need to spawn the effect as well.
        Vector3 toDestination = teleportDestination - startPosition;
        teleportTimeRemaining -= Time.unscaledDeltaTime;
        if(teleportTimeRemaining <= 0.0f)
        {
            teleportTimeRemaining = 0.0f;
            EndTeleport();
        }

        float perc = 1.0f - (teleportTimeRemaining / teleportTime);
        transform.position = startPosition + toDestination * perc; 

        // We need to create the particle effect around the teleporter.
        if(perc < 0.5f && !otherObjectTeleported)
        {
            teleportObjectTransform.position = startPosition;
        }

    }
    
    void TryTeleport()
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
            if(hit_object.CompareTag("Teleporter"))
            {
                StartTeleport(hit_object);
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
        if(testTeleport)
        {
            TryTeleport();
            testTeleport = false;
        }
    }

    void PlayerUpdate()
    {
        if(Input.GetMouseButtonDown(teleportButton))
        {
            testTeleport = true;
        }
    }

    void Update()
    {
        if(performingTeleport)
        {
            PlayerTeleport();
        }
        else
        {
            PlayerUpdate();
        }
        
    }
}

