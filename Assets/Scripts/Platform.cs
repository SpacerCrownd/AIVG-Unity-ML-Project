using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float maxRotation = 30f;
    public float interpolationRatio = 0.1f;
    public int rotationInterval = 5;

    private Quaternion targetRotation;

    void Start()
    {
        StartRotation();
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, interpolationRatio);
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
