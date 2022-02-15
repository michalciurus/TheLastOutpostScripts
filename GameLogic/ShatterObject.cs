using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShatterObject : MonoBehaviour
{
    public string shatterObjectName = "";

    private bool alreadyShattered = false;

    public UnityEvent didGetShattered = new UnityEvent();

    public void Shatter()
    {
        if (alreadyShattered) return;
        
        didGetShattered.Invoke();

        alreadyShattered = true;
        
        var shatteredObject = Resources.Load("Prefabs/" + shatterObjectName) as GameObject;

        var insta = Instantiate(shatteredObject);

        insta.transform.position = transform.position;
        insta.transform.rotation = transform.rotation;
        
        foreach(Transform child in insta.transform)
        {
           child.GetComponent<Rigidbody>().velocity = GetComponent<VelocityQueue>().GetMeanVector() * 2f;
           child.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        
        Destroy(this.gameObject);
    }
}
