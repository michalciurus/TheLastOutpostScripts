using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class AttackAction : Action
{

    public override TaskStatus OnUpdate() {
        return TaskStatus.Success;
    }

}
