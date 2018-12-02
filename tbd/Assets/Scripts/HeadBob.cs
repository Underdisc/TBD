////////////////////////////////////////////////////////////////////////////////
// Author: Connor Deakin
// Date: 18-19-23
//
// Contains the implementation for a basic first person head bob.
////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour 
{
    // The transform component of the camera object.
    public Transform _object_transform;
    // The max height the camera will be offset from its original position.
    public float _height;
    // The max distance to the left or right for the applied offset.
    public float _max;
    // The time it takes to complete one head bob cycle.
    public float _period;

    // The camera's startin position.
    private Vector3 _og_position;
    // The direction the input is moving on the model function.
    private float _direction;
    // The factor multiplied by delta time to obtain the input delta.
    private float _omega;
    // The input used for the previous frame.
    private float _prev_input;
    // The amount of contribution the head bob will affect the camera position.
    private float _contribution;

    // Update the contribution of the head bob. This should be a 0 to 1 value as
    // it used as an interpolation parameter.
    public void UpdateContribution(float new_contribution)
    {
        _contribution = new_contribution;
    }

    void Start () 
    {
        _og_position = _object_transform.localPosition;
        _direction = 1.0f;
        _prev_input = -_max;
        _contribution = 1.0f;

        // Omega must go from -_max to _max and back over the amount of time
        // specified by the _period.
        _omega = (_max * 4.0f) / _period;
    }
 
    float ModelFunction(float x)
    {
        // The model function written is an upside down quadratic. The height
        // of the quadratic is _height and it crosses the x axis at -_max and
        // max.
        float st = x / _max;
        float output = _height - _height * st * st;
        return output;
    }

    void Update () 
    {
        // If there is negative or no contribution, we immediatly exit the
        // update for the head bob.
        if(_contribution <= 0.0f)
        {
            return;
        }

        // Calculate the new input.
        float dt = Time.deltaTime;
        float input = _prev_input + _omega * dt * _direction; 

        // Change the direction the head bob is moving in if we reach the
        // maximum input value.
        if(Mathf.Abs(input) >= _max)
        {
            input = _direction * _max;
            _direction *= -1.0f;
        }

        // Calculate the output value, update the transform and save the input
        // used during this frame for the next..
        float output = ModelFunction(input);
        Vector3 new_position = _og_position;
        new_position.x += input * _contribution;
        new_position.y += output * _contribution;
        _object_transform.localPosition = new_position;
        _prev_input = input;
    }
}
