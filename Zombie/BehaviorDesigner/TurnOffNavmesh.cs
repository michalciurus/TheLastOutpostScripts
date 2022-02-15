using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class TurnOffNavmesh : Action
{
    public override TaskStatus OnUpdate()
    {
        GetComponent<NavMeshAgent>().isStopped = true;
        return TaskStatus.Success;
    }
    
}

public class TurnOnManualRotation : Action
{

    public bool on = true;

    public override TaskStatus OnUpdate()
    {
        GetComponent<AI>().manualRotation = on;
        return TaskStatus.Success;
    }
}


