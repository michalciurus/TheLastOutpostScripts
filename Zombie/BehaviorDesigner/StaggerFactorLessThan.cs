using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;

public class StaggerFactorLessThan : Conditional {

    public float staggerLessThan = 0;

    public override TaskStatus OnUpdate() {

        var ai = gameObject.GetComponent<AI>().staggerFactor;

        return ai <= staggerLessThan ? TaskStatus.Success : TaskStatus.Failure;

    }

}
