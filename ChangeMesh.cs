using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMesh : MonoBehaviour
{

    public string meshName;

    // Update is called once per frame
    public void Change()
    {
        var model = Resources.Load<Mesh>("Models/" + meshName);
        GetComponent<MeshFilter>().sharedMesh = model;
    }
}
