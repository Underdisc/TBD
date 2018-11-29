using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Tron amnesia

// teleport will set time scale to zero

public class Teleport : MonoBehaviour 
{
    // Player components.
    public Rigidbody playerRigidbody;

    // Camera obect and components.
    public GameObject camera;
    public Transform cameraTransform;
    public Camera cameraCamera;

    // Bash options.
    public float bashEndSpeed;
    public float bashTime;

    // Teleport options.
    public float teleportTime;
    public float teleportMidpoint;
    public float teleportFov;
    public float teleportEffectFoldStart;
    public float teleportEffectFoldEnd;
    public float teleportEffectDissolveTime;

    // Teleport ability optins.
    public int teleportButton = 0;  

    // Effect prefabs.
    public GameObject teleportLaserEffect;
    public GameObject teleportEffectCube;

    // The game object that holds the location where the teleport laser will
    // be emitted from.
    public GameObject teleportBarrel;

    // Privates.
    private float fovRange;
    private float initialFov;
    private float bashStoredPlayerSpeed;
    private float bashTimeElapsed;
    private bool testTeleport = false;
    private bool performingTeleport = false;
    private float originalTimeScale;
    private float teleportTimeElapsed;
    private float timeInTeleport;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 deltaPosition;
    private Vector3 teleportDirection;
    private float teleportEffectFoldDelta;
    private bool teleportEffectDissolving = false;
    private float teleportEffectDissolveTimeElapsed;
    private bool otherObjectTeleported = false;
    private Transform teleportObjectTransform;
    private GameObject effectCube;
    private Material effectCubeMaterial;


    // The teleportStage delegate will be set depending on what stage of the
    // teleport the player is on.
    delegate void TeleportStageDelegate();
    private TeleportStageDelegate teleportStage;

    void EndTeleport()
    {
        performingTeleport = false;
        Vector3 center_p = new Vector3(0.5f, 0.5f, cameraTransform.position.z);
        Ray ray = cameraCamera.ViewportPointToRay(center_p);
        float player_speed = bashEndSpeed + bashStoredPlayerSpeed;
        playerRigidbody.velocity = ray.direction * (bashEndSpeed);
        Time.timeScale = originalTimeScale;
        Destroy(effectCube);
    }

    void TeleportStage1()
    {
        bashTimeElapsed += Time.unscaledDeltaTime;
        if(bashTimeElapsed >= bashTime || 
           Input.GetMouseButtonDown(teleportButton))
        {
            EndTeleport();
        }
    }

    void TeleportTransitionStage0Stage1()
    {
        teleportStage = TeleportStage1;
        this.transform.position = endPosition;
        bashTimeElapsed = 0.0f;
        teleportEffectDissolveTimeElapsed = 0.0f;
        teleportEffectDissolving = true;
    }

    void TeleportStage0()
    {
        teleportTimeElapsed += Time.unscaledDeltaTime;
        if(teleportTimeElapsed >= teleportTime)
        {
            TeleportTransitionStage0Stage1();
        }

        float perc = teleportTimeElapsed / teleportTime;

        // Change fov when the lerp param is less than 0.5;
        if(perc < teleportMidpoint)
        {
            // l is our lerp parameter.
            float lp = perc / teleportMidpoint;
            float lp_quadout = Action.QuadOut(lp);
            float lp_quadin = Action.QuadIn(lp);
            // Update the player's fov.
            float new_fov = initialFov + lp_quadout * fovRange;
            cameraCamera.fieldOfView = new_fov;
            // Update the material
            float contain_min = teleportEffectFoldStart +
                                lp_quadout * teleportEffectFoldDelta;
            effectCubeMaterial.SetFloat("_ContainMin", contain_min);
        }
        else
        {
            // lp is our lerp parameter.
            float lp = (perc - teleportMidpoint) / (1.0f - teleportMidpoint);
            lp = Action.QuadIn(lp);
            // Update the player's position.
            Vector3 new_pos = startPosition + lp * deltaPosition;
            this.transform.position = new_pos;
            // Update the player's fov.
            float max_fov = initialFov + fovRange;
            float new_fov = max_fov + lp * (initialFov - max_fov);
            cameraCamera.fieldOfView = new_fov;
        }

        // Telport the object to where the player started the teleportation.
        // if(perc > teleportMidpoint && !otherObjectTeleported)
        // {
        //     teleportObjectTransform.position = startPosition;
        // } 
        //*Dylan Note: disabled for having fixed position teleporters
        // Setting collider to trigger to avoid collision with player
    }

