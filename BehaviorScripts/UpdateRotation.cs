using System.Collections;
using System.Collections.Generic;

using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class UpdateRotation : Action {

    public bool updateRotation = true;
    public override TaskStatus OnUpdate() {

        GetComponent<NavMeshAgent>().updateRotation = updateRotation;

        return TaskStatus.Success;
    }

}


