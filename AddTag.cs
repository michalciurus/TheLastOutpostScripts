using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTag: MonoBehaviour {

    public string tagToAdd;

    private void Start() {
        this.addTag(tagToAdd);
    }

}