using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergy : MonoBehaviour 
{
    public float vignetteTransitionTime;
    public Vector2 vignetteMinMax;
    public GameObject vignettePrefab;
    public GameObject energyRadialBar;

    // vignette privates
    private GameObject vignette;
    private bool expanding;
    private bool retracting;
    private bool transitioning;
    private float elapsedTransitionTime;
    private const float vignetteAbsoluteMax = 1.5f;

    // Should be called when the player stops using there energy.
    void DeactivateEnergyVignette()
    {
        // Switch to retracting mode if we are in expanding mode, otherwise,
        // begin the retracting mode.
        if(expanding)
        {
            expanding = false;
            elapsedTransitionTime = vignetteTransitionTime - elapsedTransitionTime;
        }
        else
        {
            elapsedTransitionTime = 0.0f;
        }
        retracting = true;
        transitioning = true;
    }

    // Should be called when the player begins using their energy.
    void ActivateEnergyVignette()
    {
        // Create the vignette if it does not exist.
        if(vignette == null)
        {
            vignette = Instantiate(vignettePrefab, this.transform);
        }

        // Make a retracting vignette an expanding vignette, otherwise, begin
        // the expanding process.
        if(retracting)
        {
            retracting = false;
            elapsedTransitionTime = vignetteTransitionTime - elapsedTransitionTime;
        }
        else
        {
            elapsedTransitionTime = 0.0f;
        }
        expanding = true;
        transitioning = true;
    }

    // Should be each frame to reflect percentage changes.
    void OnEnergyUpdate(float percentage)
    {
        // Update the UI to reflect how much energy the player has.
        Image image_comp = energyRadialBar.GetComponent<Image>();
        image_comp.material.SetFloat("_Percentage", percentage);

        // Update the time for the vignette if it exists.
        if(vignette != null)
        {
            Image image = vignette.GetComponent<Image>();
            float unscaled_time = Time.unscaledTime;
            image.material.SetFloat("_UnscaledTime", unscaled_time);
        }

        // Stop if we are not transitioning.
        if(!transitioning)
        {
            return;
        }

        // Test to see if the transition has ended.
        elapsedTransitionTime += Time.unscaledDeltaTime;
        if(elapsedTransitionTime >= vignetteTransitionTime)
        {
            // Destroy the vignette if we were retracting.
            if(retracting)
            {
                Destroy(vignette);
                vignette = null;
            }
            expanding = false;
            retracting = false;
            transitioning = false;
            return;
        }

        // Find the linear interpolation parameter and decide the min
        // and max for the vignette with the linear interpolation parameter.
        float lerp_param = elapsedTransitionTime / vignetteTransitionTime;
        float vignette_min = 0.0f;
        float vignette_max = 0.0f;
        if(expanding)
        {
            vignette_min = Math.QuadInLerp(vignetteAbsoluteMax, 
                                           vignetteMinMax.x, 
                                           lerp_param);
            vignette_max = Math.QuadInLerp(vignetteAbsoluteMax, 
                                           vignetteMinMax.y, 
                                           lerp_param);
        }
        else if(retracting)
        {
            vignette_min = Math.QuadOutLerp(vignetteMinMax.x, 
                                            vignetteAbsoluteMax, 
                                            lerp_param);
            vignette_max = Math.QuadOutLerp(vignetteMinMax.y, 
                                            vignetteAbsoluteMax, 
                                            lerp_param);
        }
        
        // Apply the vignette min and max.
        image_comp = vignette.GetComponent<Image>();
        Vector4 vignette_min_max = new Vector4(vignette_min, vignette_max, 0.0f, 0.0f);
        image_comp.material.SetVector("_VignetteMinMax", vignette_min_max);

    }
}
