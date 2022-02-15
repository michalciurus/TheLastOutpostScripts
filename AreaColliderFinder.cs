using System;
using System.Linq;
using Lightbug.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class AreaColliderEvent: UnityEvent<GameObject> { }

public class AreaColliderObserver: MonoBehaviour
{
    public Transform parentTransform;
    public float radius = 0.5f;
    public string tagFilter = null;
    public Type type = null;
    public Vector3 box = Vector3.zero;
    
    public AreaColliderEvent didEnter = new AreaColliderEvent();
    public AreaColliderEvent didExit = new AreaColliderEvent();

    public Collider[] collidersArray = new Collider[100];
    
    public void FixedUpdate()
    {
        if (parentTransform == null)
        {
            Debug.LogError("MISSING PARENT TRANSFORM");
            return;
        }
        
        var position = parentTransform.position;

        var oldArray = collidersArray.ToList();
        
        Array.Clear(collidersArray, 0, collidersArray.Length);

        var size = 0;

        if (box != Vector3.zero)
        {
            size =
                Physics.OverlapBoxNonAlloc(parentTransform.position, box / 2.0f, collidersArray, Quaternion.identity, Physics.AllLayers,
                    QueryTriggerInteraction.Collide);
        }
        else
        {
            size = Physics.OverlapSphereNonAlloc(position, radius, collidersArray ,Physics.AllLayers, QueryTriggerInteraction.Collide);
        }

        foreach(Collider collider in collidersArray)
        {
            if (collider == null) break;
            
            if (!tagFilter.IsNullOrEmpty() && !collider.gameObject.hasTag(tagFilter)) continue;
            
            if (oldArray.Exists(element => element == collider)) continue;
            
            if(type != null && collider.gameObject.GetComponent(type) == null) continue;
            
            didEnter.Invoke(collider.gameObject);
        }

        foreach (Collider oldCollider in oldArray)
        {
            if (oldCollider == null)
            {
                break;
            };
            
            //Not sure of that one, should we notify? Prolly yeah!
            if (!oldCollider.gameObject.activeSelf)
            {
                //Setting active to simulate leaving
                oldCollider.gameObject.SetActive(true);
                didExit.Invoke(oldCollider.gameObject);
                oldCollider.gameObject.SetActive(false);
                continue;
            }
            
            if ( oldCollider.IsNullOrDestroyed())
            {
                if(!oldCollider.gameObject.IsNullOrDestroyed()) didExit.Invoke(oldCollider.gameObject);
                continue;
            }

            if(!Array.Exists(collidersArray, element => element == oldCollider))
            {

                if (!tagFilter.IsNullOrEmpty() && !oldCollider.gameObject.hasTag(tagFilter)) continue;
                
                if(type != null && oldCollider.gameObject.GetComponent(type) == null) continue;
                
                if (oldCollider.gameObject != null)
                {
                    didExit.Invoke(oldCollider.gameObject);
                }
            }
        }
    }
    
}
