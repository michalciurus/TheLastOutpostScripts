using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody2D;
using UnityEngine;
using UnityEngine.AI;

public class HoverCart : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rigidbody;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        
        agent.updatePosition = false;
        agent.updateRotation = true;
        agent.updateUpAxis = false;

        agent.stoppingDistance = 1f;

        var confi = GetComponents<ConfigurableJoint>();

        foreach (ConfigurableJoint joint in confi)
        {
            
        }
        
    }

    private void Update()
    {
        return;
        
        agent.nextPosition = transform.position;

        var player = GameObject.Find("MainPlayer");
        
        agent.SetDestination(player.transform.position);
        
        var newVelocity = new Vector3(agent.velocity.normalized.x * 1.3f, rigidbody.velocity.y, agent.velocity.normalized.z * 1.3f);

        var k = Vector3.Slerp(rigidbody.velocity, newVelocity, Time.deltaTime * 5);
        
        float rotationSpeed = 10f;
        
        if (agent.remainingDistance <= agent.stoppingDistance) {
            rigidbody.velocity = Vector3.zero;
        } else {
            rigidbody.velocity = k;
        }
    }
}
