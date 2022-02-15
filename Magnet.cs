using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{

    AreaColliderObserver observer;

    // Start is called before the first frame update
    void Start()
    {
        //observer = new AreaColliderObserver("Magnetable", transform, 0.5f);
    }

    private void FixedUpdate()
    {

        return;
        
        foreach(Collider coll in observer.collidersArray) {

            var rigid = coll.gameObject.GetComponent<Rigidbody>();

            if(rigid != null && coll.gameObject != gameObject) {

                var k = transform.position - rigid.transform.position;

                rigid.velocity = k * 10;
                
            }

        }

    }
}
