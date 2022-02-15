using System.Collections.Generic;
using System.Linq;
using DebugPlusNS;
using UnityEngine;

public delegate void DelayedTaskDelegate();

public class DelayedTask
{
    public DelayedTaskDelegate taskAction;
    public bool cancelled = false;
    
    private float count = 0;
    private float delay = 0;

    public DelayedTask(DelayedTaskDelegate action, float delay)
    {
        taskAction = action;
        this.delay = delay;
    }

    public void Tick(float deltaTime)
    {
        count += deltaTime;
        
        if (!cancelled && count >= delay)
        { 
            taskAction();
            cancelled = true;
        }
    }
}

public class DelayedTasker : SingletonBehavior<DelayedTasker>
{
    private List<DelayedTask> tasks = new List<DelayedTask>();

    public DelayedTask AddTask(float delay, DelayedTaskDelegate action)
    {
        var task = new DelayedTask(action, delay);
        tasks.Add(task);
        return task;
    }
    
    void Update()
    {
        List<DelayedTask> toDelete = new List<DelayedTask>();
        
        foreach (var task in tasks.ToList())
        {
            if (!task.cancelled)
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