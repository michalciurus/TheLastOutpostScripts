using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class IgnoreColliders : MonoBehaviour
{
    public Collider[] collidersToIgnore;
    
    private bool donezo = false;
    private void FixedUpdate()
    {
        if (donezo) return;
        
        donezo = true;
    
        IgnoreSwitch(true);
    }
    
    private void IgnoreSwitch(bool ignoreOn)
    {
        if (collidersToIgnore != null && collidersToIgnore.Length > 0)
        {
            foreach(Collider c in collidersToIgnore)
            {
                foreach (Collider myCollider in gameObject.GetAllPossibleColliders())
                    {
                        if(myCollider.IsNullOrDestroyed() || c.IsNullOrDestroyed()) continue;
                        Physics.IgnoreCollision(myCollider, c, ignoreOn);
                    }
            }
        }
    }
    

    private void OnDestroy()
    {
        IgnoreSwitch(false);
    }
}
