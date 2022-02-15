using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

public class DynamicCollisionIgnore : MonoBehaviour
{
    
    public string layerNameToIgnore = "";
    public bool inverseLogic = false;

    public List<Collider> ignoredObjects;

    private void Awake()
    {
        ignoredObjects = new List<Collider>();
    }

    private void OnCollisionEnter(Collision other)
    {

        // Ignore all collisions other than the one specified
        if (inverseLogic)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer(layerNameToIgnore))
            {
                foreach (Collider collider in transform.gameObject.GetAllPossibleColliders())
                {
                    Physics.IgnoreCollision(other.collider, collider, true);
                }
                ignoredObjects.Add(other.collider);
            }

            return;
        }
        
        if (other.gameObject.layer == LayerMask.NameToLayer(layerNameToIgnore))
        {
            foreach (Collider collider in transform.gameObject.GetAllPossibleColliders())
            {
                Physics.IgnoreCollision(other.collider, collider, true);
            }
            ignoredObjects.Add(other.collider);
        }
    }

    private void OnDestroy()
    {
        foreach (Collider col in ignoredObjects)
        {
            foreach (Collider collider in transform.gameObject.GetAllPossibleColliders())
            {
                Physics.IgnoreCollision(col, collider, false);
            }
        }
    }
}
