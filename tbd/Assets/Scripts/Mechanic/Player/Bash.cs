using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Tron amnesia

// teleport will set time scale to zero

public class Bash : MonoBehaviour 
{
    // Player components.
    public Rigidbody playerRigidbody;

    // Camera obect and components.
    public GameObject camera;
    public Transform cameraTransform;
    public Camera cameraCamera;

    public Image[] uiImages;

    public float bashEndTime;
    private float bashEndTimeElapsed;
    private float bashEndStartFov;

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
    public float radius;
    public float range;

    // Effect prefabs.
    public GameObject teleportLaserEffect;
    public GameObject teleportEffectCube;

    // The game object that holds the location where the teleport laser will
    // be emitted from.
    public GameObject teleportBarrel;

    public TeleporterTracker tracker;

    public GameObject UIReticle;

    public GameObject teleportColliderPrefab;


    // Private Section
    private float fovRange;
    private float initialFov;
    private float bashStoredPlayerSpeed;
    private float bashTimeElapsed;
    private bool testTeleport = false;
    private bool performingTeleport = false;
    private bool endingTeleport = false;
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
    private GameObject effectCube;
    private Material effectCubeMaterial;
    private Material reticleMaterial;
    private Teleporter teleporter;
    private GameObject teleportCollider;

    // The teleportStage delegate will be set depending on what stage of the
    // teleport the player is on.
    private delegate void TeleportStageDelegate();
    private TeleportStageDelegate teleportStage;

    void SetTransparencies(float transparency)
    {
        foreach(Image image_comp in uiImages)
        {
            Color color = image_comp.color;
            color[3] = transparency;
            image_comp.color = color;
        }
    }

    void EndingTeleport()
    {
        bashEndTimeElapsed += Time.unscaledDeltaTime;
        if(bashEndTimeElapsed >= bashEndTime)
        {
            endingTeleport = false;
            cameraCamera.fieldOfView = initialFov;
        }
        float lerp = bashEndTimeElapsed / bashEndTime;
        cameraCamera.fieldOfView = Mathf.Lerp(bashEndStartFov, initialFov, lerp);
    }

    void EndTeleport()
    {
        performingTeleport = false;
        endingTeleport = true;
        bashEndStartFov = cameraCamera.fieldOfView;
        bashEndTimeElapsed = 0.0f;
        Vector3 center_p = new Vector3(0.5f, 0.5f, cameraTransform.position.z);
        Ray ray = cameraCamera.ViewportPointToRay(center_p);
        float player_speed = bashEndSpeed;
        Vector3 direction = ray.direction.normalized * player_speed;
        playerRigidbody.velocity = direction * (bashEndSpeed);
        Time.timeScale = originalTimeScale;
        Destroy(effectCube);
        Destroy(teleportCollider);
        // Tell the telelporter that the player has performed their bash.
        teleporter.OnBash();
    }

    void TeleportStage1()
    {
        bashTimeElapsed += Time.unscaledDeltaTime;
        float perc = bashTimeElapsed / bashTime;
        float lerp = ActionOperation.QuadOut(perc);
        float new_fov = initialFov + lerp * fovRange;
        cameraCamera.fieldOfView = new_fov;
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
            float lp_quadout = ActionOperation.QuadOut(lp);
            float lp_quadin = ActionOperation.QuadIn(lp);
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
            lp = ActionOperation.QuadIn(lp);
            // Update the player's position.
            Vector3 new_pos = startPosition + lp * deltaPosition;
            this.transform.position = new_pos;
            // Update the player's fov.
            float max_fov = initialFov + fovRange;
            float new_fov = max_fov + lp * (initialFov - max_fov);
            cameraCamera.fieldOfView = new_fov;
        }
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
        Transform teleportObjectTransform = other.GetComponent<Transform>();
        endPosition = teleportObjectTransform.position;
        deltaPosition = endPosition - startPosition;
        teleportDirection = deltaPosition.normalized;
        teleportStage = TeleportStage0;

        // Notify the Teleporter that the player is beginning the teleport
        // sequence.
        teleporter = other.GetComponent<Teleporter>();
        teleporter.OnTeleport(teleportTime);

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

        // Create the telelport collider to see if the player collides with any
        // beacons.
        teleportCollider = Instantiate(teleportColliderPrefab);
        Vector3 collider_position = startPosition + deltaPosition / 2.0f;
        teleportCollider.transform.position = collider_position;
        float collider_length = deltaPosition.magnitude;
        Vector3 scale = teleportCollider.transform.localScale;
        scale.z = collider_length;
        teleportCollider.transform.localScale = scale;
        Quaternion rotation = Quaternion.LookRotation(deltaPosition);
        teleportCollider.transform.rotation = rotation;
        Collider collider = teleportCollider.GetComponent<Collider>();

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
        GameObject best_teleporter = tracker.ChooseBestTeleporter();
        reticleMaterial.SetFloat("_ContainMax", radius);
        if(best_teleporter == null)
        {
            return;
        }
        StartTeleport(best_teleporter);
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
        // Set the color of the reticle if a teleporter is within range.
        GameObject bestTeleporter = tracker.ChooseBestTeleporter();
        if(bestTeleporter != null)
        {
            reticleMaterial.SetVector("_Color", Color.blue);
            SetTransparencies(1.0f);
        }
        else
        {
            reticleMaterial.SetVector("_Color", Color.white);
            SetTransparencies(0.5f);
        }
        
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

        // Scale back the fov.
        if(endingTeleport)
        {
            EndingTeleport();
        }
        
    }

    void Start()
    {
        // Set the needed initial values for the teleport script.
        initialFov = cameraCamera.fieldOfView;
        fovRange = teleportFov - initialFov;
        teleportEffectFoldDelta = teleportEffectFoldEnd - 
                                  teleportEffectFoldStart;
        Image image_comp = UIReticle.GetComponent<Image>();
        reticleMaterial = image_comp.material;
    }
}

