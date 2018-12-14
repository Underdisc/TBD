using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour 
{
    public bool disableCursor;

    public KeyCode cursorToggle;
    private bool cursorToggled = false;

    void ToggleCursor()
    {
        if(!cursorToggled)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        cursorToggled = !cursorToggled;
    }
    
    void Update () 
    {
        if(Input.GetKeyDown(cursorToggle))
        {
            ToggleCursor();
        }
    }

    void Start()
    {
        if(disableCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
