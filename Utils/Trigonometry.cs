using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{

    public static int getQuarterForAngle(this float angle)
    {
        if(angle < 91)
        {
            return 1;
        }

        if(angle < 181)
        {
            return 2;
        }

        if(angle < 271)
        {
            return 3;
        }

        return 4;
     
    }


}

public static class QuaternionExtensions {

    public static Quaternion inv(this Quaternion q) {
        return Quaternion.Inverse(q);
    }

}