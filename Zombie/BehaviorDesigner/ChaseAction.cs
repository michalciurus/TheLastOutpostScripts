using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class ChaseAction : Action
{
    public override void OnStart()
    {
        gameObject.GetComponent<AI>().manualRotation = true;
        var agent = gameObject.GetComponent<NavMeshAgent>();

        agent.isStopped = false;
    }

    public override TaskStatus OnUpdate() {

        GameObject player = GameObject.Find("MainPlayer");
        GameObject NPC = gameObject;
        NavMeshAgent agent = NPC.GetComponent<NavMeshAgent>();

        agent.stoppingDistance = C_WEAK_ZOMBIE.CHASE_STOPPING_DISTANCE;
        
        agent.SetDestination(player.transform.position);

        gameObject.GetComponent<AI>().speedMultiplier = 1f;

        gameObject.GetComponent<AI>().lastKnownPlayerPosition = player.transform.position;
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        var agent = gameObject.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        gameObject.GetComponent<AI>().manualRotation = false;
    }
}


