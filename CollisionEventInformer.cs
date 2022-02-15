using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class CollisionEventInformer : MonoBehaviour, CollisionBroadcasterListener
{
    private FixedSizedQueue<Vector3> lastVelocities = new FixedSizedQueue<Vector3>();
    public float meanVectorTreshold = 1.5f;
    public bool onlyStatic = false;
    public string[] meshFilters;
    public bool inverseMeshFilter = false;
    public float maximumAngle = 0;
    public Vector3 objectOrientationGateVector = Vector3.zero;
    // Wait for ExclusiveCollisionBroadcaster to call the collide method
    public bool useExclusiveCollision = false;
    public float randomGate = 1.0f;
    public string tagFilter = null;

    public UnityEvent<Collision> strongCollision = new UnityEvent<Collision>();
    
    public int priority
    {
        get
        {
            return 0;
        }
    }
    
    void Start()
    {
        lastVelocities.Limit = 5;
        
        if (useExclusiveCollision)
        {
            var broadcaster = GetComponent<ExclusiveCollisionBroadcaster>();
            if (broadcaster == null)
            {
                broadcaster = gameObject.AddComponent<ExclusiveCollisionBroadcaster>();
            }
            
            //TODO: IMPLEMENT PRIORITY
            broadcaster.listeners.Insert(0, this);
        }
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

    private void OnCollisionEnter(Collision other)
    {
        if (!useExclusiveCollision)
        {
            handleCollision(other);
        }
    }

    private bool handleCollision(Collision other)
    {
        if (onlyStatic)
        {
            if (other.gameObject.GetComponent<Rigidbody>() != null)
            {
                return false;
            }
        }

        var rand = Random.Range(0.0f, 1.0f);

        if (rand > randomGate) return false;

        if (tagFilter != null)
        {
            if (!other.gameObject.hasTag("Piercable")) return false;
        }

        if (meshFilters != null && meshFilters.Length > 0)
        {

            bool found = false;
            foreach(var mesh in meshFilters)
            {
                foreach (var point in other.contacts)
                {
                    var meshCollider = point.thisCollider as MeshCollider;
                    if (meshCollider != null && meshCollider.sharedMesh.name == mesh)
                    {
                        found = true;
                    }
                }
            }

            if (inverseMeshFilter)
            {
                if (found) return false;
            }
            else
            {
                if (!found) return false;
            }
        }
        
        if (meanVectorTreshold != 0 && GetMeanVector().magnitude < meanVectorTreshold)
        {
            return false;
        }
        
        var angle = Vector3.Angle(GetMeanVector(), -other.contacts[0].normal);

        if (maximumAngle != 0 && angle >= maximumAngle)
        {
            return false;
        }

        if (objectOrientationGateVector != Vector3.zero)
        {
            var orientationAngle = Vector3.Angle( transform.TransformDirection(objectOrientationGateVector), -other.contacts[0].normal );
            
            if (orientationAngle >= 45)
            {
                return false;
            }
        }
        
        strongCollision.Invoke(other);
        
        return true;
    }

    public bool DidCollide(Collision other)
    {
        return handleCollision(other);
    }

    private void OnDestroy()
    {
        
    }
}
