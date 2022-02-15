using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxAngularVelocity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().maxAngularVelocity = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody>().angularVelocity *= 0.01f;
    }
}
