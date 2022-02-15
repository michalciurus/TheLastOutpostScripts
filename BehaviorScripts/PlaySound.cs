using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class PlaySound : Action {

    public string name;
    public override TaskStatus OnUpdate() {

        AudioClip clip = (AudioClip)Resources.Load("Sounds/" + name);

        //GetComponent<AudioSource>().PlayOneShot(clip);
        
        AudioSource.PlayClipAtPoint(clip, transform.position);

        return TaskStatus.Success;
    }

}


