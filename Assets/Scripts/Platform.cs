using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float maxRotation = 30f;
    public float interpolationRatio = 1f;
    public int rotationInterval = 5;

    private Quaternion targetRotation;
    private bool started = false;

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, interpolationRatio * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ball") && !started)
        {
            started = true;
            StartRotation();
        }
    }

    public void RotateRandom()
    {
        targetRotation = Quaternion.Euler(Random.Range(-maxRotation, maxRotation), 0, Random.Range(-maxRotation, maxRotation));
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    public void StartRotation()
    {
        StartCoroutine(SetRandomTargetPosition());
    }

    public void StopRotation()
    {
        started = false;
        StopCoroutine(SetRandomTargetPosition());
    }

    private IEnumerator SetRandomTargetPosition()
    {
        while (true)
        {
            RotateRandom();
            yield return new WaitForSeconds(rotationInterval);
        }
    }
}
