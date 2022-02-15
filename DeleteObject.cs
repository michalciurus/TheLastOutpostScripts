using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
