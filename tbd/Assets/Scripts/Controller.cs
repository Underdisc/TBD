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
    //public HeadBob headBob;

    // Should be seperated from the controller
    public float bounceSpeed;
    public float maxBounceSpeed;

    public float height;
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
    private Vector3 respawnPosition;

    // Declare the keys that will be used to detect movement.
    private System.String xAxisInput;
    private System.String yAxisInput;
    private KeyCode forwardKey;
    private KeyCode backwardKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode jumpKey;
    private KeyCode respawnKey;

    void Start () 
    {
        ogQuat = transform.localRotation;
        yRotation = transform.eulerAngles.y;
        zRotation = transform.eulerAngles.z;
        respawnPosition = transform.position;
        
        // Set the keys used for movment detection.
        xAxisInput = "Mouse X";
        yAxisInput = "Mouse Y";
        forwardKey = KeyCode.W;
        backwardKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        jumpKey = KeyCode.Space;
        respawnKey = KeyCode.R;
    }

    Vector3 ScaleBackVector(Vector3 vector, float max_magnitude)
    {
        float magnitude = vector.magnitude;
        if(magnitude > max_magnitude)
        {
            float scale = max_magnitude / magnitude;
            vector *= scale;
        }
        return vector;
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
        velocity -= Time.smoothDeltaTime * groundAcceleration * velocity_unit;

        // If we happen to pass over the zero vector, that means the player
        // should stop moving.
        IntVector3 new_sign_vector = IntVector3.Sign(velocity);
        if(og_sign_vector != new_sign_vector)
        {
            velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
        return velocity;
    }

    bool ApplyMovementKeys(out Vector3 d_velocity, 
                           Vector3 forward, 
                           Vector3 left, 
                           float acceleration)
    {
        bool input = false;
        d_velocity = new Vector3(0.0f, 0.0f, 0.0f);
        if(Input.GetKey(forwardKey))
        {
            d_velocity += Time.smoothDeltaTime * acceleration * forward;
            input = true;
        }
        if(Input.GetKey(backwardKey))
        {
            d_velocity -= Time.smoothDeltaTime * acceleration * forward;
            input = true;
        }
        if(Input.GetKey(leftKey))
        {
            d_velocity += Time.smoothDeltaTime * acceleration * left;
            input = true;
        }
        if(Input.GetKey(rightKey))
        {
            d_velocity -= Time.smoothDeltaTime * acceleration * left;
            input = true;
        }
        return input;
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

        Vector3 d_velocity;
        bool input = ApplyMovementKeys(out d_velocity, 
                                       forward, 
                                       left, 
                                       groundAcceleration);

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
        velocity = ScaleBackVector(velocity, maxGroundSpeed);

        return velocity;
    }

    Vector3 AirMovement(Vector3 velocity, Vector3 forward, Vector3 left)
    {
        Vector3 d_velocity;
        ApplyMovementKeys(out d_velocity, forward, left, airAcceleration);
        d_velocity -= Time.smoothDeltaTime * gravityAcceleration * Vector3.down;
        
        velocity += d_velocity;
        return velocity;
    }

    bool GetGround(out RaycastHit hit_information)
    {
        // Create the ray pointing down from the player's position.
        Ray ground_ray = new Ray();
        Vector3 ray_direction = new Vector3(0.0f, -1.0f, 0.0f);
        ground_ray.origin = transform.position;
        ground_ray.direction = ray_direction;

        // Test to see if the player is standing on the object.
        bool hit = Physics.Raycast(ground_ray, out hit_information);
        if(hit && hit_information.distance <= height)
        {
            return true;
        }
        return false;
    }

    void CheckRespawn()
    {
        // Reset the playes position if they hit the respawn key.
        if(Input.GetKeyDown(respawnKey))
        {
            transform.position = respawnPosition;
            rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }


    void Update () 
    {
        // This function is for testing and should be removed for the actual
        // game.
        CheckRespawn();

        // Apply the rotation around the y axis with the x axis mouse input.
        float x_input = Input.GetAxis(xAxisInput);
        float d_y_rotation = Time.unscaledDeltaTime * x_input * xSensitivity * 
                             360.0f;
        yRotation += d_y_rotation;
        yRotation %= 360.0f;

        // Do the same for the rotation around the z axis.
        float y_input = Input.GetAxis(yAxisInput);
        float d_z_rotation = Time.unscaledDeltaTime * y_input * zSensitivity * 
                             360.0f;
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

        RaycastHit hit_information;
        bool grounded = GetGround(out hit_information);
        
        if(grounded)
        {
            // This should be seperated into another script.
            GameObject ground_object = hit_information.collider.gameObject;
            if(ground_object.CompareTag("Bounce"))
            {
                Vector3 bounceDirection = new Vector3(0.0f, 1.0f, 0.0f);
                velocity = bounceSpeed * bounceDirection;
                velocity = ScaleBackVector(velocity, maxBounceSpeed);
            }
            else
            {
                Vector3 new_position = hit_information.point;
                new_position.y += height;
                transform.position = new_position;
                velocity = GroundMovement(velocity,
                                          forward_heading, 
                                          left_heading);

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
                //headBob.UpdateContribution(head_bob_contribution);
            }
        }
        else
        {
            velocity = AirMovement(velocity, forward_heading, left_heading);

            // Do not perform head bobbing while flying through the air.
            //headBob.UpdateContribution(0.0f);
        }
        
        // Reset the velocity of the rigidody for the next physics updat.
        rigidbody.velocity = velocity;
    }
}


