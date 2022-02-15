using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10000)]
[System.Serializable]
public class MyEvent : UnityEvent { }

public class BehaviorForwarder : MonoBehaviour {

    static public MyEvent OnUpdate = new MyEvent();

    void Update()
    {
        OnUpdate?.Invoke();
    }

}
