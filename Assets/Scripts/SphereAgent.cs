using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class SphereAgent : Agent
{
    [Header("Sphere Agent Parameters")]
    public Platform platform;
    public float speed = 2f;
    public int spawnRadius = 2;

    private Rigidbody platformRigidbody;
    private Rigidbody sphereRigidbody;

    private void Start()
    {
        platformRigidbody = platform.GetComponent<Rigidbody>();
        sphereRigidbody = this.GetComponentInChildren<Rigidbody>();
    }

    // This method is run when a sequence begins. i.e., we start trying to balance the ball
    public override void OnEpisodeBegin()
    {
        // set random rotation for platform
        platform.StartRotation();
        sphereRigidbody.position = sphereRigidbody.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(sphereRigidbody.transform.forward); // vector3, sphere direction
        sensor.AddObservation(sphereRigidbody.velocity); // vector3
        sensor.AddObservation(platform.transform.rotation.x); // float
        sensor.AddObservation(platform.transform.rotation.z); // float
        sensor.AddObservation(platform.transform.position - this.gameObject.transform.position); // vector3
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Vector3 movementDirection = actionBuffers.ContinuousActions[0];

        if ((platform.transform.position - gameObject.transform.position).magnitude < 4f)
        {
            // it the ball is still on the platform, we are happy, give a reward, and go on
            SetReward(0.1f);
        }
        else
        {
            SetReward(-1f);
            platform.StopRotation();
            EndEpisode();
        }
    }
}
