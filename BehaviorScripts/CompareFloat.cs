using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using System;
using BehaviorDesigner.Runtime;

public class CompareFloat : Conditional {


    [Tooltip("The name of the field")]
    public SharedString fieldName;
    [Tooltip("The value to compare to")]
    public float compareValue;
    [Tooltip("expected result")]
    public int expectedResult;

    public override TaskStatus OnUpdate() {

        var field = gameObject.GetComponent<AI>().GetType().GetField(fieldName.Value);
        var fieldValue = field.GetValue(gameObject.GetComponent<AI>()) as float?;

        return fieldValue?.CompareTo( compareValue ) == expectedResult ? TaskStatus.Success : TaskStatus.Failure;
    }

}
