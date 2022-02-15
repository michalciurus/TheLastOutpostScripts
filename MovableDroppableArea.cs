using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableDroppableArea : MonoBehaviour
{
    public GrabEvent didDropInTheArea;
    
    private AreaColliderObserver colliderObserver;
    
    void Start()
    {
        colliderObserver = gameObject.AddComponent<AreaColliderObserver>();
        colliderObserver.parentTransform = transform;
        colliderObserver.radius = 0.5f;
        colliderObserver.type = typeof(MovableGrabbable);

        colliderObserver.didEnter.AddListener(arg0 =>
            {
                arg0.GetComponent<MovableGrabbable>().didEndGrabbing.AddListener(DidDropItem);
            }
        );

        colliderObserver.didExit.AddListener(go =>
        {
            go.GetComponent<MovableGrabbable>().didEndGrabbing.RemoveListener(DidDropItem);
        });
    }

    private void OnDrawGizmos()
    {
        var color = Color.green;
        color.a = 0.05f;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    protected void DidDropItem(MovableGrabbable movable)
    {
        didDropInTheArea.Invoke(movable);
    }
}
