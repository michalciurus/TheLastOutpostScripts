using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class ThrottleGate : Conditional {

    public float throttle;
    public string timerName;

    public override TaskStatus OnUpdate() {

        var val = gameObject.GetComponent<TimerUtility>().getTime(timerName);
        var timer = gameObject.GetComponent<TimerUtility>();

        if (!timer.timerExists(timerName)) {

            timer.addNewTimer(timerName);

            return TaskStatus.Success;
        }

        if(val >= throttle) {

            timer.zeroTimer(timerName);

            return TaskStatus.Success;
        }
        else {

            return TaskStatus.Failure;

        }


    }

}