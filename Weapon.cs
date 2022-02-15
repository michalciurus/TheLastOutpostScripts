using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    bool canShoot = true;

    void Update()
    {

        return;

        if ( !XRSugar.Right().HasValue ) {
            return;
        }

        var a = XRSugar.Trigger(XRSugar.Right().Value);

        if ( a > 0.1 ) {

            if (!canShoot) {
                return;
            }

            canShoot = false;

            var proj = UnitySugar.instantiatePrefab("Projectile");

            proj.transform.parent = this.transform;

            proj.transform.localPosition = new Vector3(0, 0, 0.7f);

            proj.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 150;

            GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * 150, new Vector3(0, 0, 0.5f), ForceMode.Impulse);
        }
        else {

            canShoot = true;

        }
    }
}
