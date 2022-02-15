using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class CompareTime : Conditional {

    public float valueToCompare;
    public string timerName;

    public bool greaterThan = false;

    public override TaskStatus OnUpdate() {

        var val = gameObject.GetComponent<TimerUtility>().getTime(timerName);

        if (greaterThan) {
            return val >= valueToCompare ? TaskStatus.Success : TaskStatus.Failure;
        }
        else {
            return val <= valueToCompare ? TaskStatus.Success : TaskStatus.Failure;
        }

    }

}