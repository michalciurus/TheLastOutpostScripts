using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class StartTimer : Action
{
    public string timerName;

    public override TaskStatus OnUpdate() {

        gameObject.GetComponent<TimerUtility>().addNewTimer(timerName);

        return base.OnUpdate();
    }

}
