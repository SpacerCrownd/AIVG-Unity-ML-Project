using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class BalancingBallAgent : Agent
{
    [Header("Balancing Ball Agent Parameters")]
    public Platform platform;
    public float speed = 2f;
    public float spawnRadius = 2f;
    public float spawnHeigth = 1f;
    public LayerMask groundLayer;
    public float ballRadius = 0.25f;
    public float platformSize = 4f;

    private Rigidbody rbBall;
    //private bool simulationStarted = false;

    private void Start()
    {
        rbBall = GetComponentInChildren<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        //simulationStarted = false;

        // Reset platform and sphere position
        platform.ResetRotation();
        rbBall.velocity = Vector3.zero;
        rbBall.angularVelocity = Vector3.zero;
        rbBall.transform.position = new Vector3(
            Random.Range(-spawnRadius/2, spawnRadius/2),
            1f,
            Random.Range(-spawnRadius/2, spawnRadius/2)
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(platform.transform.rotation.eulerAngles.x/180);
        sensor.AddObservation(platform.transform.rotation.eulerAngles.z/180);
        sensor.AddObservation(rbBall.transform.position - gameObject.transform.position);
        sensor.AddObservation(rbBall.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract ball direction as rotation relative to forward axis for movement
        float rotation = Mathf.Clamp(actions.ContinuousActions[0], -1, 1) * 180 * Mathf.PI;
        float rotationInRadians = Mathf.Deg2Rad * rotation;

        // Convert rotation in radians to direction vector
        Vector3 localMoveDir = new Vector3(Mathf.Cos(rotationInRadians), 0, Mathf.Sin(rotationInRadians));

        // Check if agent is on the platform
        bool grounded = Physics.Raycast(rbBall.transform.position, Vector3.down, ballRadius, groundLayer);

        if (grounded)
        {
            if (localMoveDir.magnitude != 0)
            {
                // Get movement direction relative to the slope of the platform
                Vector3 moveDir = Vector3.ProjectOnPlane(localMoveDir, platform.transform.up);

                // Create velocity to apply to maintain a constant speed of 2 m/s in the chosen direction
                Vector3 newVelocity = moveDir.normalized * speed; 

                // Apply force to the ball as a velocity change to keep speed constant
                rbBall.AddForce(newVelocity, ForceMode.VelocityChange);
            }
        }

        if ((rbBall.transform.position - platform.transform.position).magnitude > 4f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }

    void FixedUpdate()
    {
        if (rbBall.velocity.magnitude != speed)
        {
            rbBall.velocity = rbBall.velocity.normalized * speed;
        }
    }
}
