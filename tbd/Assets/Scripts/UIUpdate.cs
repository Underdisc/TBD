using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdate : MonoBehaviour 
{
    public Text objectTextMesh;

    void OnEnergyUpdate(float percentage)
    {
        int int_percentage = (int)(percentage * 100.0f);
        string string_percentage = int_percentage.ToString();
        string new_text = "Energy: " + string_percentage + "%";
        objectTextMesh.text = new_text;
    }
}
