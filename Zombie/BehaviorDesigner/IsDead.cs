using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class IsDead : Conditional
{
    public override TaskStatus OnUpdate() {
        
        return gameObject.GetComponent<AI>().isDead ? TaskStatus.Success : TaskStatus.Failure;

    }

}
