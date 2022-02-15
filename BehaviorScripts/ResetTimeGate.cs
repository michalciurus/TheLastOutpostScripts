using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class ReseTimeGate : Action { 

    public TimeGatedSetSharedBool toReset;

    public override TaskStatus OnUpdate() {

        toReset.currentTime = 0;

        return TaskStatus.Success;
    }

}
