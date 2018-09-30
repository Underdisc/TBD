using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class MathExtra
{
    public static int Sign(float input)
    {
       if(input > 0.0f)
         return 1;
     if(input < 0.0f)
         return -1;
     return 0;
    }
}

public class IntVector3
{
    public int x;
    public int y;
    public int z;

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static IntVector3 Sign(Vector3 vector)
    {
        IntVector3 output = new IntVector3(0, 0, 0);
        output.x = MathExtra.Sign(vector.x);
        output.y = MathExtra.Sign(vector.y);
        output.z = MathExtra.Sign(vector.z); 
        return output;
    }

    public static bool operator==(IntVector3 lhs, IntVector3 rhs)
    {
        if(lhs != rhs)
        {
            return false;
        }
        return true;
    }
    
    
    public static bool operator!=(IntVector3 lhs, IntVector3 rhs)
    {
        return (lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z);
    }
}

public class Controller : MonoBehaviour 
{

    public Transform transform;
    public Rigidbody rigidbody;
    public HeadBob headBob;

    public float xSensitivity;
    public float zSensitivity;

    public float maxGroundSpeed;
    public float groundAcceleration;

    public float airAcceleration;
    public float gravityAcceleration;
    public float drag;

    // Privates
    private Quaternion ogQuat;
    private float yRotation;
    private float zRotation;
    private bool grounded;

    // Declare the keys that will be used to detect movement.
    private System.String xAxisInput;
    private System.String yAxisInput;
    private KeyCode forwardKey;
    private KeyCode backwardKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode jumpKey;

    void Start () 
    {
        ogQuat = transform.localRotation;
        yRotation = transform.eulerAngles.y;
        zRotation = transform.eulerAngles.z;
        
        // Set the keys used for movment detection.
        xAxisInput = "Mouse X";
        yAxisInput = "Mouse Y";
        forwardKey = KeyCode.W;
        backwardKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        jumpKey = KeyCode.Space;
    }

