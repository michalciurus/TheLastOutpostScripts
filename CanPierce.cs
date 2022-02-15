using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SliderJoint.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class PierceAction : MonoBehaviour
{
    public SliderJoint joint;
    
    public UnityEvent<bool> changedPierceState = new UnityEvent<bool>();

    public EnemyDamageable currentEnemyDamageable;

    public Vector3 pierceOrientation = Vector3.right;
    
    public void Pierce(Collision other)
    {
        if (joint != null) return;

        currentEnemyDamageable = other.gameObject.GetComponent<EnemyDamageable>();
        
        joint = gameObject.AddComponent<SliderJoint>();
           
        joint.ConnectedBody = other.gameObject.GetComponent<Rigidbody>();
        var lim = joint.Limits;

        joint.UseLimits = true;
           
        lim.Lower = -0.1f;
        lim.Upper = 0.3f;

        joint.Limits = lim;

        joint.Axis = pierceOrientation;

        joint.Anchor = pierceOrientation * -0.11f;
        
        var ignore = gameObject.AddComponent<DynamicCollisionIgnore>();
        ignore.layerNameToIgnore = "EnemyBodyPart";
        
        DelayedTasker.instance.AddTask(0.5f, () =>
        {
            //joint.BreakForce = 400;
        });
    }

    private void Update()
    {
        
        if (joint == null)
        {
            gameObject.layer = LayerMask.NameToLayer("GrabbableDynamicObject");
            currentEnemyDamageable = null;
            Destroy(GetComponent<DynamicCollisionIgnore>());
        }
        else
        {
            if (joint.CurrentPosition < -0.08f)
            {
                joint.BreakForce = 200;
            }
            else
            {
                joint.BreakForce = int.MaxValue;
            }
        }

    }
}
