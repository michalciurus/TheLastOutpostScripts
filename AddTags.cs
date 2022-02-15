using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTags : MonoBehaviour
{

    public string[] tagsToAdd;

    // Start is called before the first frame update
    void Start()
    {
        foreach(string t in tagsToAdd) {
            this.addTag(t);
        }

    }

}
