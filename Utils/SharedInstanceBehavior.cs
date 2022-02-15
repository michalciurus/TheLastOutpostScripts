using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedInstanceBehavior<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T _sharedInstance;

    public static T sharedInstance
    {
        get
        {
            if (_sharedInstance == null)
            {
                var go = new GameObject();
                _sharedInstance = go.AddComponent<T>();

                DontDestroyOnLoad(go);

                return _sharedInstance;    
            }
            else
            {
                return _sharedInstance;
            }
        }
    }
    
    
}
