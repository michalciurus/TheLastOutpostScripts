using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TimerUtility : SharedInstanceBehavior<TimerUtility> {

    Dictionary<string, float> timerDictionary;

    private void Awake()
    {
        timerDictionary = new Dictionary<string, float>();
    }

    public static TimerUtility GetSharedTimer()
    {
        return sharedInstance;
    }

    private void Update() {
        
        foreach(string key in new List<string>(timerDictionary.Keys)) {
            timerDictionary[key] += Time.deltaTime;
        }

    }

    public float getTime(string name) {


        if (!timerDictionary.ContainsKey(name)) {
            return 0;
        }

        return timerDictionary[name];

    }

    public bool timerExists(string name) {

        if (!timerDictionary.ContainsKey(name)) {
            return false;
        }

        return true;

    }

    public void addNewTimer(string name) {

        timerDictionary[name] = 0;

    }

    public void zeroTimer(string name) {

        timerDictionary[name] = 0;

    }




}
