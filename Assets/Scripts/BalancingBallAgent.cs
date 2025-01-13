using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class BalancingBallAgent : Agent
{
    [Header("Balancing Ball Agent Parameters")]
    public Platform platform; // reference to Platform script and gameobject
    public float speed = 2f; // agent speed in m/s
    public float spawnRadius = 2f; // agent max spawn distance from the center of the platform
    public float spawnHeigth = 1f; // agent spawn height
    public LayerMask groundLayer; // layer used to check for ground
    public float ballRadius = 0.25f; // radius of the ball
    public float platformSize = 4f; // platform side size
    public float groundOffset = 0.01f; // offset used during ground check

    private Rigidbody rbBall;
    private Rigidbody rbPlatform;
    private Vector3 globalMoveDir; // agent movement direction in world coordinates

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
        spawnPosition = spawnPosition.normalized * spawnRadius; // normalize and multiply by spawn radius to avoid exceeding maximum distance
        // account for position of platform different than scene origin
        spawnPosition.x += platform.transform.position.x;
        spawnPosition.y = spawnHeigth;
        spawnPosition.z += platform.transform.position.z;
        rbBall.MovePosition(spawnPosition);

        // Reset platform velocity and rotation
        rbPlatform.angularVelocity = Vector3.zero;
        rbPlatform.MoveRotation(Quaternion.identity);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // platform rotation around x axis normalized
        sensor.AddObservation(platform.transform.rotation.eulerAngles.x/180);
        // platform rotation around z axis normalized
        sensor.AddObservation(platform.transform.rotation.eulerAngles.z/180);
        // vector3 that represents the distance of the ball from the platform, normalized dividing by platform size
        sensor.AddObservation((rbBall.transform.position - platform.transform.position) / platformSize);
        // vector3 that represents the ball's velocity in the x,y,z axes
        sensor.AddObservation(rbBall.velocity / 2);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (platform.Started)
        {
            float xAction = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            float zAction = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

            if (xAction != 0 || zAction != 0)
            {
                globalMoveDir = new Vector3(xAction, 0, zAction);
                globalMoveDir.Normalize();
            }
        

            if ((rbBall.transform.position - platform.transform.position).magnitude < platformSize/2 + 0.5f)
            {
                SetReward(0.1f);
            }
            else
            {
                SetReward(-1f);
                platform.StopSimulation();
                EndEpisode();
            }
        }
    }

    void FixedUpdate()
    {
        // cast ray along platform's normal to check if the ball is on the platform
        bool isOnPlatform = Physics.Raycast(rbBall.position, -rbPlatform.transform.up, ballRadius + groundOffset);

        // if on the platform move in the direction decided by the agent at constant speed
        if (isOnPlatform)
        {
            if (globalMoveDir != Vector3.zero)
            {
                Vector3 localMoveDir = Vector3.ProjectOnPlane(globalMoveDir, -rbPlatform.transform.up).normalized;
                Vector3 targetVelocity = localMoveDir * speed;

                rbBall.AddForce(targetVelocity - rbBall.velocity, ForceMode.VelocityChange);
            }
        }
        // otherwise maitain contact with platform by falling with gravity
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}