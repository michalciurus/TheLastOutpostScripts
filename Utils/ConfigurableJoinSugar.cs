using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class ConfigurableJointWrapper : MonoBehaviour
{
    public ConfigurableJoint joint;

    public UnityEvent<ConfigurableJointWrapper> jointDestroyed = new UnityEvent<ConfigurableJointWrapper>();
    
    private void Awake()
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();
    }

    public void SetMoveSpring(float spring, float damping)
    {
        var drive1 = joint.xDrive;
        var drive2 = joint.yDrive;
        var drive3 = joint.zDrive;

        drive1.positionSpring = drive2.positionSpring = drive3.positionSpring = spring;
        drive1.positionDamper = drive2.positionDamper = drive3.positionDamper = damping;

        joint.xDrive = drive1;
        joint.yDrive = drive2;
        joint.zDrive = drive3;
    }

    public void SetRotateSpring(float spring, float damping)
    {
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        var drive = joint.slerpDrive;

        drive.positionSpring = spring;
        drive.positionDamper = damping;

        joint.slerpDrive = drive;
    }

    public void DestroyJoint()
    {
        Destroy(joint);
    }

    private DelayedTask dropTask;
    private void FixedUpdate()
    {
        if (joint == null) return;
        if (joint.connectedBody == null) return;

        var movableAnchor = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);

        if ((movableAnchor - transform.position).magnitude > 0.3f)
        {
            if (dropTask == null)
            {
                dropTask = DelayedTasker.instance.AddTask(0.2f, () =>
                {
                    Destroy(joint);
                    jointDestroyed.Invoke(this);
                });
            }
        }
        else
        {
            if (dropTask != null)
            {
                dropTask.cancelled = true;
            }
        }
        
    }

    public void SetRotation(Vector3 snapRotation)
    {
        var targetRotation = transform.rotation;

        var a = targetRotation;
        var b = joint.connectedBody.transform.rotation;

        var rotDif = b.inv() * a;
        
        joint.targetRotation = Quaternion.Inverse(Quaternion.Euler(snapRotation.x, snapRotation.y, snapRotation.z)) * rotDif;
    }

    private void OnDestroy()
    {
        Destroy(joint);
    }
}
