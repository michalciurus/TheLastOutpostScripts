using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JointSugar 
{
    public static void SetMoveSpring(this ConfigurableJoint joint, float spring, float damping)
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
    
    public static void SetRotateSpring(this ConfigurableJoint joint, float spring, float damping)
    {
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        var drive = joint.slerpDrive;

        drive.positionSpring = spring;
        drive.positionDamper = damping;

        joint.slerpDrive = drive;
    }
    
    public static void SetRotateSpringYZ(this ConfigurableJoint joint, float spring, float damping)
    {
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;

        var drive = joint.angularYZDrive;
        
        drive.positionSpring = spring;
        drive.positionDamper = damping;

        joint.angularYZDrive = drive;
    }
}
