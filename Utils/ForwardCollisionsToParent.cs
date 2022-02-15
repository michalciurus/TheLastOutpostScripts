using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyIntEvent : UnityEvent<GameObject, Collision> {
}

[System.Serializable]
public class ForwardCollisionsToParent : MonoBehaviour {

    public MyIntEvent OnChildCollideEnter = new MyIntEvent();
    public MyIntEvent onChildCollideExit = new MyIntEvent();

    private void OnCollisionEnter(Collision collision) {
        OnChildCollideEnter?.Invoke(this.gameObject, collision);
    }

    private void OnCollisionExit(Collision collision) {
        onChildCollideExit?.Invoke(this.gameObject, collision);
    }

}
 