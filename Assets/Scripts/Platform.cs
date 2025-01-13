using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Platform : MonoBehaviour
{ 
    public bool Started
    {
        get => started;
    }

    private bool started = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!started && collision.transform.CompareTag("Ball"))
        {
            started = true;
        }
    }

    public void StopSimulation()
    {
        started = false;
    }
}