    Vector3 Decelerate(Vector3 velocity)
    {
        // Check to see if the velocity is already the zero vector and exit if
        // it is.
        IntVector3 zero_vector = new IntVector3(0, 0, 0);
        IntVector3 og_sign_vector = IntVector3.Sign(velocity);
        if(og_sign_vector == zero_vector)
        {
            return velocity;
        }
        
        // Bring the velocity closer to the zero vector.
        Vector3 velocity_unit = velocity.normalized;
        velocity -= Time.deltaTime * groundAcceleration * velocity_unit;

        // If we happen to pass over the zero vector, that means the player
        // should stop moving.
        IntVector3 new_sign_vector = IntVector3.Sign(velocity);
        if(og_sign_vector != new_sign_vector)
        {
            velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
        return velocity;
    }

    Vector3 GroundMovement(Vector3 velocity, Vector3 forward, Vector3 left)
    {
        // Before progressing with any ground movement, check to see if the
        // player is jumping. If they are, there is no need to perform any other
        // ground movment.
        if(Input.GetKeyDown(jumpKey))
        {
            velocity.y += 8.0f;
            return velocity;
        }

        // Find the change in the velocity of the player.
        bool input = false;
        Vector3 d_velocity = new Vector3(0.0f, 0.0f, 0.0f);
        if(Input.GetKey(forwardKey))
        {
            d_velocity += Time.deltaTime * groundAcceleration * forward;
            input = true;
        }
        if(Input.GetKey(backwardKey))
        {
            d_velocity -= Time.deltaTime * groundAcceleration * forward;
            input = true;
        }
        if(Input.GetKey(leftKey))
        {
            d_velocity += Time.deltaTime * groundAcceleration * left;
            input = true;
        }
        if(Input.GetKey(rightKey))
        {
            d_velocity -= Time.deltaTime * groundAcceleration * left;
            input = true;
        }

        // If the player provides no input, we bring the velocity back down to
        // zero.
        if(!input)
        {
            velocity = Decelerate(velocity);
        }

        // If the speed of the velocity vector is greater than the max speed, 
        // we scale the velocity vector back to have a magnitude equivalent to
        // maxGroundSpeed.
        velocity += d_velocity;
        float speed = velocity.magnitude;
        if(speed > maxGroundSpeed)
        {
            float scale = maxGroundSpeed / speed;
            velocity *= scale;
        }

        return velocity;
    }

    Vector3 AirMovement(Vector3 velocity, Vector3 forward, Vector3 left)
    {
        velocity -= Time.deltaTime * gravityAcceleration * Vector3.down; 
        return velocity;
    }

    void Update () 
    {
        // Apply the rotation around the y axis with the x axis mouse input.
        float x_input = Input.GetAxis(xAxisInput);
        float d_y_rotation = Time.deltaTime * x_input * xSensitivity * 360.0f;
        yRotation += d_y_rotation;
        yRotation %= 360.0f;

        // Do the same same for the rotation around the z axis.
        float y_input = Input.GetAxis(yAxisInput);
        float d_z_rotation = Time.deltaTime * y_input * zSensitivity * 360.0f;
        zRotation += d_z_rotation;
        zRotation = Mathf.Clamp(zRotation, -90.0f, 90.0f);
        
        // Rotate the transorm with the new rotation values.
        Quaternion y_quat = Quaternion.AngleAxis(yRotation, Vector3.up);
        Quaternion z_quat = Quaternion.AngleAxis(zRotation, Vector3.left);
        transform.localRotation = ogQuat * y_quat * z_quat;

        // Get the 2 dimension horizontal vector that describes where the player
        // is looking. We use the rotation from the transfrom because the
        // rotation values defined in this file are relative to the player's
        // starting position.
        float y_rotation = transform.eulerAngles.y;
        float rad_y_rotation = Mathf.PI * (y_rotation / 180.0f);
        Vector3 forward_heading;
        forward_heading.x = Mathf.Sin(rad_y_rotation);
        forward_heading.y = 0.0f; 
        forward_heading.z = Mathf.Cos(rad_y_rotation);
        
        // Get another horizontal vector that is pointing directly to the left
        // of where the player is looking.
        float rad_y_rotation_offset = rad_y_rotation + (Mathf.PI / 2.0f);
        Vector3 left_heading;
        left_heading.x = -Mathf.Sin(rad_y_rotation_offset);
        left_heading.y = 0.0f; 
        left_heading.z = -Mathf.Cos(rad_y_rotation_offset);

        // We use the velocity on the rigibody component instead of storing the
        // velocity of the controller component. If keep a copy of our own
        // velocity, unity will not take care of updating the vector like the
        // rigidbody will. We want custom controls, but we do not want custom
        // physics. 
        Vector3 velocity = rigidbody.velocity;

        // Apply ground movement controls when the player is on the ground and
        // air movement controls when they are not.
        if(grounded)
        {
            velocity = GroundMovement(velocity, forward_heading, left_heading);

            // Update the head bob contribution depending on how close the 
            // player is to the max speed.
            float speed = velocity.magnitude;
            float head_bob_contribution;
            if(speed == 0.0f)
            {
                head_bob_contribution = 0.0f;
            }
            else
            {
                head_bob_contribution = speed / maxGroundSpeed;
            }
            headBob.UpdateContribution(head_bob_contribution);
        }
        else
        {
            velocity = AirMovement(velocity, forward_heading, left_heading);

            // Do not perform head bobbing while flying through the air.
            headBob.UpdateContribution(0.0f);
        }
        
        // Reset the velocity of the rigidody for the next physics updat.
        rigidbody.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // The player is grounded when they collide with the ground.
        if(collision.gameObject.CompareTag("ground"))
        {
            grounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // The player is not grounded when they exit collision with the ground.
        if(collision.gameObject.CompareTag("ground"))
        {
            grounded = false;
        }
    }
}


