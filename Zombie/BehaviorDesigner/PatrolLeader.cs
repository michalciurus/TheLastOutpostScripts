using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PatrolLeader : Conditional
{
	public override TaskStatus OnUpdate()
	{
		return GetComponent<AI>().isAFlockLeader ? TaskStatus.Success : TaskStatus.Failure ;
	}
}