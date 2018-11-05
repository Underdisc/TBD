using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tron amnesia

// teleport will set time scale to zero

public class Teleport : MonoBehaviour 
{

    public float rightEmissionOffset;
    public float downEmissionOffset;

    public Transform cameraTransform;
    public Camera cameraCamera;
    public bool isPlayer = false;
    public int teleportButton = 0;

    public float teleportFov;
    private float fovRange;
    private float initialFov;


    public float teleportTime = 0.1f;
    public GameObject TeleportLaserEffect;

    private bool usedTeleport = false;
    private bool testTeleport = false;
    private bool performingTeleport = false;
    private float originalTimeScale;
    private float teleportTimeRemaining;
    private Vector3 startPosition;
    private Vector3 teleportDestination;
    private bool otherObjectTeleported = false;
    private Transform teleportObjectTransform;

    public GameObject teleportBarrel;

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

    // Starts slowly and appproaches quickly.
    float QuadOut(float perc)
    {
        return perc * perc;
    }

    // Starts fast and approaches slowly.
    float QuadIn(float perc)
    {
        return ((-perc + 1.0f) * (perc - 1.0f)) + 1.0f;
    }

    // Starts slow, moves quickly through the center, and ends slow.
    float QuadOutIn(float perc)
    {
        float lerp_param;
        if(perc < 0.5)
        {
            perc *= 2.0f;
            lerp_param = QuadOut(perc);
            lerp_param /= 2.0f;
        }
        else
        {
            perc = perc - 0.5f;
            perc *= 2.0f;
            lerp_param = QuadIn(perc);
            lerp_param = 0.5f + lerp_param / 2.0f;
        }
        return lerp_param;
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
        float lerp_param = QuadOutIn(perc);
        transform.position = startPosition + toDestination * lerp_param;

        if(lerp_param > 0.5f)
        {
            lerp_param = 1.0f - lerp_param;
        }
        lerp_param = lerp_param / 0.5f;
        float new_fov = initialFov + lerp_param * fovRange;
        cameraCamera.fieldOfView = new_fov;

        // We need to create the particle effect around the teleporter.
        if(lerp_param > 0.5f && !otherObjectTeleported)
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
        Vector3 end_position;

        if(hit_occured)
        {
            GameObject hit_object = hit_information.collider.gameObject;
            if(hit_object.CompareTag("Teleporter"))
            {
                StartTeleport(hit_object);
            }
            end_position = hit_information.point;
        }
        else
        {
            end_position = position + forward * 100.0f;
        }
        
        // Start drawing the teleport beam.
        GameObject laserEffect = Instantiate(TeleportLaserEffect);
        LaserEffect effect_comp = laserEffect.GetComponent<LaserEffect>();
        Vector3 start_position = teleportBarrel.transform.position;
        effect_comp.SetPositions(start_position, end_position);
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

    void Start()
    {
        initialFov = cameraCamera.fieldOfView;
        fovRange = teleportFov - initialFov;
    }
}

