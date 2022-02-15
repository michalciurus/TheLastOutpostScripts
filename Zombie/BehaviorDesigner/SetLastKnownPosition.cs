using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class SetLastKnownPosition : Action {
    public override TaskStatus OnUpdate() {

        GameObject player = GameObject.Find("MainPlayer");

        gameObject.GetComponent<AI>().lastKnownPlayerPosition = player.transform.position;

        return TaskStatus.Success;
    }

}
