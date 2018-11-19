using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tron amnesia

// teleport will set time scale to zero

public class Teleport : MonoBehaviour 
{
    public Rigidbody playerRigidbody;
    private Vector3 playerVelocity;

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
    private float teleportTimeElapsed;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool otherObjectTeleported = false;
    private Transform teleportObjectTransform;

    public GameObject teleportBarrel;

    void StartTeleport(GameObject other)
    {
        performingTeleport = true;
        playerVelocity = playerRigidbody.velocity;
        playerRigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        //originalTimeScale = Time.timeScale;
        //Time.timeScale = 0.0f;
        teleportTimeElapsed = 0.0f;
        startPosition = transform.position;
        teleportObjectTransform = other.GetComponent<Transform>();
        endPosition = teleportObjectTransform.position;
    }

    void EndTeleport()
    {
        performingTeleport = false;
        playerRigidbody.velocity = playerVelocity;
        //Time.timeScale = originalTimeScale;
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

        // We need to spawn the effect as well.
        teleportTimeElapsed += Time.deltaTime;
        if(teleportTimeElapsed >= teleportTime)
        {
            EndTeleport();
        }

        float perc = teleportTimeElapsed / teleportTime;

        // Change fov when the lerp param is less than 0.5;
        if(perc < 0.5f)
        {
            // l is our lerp parameter.
            float l = perc / 0.5f;
            l = QuadOut(l);
            // Update the player's fov.
            float new_fov = initialFov + l * fovRange;
            cameraCamera.fieldOfView = new_fov;
        }
        else
        {
            // l is our lerp parameter.
            float l = (perc - 0.5f) / 0.5f;
            l = QuadIn(l);
            // Update the player's position.
            Vector3 new_pos = startPosition + l * (endPosition - startPosition);
            this.transform.position = new_pos;
            // Update the player's fov.
            float max_fov = initialFov + fovRange;
            float new_fov = max_fov + l * (initialFov - max_fov);
            cameraCamera.fieldOfView = new_fov;
        }

        // Telport the object to where the player started the teleportation.
        if(perc > 0.5f && !otherObjectTeleported)
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

