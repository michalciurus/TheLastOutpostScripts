using System;
using System.Collections;
using System.Collections.Generic;
using DebugPlusNS;
using UnityEngine;

public delegate bool TaskDelegate(float deltaTime);

public class Task
{
    public TaskDelegate taskAction;
    public bool finished = false;
    public GameObject owner;

    public Task(TaskDelegate action, GameObject owner)
    {
        this.owner = owner;
        taskAction = action;
    }

    public void Tick(float deltaTime)
    {
        if (!finished)
        {
            finished = taskAction(deltaTime);
        }
    }
}

public class Coroutiner : SingletonBehavior<Coroutiner>
{
    private List<Task> tasks = new List<Task>();

    public Task AddTask(GameObject owner, TaskDelegate action)
    {
        var task = new Task(action, owner);
        tasks.Add(task);
        return task;
    }
    
    void Update()
    {
        List<Task> toDelete = new List<Task>();
        
        foreach (var task in tasks)
        {
            if (task.owner.IsNullOrDestroyed()) continue;

            if (!task.finished)
            {
                task.Tick(Time.deltaTime);
            }
            else
            {
                toDelete.Add(task);
            }
        }

        foreach (var del in toDelete)
        {
            tasks.Remove(del);
        }
    }
}