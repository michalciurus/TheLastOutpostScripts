using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class ResetTimer : Action {
    public string timerName;

    public override TaskStatus OnUpdate() {

        gameObject.GetComponent<TimerUtility>().zeroTimer(timerName);

        return base.OnUpdate();
    }

}
