using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class BalancingBallAgent : Agent
{
    [Header("Balancing Ball Agent Parameters")]
    public GameObject platform;
    public BallCollision ball;
    public float speed = 2f;
    public float spawnRadius = 2f;
    public float spawnHeigth = 1f;
    public LayerMask groundLayer;
    public float ballRadius = 0.25f;
    public float platformSize = 4f;
    public float groundOffset = 0.2f;

    private Rigidbody rbBall;
    private Rigidbody rbPlatform;
    private bool isOnPlatform;
    private Vector3 globalMoveDir = Vector3.zero;

    private void Start()
    {
        rbBall = GetComponentInChildren<Rigidbody>();
        rbPlatform = platform.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent's selected movement direction
        globalMoveDir = Vector3.zero;

        // Reset sphere velocities, rotation and position
        rbBall.velocity = Vector3.zero;
        rbBall.angularVelocity = Vector3.zero;
        rbBall.MoveRotation(Quaternion.identity);

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
            );
        spawnPosition.Normalize();
        spawnPosition.y = spawnHeigth;
        rbBall.MovePosition(spawnPosition);

        // Reset platform velocity and rotation
        rbPlatform.angularVelocity = Vector3.zero;
        rbPlatform.MoveRotation(Quaternion.identity);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(platform.transform.rotation.eulerAngles.x/180);
        sensor.AddObservation(platform.transform.rotation.eulerAngles.z/180);
        sensor.AddObservation((rbBall.transform.position - platform.transform.position) / platformSize);
        sensor.AddObservation(rbBall.velocity / 2);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (ball.Started)
        {
            float xAction = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            float zAction = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

            if (xAction != 0 || zAction != 0)
            {
                globalMoveDir = new Vector3(xAction, 0, zAction);
                globalMoveDir.Normalize();
            }
        }

        if ((rbBall.transform.position - platform.transform.position).magnitude < 3f)
        {
            SetReward(0.1f);
        }
        else
        {
            SetReward(-1f);
            //ball.StopSimulation();
            //EndEpisode();
        }
    }

    void FixedUpdate()
    {
        RaycastHit slopeHit;
        isOnPlatform = Physics.Raycast(rbBall.position, -rbPlatform.transform.up, out slopeHit, ballRadius + groundOffset);

        // if grounded move in the direction decided by the agent at constant speed
        if (isOnPlatform)
        {
            if (globalMoveDir != Vector3.zero)
            {
                Vector3 localMoveDir = Vector3.ProjectOnPlane(globalMoveDir, slopeHit.normal).normalized;

                Vector3 gravityForce = Physics.gravity;
                Vector3 normal = slopeHit.normal;
                Vector3 gravityOnSlope = Vector3.ProjectOnPlane(gravityForce, normal);

                Vector3 targetVelocity = localMoveDir * speed;
                Vector3 requiredForce = rbBall.mass * (targetVelocity - rbBall.velocity) / Time.fixedDeltaTime; 
                Vector3 gravityCompensation = -gravityOnSlope * rbBall.mass;

                //rbBall.AddForce(requiredForce + gravityCompensation);
                rbBall.AddForce(targetVelocity - rbBall.velocity, ForceMode.VelocityChange);
            }
        }else
        {
            Debug.Log("not on platform");
        }
        Debug.Log(rbBall.velocity.magnitude);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}