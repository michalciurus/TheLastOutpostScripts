using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NeckBiteListener : MonoBehaviour
{
    private AreaColliderObserver observer;
    public UnityEvent didBite = new UnityEvent();

    private void Awake()
    {
        observer = gameObject.AddComponent<AreaColliderObserver>();
        observer.parentTransform = transform;
        observer.radius = 0.4f;
        observer.type = typeof(MainPlayer);

        observer.didEnter.AddListener(arg0 =>
        {
            didBite.Invoke();
        });
    }

    private void OnDestroy()
    {
        Destroy(observer);
    }
}
