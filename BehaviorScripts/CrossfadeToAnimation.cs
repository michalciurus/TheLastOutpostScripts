using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using System;
using Technie.VirtualConsole;

public class CrossfadeToAnimation : BehaviorDesigner.Runtime.Tasks.Action {

    public string animationName;
    public float time;
    

    public override void OnStart() {

        var en = (AIState) Enum.Parse(typeof(AIState), animationName);

        GetComponent<AI>().ChangeToState(en, false);

        base.OnStart();
    }

}

public class CrossfadeOneOff : BehaviorDesigner.Runtime.Tasks.Action
{
    public string animationName;
    public float numberOfTimes = 1.0f;
    public float blend = 1.0f;
    public bool forceUpdate = false;

    private float startNormalizedTime = 0.0f;

    public override void OnStart()
    {
        var en = (AIState) Enum.Parse(typeof(AIState), animationName);
        GetComponent<AI>().ChangeToState(en, true, blend);
    }

    public override TaskStatus OnUpdate()
    {
        var deltaNormalTime = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (!gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            return TaskStatus.Running;
        }
        
        if (deltaNormalTime > numberOfTimes && !gameObject.GetComponent<Animator>().IsInTransition(0))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
