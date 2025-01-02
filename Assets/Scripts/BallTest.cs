using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    public float speed = 2f;

    private float directionEuler = 0f; // direction relative to x in angles
    private Vector3 direction = Vector3.zero;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            float angleInRadians = 0f * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
            Debug.Log(direction);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            float angleInRadians = 90f * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
            Debug.Log(direction);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            float angleInRadians = 180f * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
            Debug.Log(direction);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            float angleInRadians = 270f * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
            Debug.Log(direction);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(direction.normalized * speed, ForceMode.VelocityChange);
        Vector3 velocity = rb.velocity;
        if (velocity.magnitude != speed)
        {
            rb.velocity = velocity.normalized * speed;
            Debug.Log(rb.velocity);
        }
    }
}
