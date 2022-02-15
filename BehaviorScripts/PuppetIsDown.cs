using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using System;
using Technie.VirtualConsole;

public class PuppetIsDown : Conditional {

    [Tooltip("The value to compare to")]
    public bool compareValue;

    public override TaskStatus OnUpdate()
    {
        
        if (GetComponent<AI>().puppetBehaviour.state != RootMotion.Dynamics.BehaviourPuppet.State.Puppet) {

            if (compareValue)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }

        if (compareValue)
        {
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }

    public override void OnReset() {

    }
}
