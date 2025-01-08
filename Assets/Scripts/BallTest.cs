using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallTest : MonoBehaviour
{
    public float speed = 2f;
    public GameObject platform;
    public LayerMask groundLayer;
    public float ballRadius = 0.2f;

    private Vector3 localMoveDir = Vector3.zero;
    private Rigidbody rb;
    private bool grounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            float angleInRadians = 0f * Mathf.Deg2Rad;
            localMoveDir = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            float angleInRadians = 90f * Mathf.Deg2Rad;
            localMoveDir = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            float angleInRadians = 180f * Mathf.Deg2Rad;
            localMoveDir = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            float angleInRadians = 270f * Mathf.Deg2Rad;
            localMoveDir = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
        }
    }

    void FixedUpdate()
    {
        grounded = Physics.Raycast(rb.transform.position, Vector3.down, ballRadius, groundLayer);
        if (grounded)
        {
            if (localMoveDir.magnitude != 0)
            {
                Vector3 moveDir = Vector3.ProjectOnPlane(localMoveDir, platform.transform.up);
                Vector3 desiredVelocity = moveDir.normalized * speed;

                rb.AddForce(desiredVelocity - rb.velocity, ForceMode.VelocityChange);
                //rb.MovePosition(rb.position + desiredVelocity * Time.deltaTime);
                //rb.velocity = desiredVelocity;
            }
        }

        Debug.Log(rb.velocity.magnitude);
    }
}