    void teleportGeneral()
    {
        timeInTeleport += Time.unscaledDeltaTime;
        effectCubeMaterial.SetFloat("_CurrentTime", timeInTeleport);
    }

    void PlayerTeleport()
    {
        teleportStage();
        teleportGeneral();
    }

    void StartTeleport(GameObject other)
    {
        performingTeleport = true;
        bashStoredPlayerSpeed = playerRigidbody.velocity.magnitude;
        playerRigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        teleportTimeElapsed = 0.0f;
        timeInTeleport = 0.0f;
        startPosition = transform.position;
        teleportObjectTransform = other.GetComponent<Transform>();
        endPosition = teleportObjectTransform.position;
        deltaPosition = endPosition - startPosition;
        teleportDirection = deltaPosition.normalized;
        teleportStage = TeleportStage0;

        // Create the teleport effect cube.
        effectCube = Instantiate(teleportEffectCube);
        Attach attach_comp = effectCube.GetComponent<Attach>();
        attach_comp.attachedToObject = camera;
        // Set the forward direction on the effect cube to indicate the
        // direction that the player is teleporting.
        MeshRenderer mesh_comp = effectCube.GetComponent<MeshRenderer>();
        effectCubeMaterial = mesh_comp.material;
        Vector4 material_forward = new Vector4(teleportDirection.x,
                                               teleportDirection.y,
                                               teleportDirection.z,
                                               0.0f);
        effectCubeMaterial.SetVector("_Forward", material_forward);
        // Make sure all of the effect cube is visible.
        effectCubeMaterial.SetFloat("_DissolvePercentage", 0.0f);
        // Begin the teleport sequence with the first teleport call.
        PlayerTeleport();
    }

    void DissolveTeleportEffect()
    {
        teleportEffectDissolveTimeElapsed += Time.unscaledDeltaTime;
        if(teleportEffectDissolveTimeElapsed > teleportEffectDissolveTime)
        {
            Destroy(effectCube);
            teleportEffectDissolving = false;
        }
        float elapsed = teleportEffectDissolveTimeElapsed;
        float total = teleportEffectDissolveTime;
        float dissolve_perc = elapsed / total;
        effectCubeMaterial.SetFloat("_DissolvePercentage", dissolve_perc);
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
        GameObject laserEffect = Instantiate(teleportLaserEffect);
        LaserEffect effect_comp = laserEffect.GetComponent<LaserEffect>();
        Vector3 start_position = teleportBarrel.transform.position;
        effect_comp.SetPositions(start_position, end_position);
    }

    void FixedUpdate () 
    {
        if(testTeleport)
        {
            // Check to see if the player's teleport request is valid.
            TryTeleport();
            testTeleport = false;
        }
    }

    void CheckTeleportRequest()
    {
        // If the player attempts to teleport, whether a teleport occurs or not
        // will be checked for during the fixed update.
        if(Input.GetMouseButtonDown(teleportButton))
        {
            testTeleport = true;
        }
    }

    void Update()
    {
        // If the player is in the middle of a teleport, continue performing the
        // teleport. If not, check for a teleport request.
        if(performingTeleport)
        {
            PlayerTeleport();
        }
        else
        {
            CheckTeleportRequest();
        }

        // Dissolve the teleport affect.
        if(teleportEffectDissolving)
        {
            DissolveTeleportEffect();
        }
        
    }

    void Start()
    {
        // Set the needed initial values for the teleport script.
        initialFov = cameraCamera.fieldOfView;
        fovRange = teleportFov - initialFov;
        teleportEffectFoldDelta = teleportEffectFoldEnd - 
                                  teleportEffectFoldStart;
    }
}

