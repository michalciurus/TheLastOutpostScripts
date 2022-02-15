using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slerper
{
    public Transform toSlerp;
    public Quaternion fromRotation;
    public Quaternion toRotation;

    public bool finished = false;
    
    public void updateSlerp()
    {
        finished = Quaternion.Angle(fromRotation, toRotation) < 0.5;

        if(finished) {
            return;
        }

        Debug.Log(toSlerp.rotation);
        toSlerp.rotation = Quaternion.Slerp(toSlerp.rotation, toRotation, 0.1f);
    }
}
