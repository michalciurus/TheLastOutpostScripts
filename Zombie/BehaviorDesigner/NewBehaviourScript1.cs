using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;

public class ApplyPushbackForce : Action {

    public override TaskStatus OnUpdate() {

        gameObject.GetComponent<AI>().ApplyPushbackForce();

        return TaskStatus.Success;

    }

}
