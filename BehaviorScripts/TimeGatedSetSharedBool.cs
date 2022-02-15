using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class TimeGatedSetSharedBool : Action {
    public SharedBool targetValue;

    public SharedBool targetVariable;

    public float timeNeeded = 0;

    public float currentTime = 0;

    public override TaskStatus OnUpdate() {

        if(currentTime >= timeNeeded) {
            targetVariable.Value = targetValue.Value;
        }

        currentTime += Time.deltaTime;

        return TaskStatus.Success;
    }

    public override void OnReset() {
        targetValue = false;
        targetVariable = false;
    }
}
