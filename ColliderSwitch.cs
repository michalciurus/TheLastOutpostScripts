using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSwitch : MonoBehaviour
{

    public void SwitchCollider(bool on)
    {
        GetComponent<Collider>().enabled = on;
    }
}
