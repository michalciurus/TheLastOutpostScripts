using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySugar
{

    public static GameObject instantiatePrefab(string resource) {
        var res = Resources.Load( "Prefabs/" + resource);
        if (res == null)
        {
            Debug.Log(resource);
        }
        return GameObject.Instantiate(res) as GameObject;
    }

    public static Mesh instantiateMesh(string resource) {
        return Resources.Load<Mesh>("Models/" + resource);
    }
  

}

public static class GameobjectExtensions
{
    public static void SetLayerRecursively(this GameObject obj, int layer) {
        obj.layer = layer;
 
        foreach (Transform child in obj.transform) {
            child.gameObject.SetLayerRecursively(layer);
        }
    }
    
    public static void ReplaceLayerRecurs(this GameObject obj, int toReplace, int layer) {

        if (obj.layer == toReplace)
        {
            obj.layer = layer;
        }
 
        foreach (Transform child in obj.transform) {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    public static GameObject GetColliderBodyParent(this Collider obj)
    {

        if (obj.GetComponent<Rigidbody>() != null) return obj.gameObject;

        if (obj.transform.parent != null && obj.transform.parent.GetComponent<Rigidbody>() != null)
        {
            return obj.transform.parent.gameObject;
        }

        return null;
    }

    public static Collider[] GetAllPossibleColliders(this GameObject obj, bool recursive = false)
    {

        var mainCollider = obj.GetComponent<Collider>();

        if (mainCollider != null)
        {
            return new[] {mainCollider};
        }

        if (recursive)
        {
            return obj.GetComponentsInChildren<Collider>();
        }
        else
        {
            List<Collider> childColliders = new List<Collider>();
            foreach (Transform transform in obj.transform)
            {
                transform.gameObject.GetComponent<Collider>().AsIf<Collider>(collider =>
                {
                    childColliders.Add(collider);
                });
            }
            
            return childColliders.ToArray();

        }

    }
    
    public static void AsIf<T>(this T value, Action<T> action) where T : class
    {
        if (!value.IsNullOrDestroyed())
        {
            action(value);
        }
    }
    
    
    public static T CreateComponentIfMissing<T>(this MonoBehaviour value) where T : Component
    {
        T t = value.GetComponent<T>();

        if (t == null)
        {
            t = value.gameObject.AddComponent<T>();
        }

        return t;
    }
    
    
    
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }    
    
    public static T FindDeepChildComponent<T>(this Transform aParent, string aName)
            {
                Queue<Transform> queue = new Queue<Transform>();
                queue.Enqueue(aParent);
                while (queue.Count > 0)
                {
                    var c = queue.Dequeue();
                    if (c.name == aName)
                        return c.gameObject.GetComponent<T>();
                    foreach(Transform t in c)
                        queue.Enqueue(t);
                }
        
                return default(T);
            } 
    
    public static T FindDeepChildComponent<T>(this Transform aParent)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.gameObject.GetComponent<T>() != null)
                return c.gameObject.GetComponent<T>();
            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        
        return default(T);
    }   

}


