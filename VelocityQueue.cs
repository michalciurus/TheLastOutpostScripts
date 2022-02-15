using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityQueue : MonoBehaviour
{
    private FixedSizedQueue<Vector3> lastVelocities = new FixedSizedQueue<Vector3>();
    public int queueSize = 5;

    private void Start()
    {
        lastVelocities.Limit = queueSize;
    }
    
    private void FixedUpdate()
    {
        var rigidbody = GetComponent<Rigidbody>();
        lastVelocities.Enqueue(rigidbody.velocity);
    }

    public Vector3 GetMeanVector()
    { 
        return lastVelocities.q.ToArray().GetMeanVector();
    }

}
