using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
public class RigidbodyMoveAction : Action
{

    public bool isChasing = false;

    public override TaskStatus OnUpdate() {

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        var staggerFactor = 1.0f;

        var rigidBody = GetComponent<Rigidbody>();

        var speed = 1.2f;

        if (isChasing) {
            staggerFactor = 2.4f;
        }



        var newVelocity = new Vector3(agent.velocity.normalized.x * staggerFactor, rigidBody.velocity.y, agent.velocity.normalized.z * staggerFactor);

        var k = Vector3.Slerp(rigidBody.velocity, newVelocity, Time.deltaTime * 20);

        rigidBody.velocity = k;
        rigidBody.isKinematic = false;

        return TaskStatus.Success;
    }

}
