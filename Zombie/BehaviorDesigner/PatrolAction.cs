using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class PatrolAction : Action
{
    private bool reachedPatrolPoint = false;

    public bool goToLastKnownPosition = false;

    public override void OnStart()
    {
        var agent = gameObject.GetComponent<NavMeshAgent>();
        var point = Vector3.zero;

        if (goToLastKnownPosition) {
            point = gameObject.GetComponent<AI>().lastKnownPlayerPosition;
        }
        else
        {
            point = gameObject.GetComponent<AI>().getCurrentPatrolPoint();
        }

        gameObject.GetComponent<AI>().speedMultiplier = goToLastKnownPosition ? 1.4f : 1.0f;        

        agent.SetDestination(point);
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.stoppingDistance = C_WEAK_ZOMBIE.CHASE_STOPPING_DISTANCE;
    }

    public override TaskStatus OnUpdate() {

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (!agent.pathPending)
        {
            reachedPatrolPoint = agent.remainingDistance <= agent.stoppingDistance;
        }

        if (reachedPatrolPoint)
        {
            gameObject.GetComponent<AI>().incrementPatrolPoint();
        }

        return reachedPatrolPoint ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnEnd() {
        reachedPatrolPoint = false;
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        base.OnEnd();
    }
}

public class FollowAction : Action {

    public override TaskStatus OnUpdate() {

        var point = gameObject.GetComponent<AI>().followTarget;
        
        var agent = gameObject.GetComponent<NavMeshAgent>();

        agent.isStopped = false;
        
        agent.stoppingDistance = C_WEAK_ZOMBIE.FOLLOW_STOPPING_DISTANCE;
        
        gameObject.GetComponent<NavMeshAgent>().SetDestination(point.position);

        //var reachedPatrolPoint = agent.remainingDistance <= agent.stoppingDistance;

        return TaskStatus.Running;
    }

}
