using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour {

    public Color startColor;
    public Color endColor;

    private Color toEndColor;

    public float fadeTime;
    private float fadeTimeLeft;

    public LineRenderer lineRenderer;

    void Start () 
    {
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        fadeTimeLeft = fadeTime;
    }
  
    void Update () 
    {
        fadeTimeLeft = fadeTimeLeft - Time.deltaTime;
        if(fadeTimeLeft <= 0.0f)
        {
            Destroy(gameObject);
            return;
        }
        float lerp_param = (1.0f - fadeTimeLeft) / fadeTime;
        Color current_color = Color.Lerp(startColor, endColor, lerp_param);
        lineRenderer.SetColors(current_color, current_color);
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        transform.position = start;
        Vector3 direction = end - start;
        direction.Normalize();
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction, up);
    }
}
