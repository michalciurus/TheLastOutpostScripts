using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SliderJoint.Scripts;
using Technie.VirtualConsole;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SliderJoint))]
public class ButtonSwitch : MonoBehaviour
{

    public bool isOn = false;
    public float triggerTreshold = -0.04f;
    public UnityEvent<bool> ButtonSwitched = new UnityEvent<bool>();

    public bool toogleOn = true;

    private bool readyToTrigger = true;
    
    // Start is called before the first frame update
    void Awake()
    {
    }

    private void Start()
    {
        var col = gameObject.AddComponent<DynamicCollisionIgnore>();
        col.layerNameToIgnore = "Hand";
        col.inverseLogic = true;
    }

    // Update is called once per frame
    void Update()
    {
        var absoluteTreshold = Mathf.Abs(triggerTreshold);
        
        var currentPos = Mathf.Abs(GetComponent<SliderJoint>().CurrentPosition);


        if (toogleOn)
        {
            if (readyToTrigger && currentPos > absoluteTreshold )
            {
                isOn = !isOn;
                ButtonSwitched.Invoke(isOn);
                readyToTrigger = false;
            }

            if (currentPos < absoluteTreshold)
            {
                readyToTrigger = true;
            }
        }
        else
        {
            var prev = isOn;
            if (currentPos >= absoluteTreshold)
            {
                isOn = true;
            }
            else
            {
                isOn = false;
            }

            if (prev != isOn)
            {
                if (isOn)
                {
                    AudioSource.PlayClipAtPoint((AudioClip) Resources.Load("Sounds/button_press"), transform.position);
                }
                
                ButtonSwitched.Invoke(isOn);
            }
        }
        


        
    }
}
