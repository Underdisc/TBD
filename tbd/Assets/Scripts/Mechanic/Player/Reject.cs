using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct RaycastResult
{
    public bool hitOccured;
    public RaycastHit information;
}

public class Reject : MonoBehaviour 
{
    public Transform cameraTransform;
    public Camera cameraCamera;
    public Rigidbody objectRigidbody;

    public float range;
    public float rejectSpeed;
    private int pipsAvailable;
    public float pipTime;
    private float pipTimePassed;
    public Image pipHolderImage;
    public Image[] pipImages;

    public int rejectButton = 1;

    public GameObject rejectBarrel;
    public GameObject rejectLaserEffect;
    public GameObject rejectorPrefab;
  
    private bool performReject = false;

    public void RejectInDirection(Vector3 direction)
    {
        direction.Normalize();
        Vector3 velocity = objectRigidbody.velocity;
        velocity = direction * rejectSpeed;
        objectRigidbody.velocity = velocity;
    }



    RaycastResult PerformRaycast()
    {
        RaycastHit hit_information;
        Vector3 position = cameraTransform.position;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        Vector3 up = cameraTransform.up;

        int layer_mask = 1 << 8;
        layer_mask = ~layer_mask;

        Vector3 center  = new Vector3(0.5f, 0.5f, cameraTransform.position.z);
        Ray hitscan_ray = cameraCamera.ViewportPointToRay(center);

        RaycastResult result;
        result.hitOccured = Physics.Raycast(hitscan_ray, out hit_information);
        result.information = hit_information;
        return result;
    }

    void FixedUpdate()
    {
        if(performReject)
        {
            //TryRaycast();
            performReject = false;
        }
    }

    void ChangeTransparencies(float transparency)
    {
        Color color;
        foreach(Image image_comp in pipImages)
        {
            color = image_comp.color;
            color[3] = transparency;
            image_comp.color = color;
        }
        color = pipHolderImage.color;
        color[3] = transparency;
        pipHolderImage.color = color;
    }

    void CreateLaser(RaycastResult cast)
    {
        Vector3 start_position = rejectBarrel.transform.position;
        Vector3 end_position;
        if(cast.hitOccured)
        {
            end_position = cast.information.point;
        }
        else
        {
            Vector3 position = cameraTransform.position;
            Vector3 forward = cameraTransform.forward;
            end_position = position + forward * 100.0f;
        }
        GameObject laserEffect = Instantiate(rejectLaserEffect);
        LaserEffect effect_comp = laserEffect.GetComponent<LaserEffect>();
        effect_comp.SetPositions(start_position, end_position);
    }
  
    void Update () 
    {
        if(pipsAvailable < pipImages.Length)
        {
            pipTimePassed += Time.unscaledDeltaTime;
            if(pipTimePassed > pipTime)
            {
                pipImages[pipsAvailable].enabled = true;
                pipsAvailable++;
                if(pipsAvailable == pipImages.Length)
                {
                    pipTimePassed = 0.0f;
                }
                else
                {
                    pipTimePassed = pipTimePassed - pipTime;
                }
            }
        }

        RaycastResult raycast = PerformRaycast();
        if(raycast.hitOccured && raycast.information.distance <= range)
        {
            ChangeTransparencies(1.0f);
            if(Input.GetMouseButtonDown(rejectButton) && pipsAvailable > 0)
            {
                performReject = true;
                pipsAvailable--;
                pipImages[pipsAvailable].enabled = false;
                Vector3 reject_direction = cameraTransform.position - raycast.information.point;
                RejectInDirection(reject_direction);
                CreateLaser(raycast);
            }
        }
        else
        {
            ChangeTransparencies(0.5f);
        }
    }

    void Start()
    {
        pipTimePassed = 0.0f;
        pipsAvailable = pipImages.Length;
    }
}
