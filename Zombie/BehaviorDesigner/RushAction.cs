using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class RushAction : Action
{
    private Vector3 direction;
    
    public override void OnStart()
    {
        GameObject player = GameObject.Find("MainPlayer");

        direction = player.transform.position - transform.position;

        gameObject.GetComponent<AI>().manualRotation = false;
        var agent = gameObject.GetComponent<NavMeshAgent>();

        agent.isStopped = true;
    }

    public override TaskStatus OnUpdate()
    {
        var k = 4;
        GetComponent<Rigidbody>().velocity = new Vector3(direction.normalized.x * k,
            GetComponent<Rigidbody>().velocity.y, direction.normalized.z * k);

        return TaskStatus.Running;
    }

}


