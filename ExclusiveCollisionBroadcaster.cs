using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface CollisionBroadcasterListener
{
     int priority { get;  }
     bool DidCollide(Collision other);
}

public class ExclusiveCollisionBroadcaster : MonoBehaviour
{

    public List<CollisionBroadcasterListener> listeners = new List<CollisionBroadcasterListener>();

    private void OnCollisionEnter(Collision other)
    {
        listeners = listeners.OrderBy(p => p.priority).ToList();
        foreach (CollisionBroadcasterListener listener in listeners)
        {
            if (listener.DidCollide(other)) return;
        }
    }

    private void Update()
    {
        List<CollisionBroadcasterListener> toremove = new List<CollisionBroadcasterListener>();
        foreach (CollisionBroadcasterListener listener in listeners)
        {
            if (listener.IsNullOrDestroyed())
            {
                toremove.Add(listener);
            }
        }

        foreach (CollisionBroadcasterListener listener in toremove)
        {
            listeners.Remove(listener);
        }
    }
}

public static class ObjectUtility
{
    public static bool IsNullOrDestroyed(this object value)
    {
        return ReferenceEquals(value, null) || value.Equals(null);
    }
}