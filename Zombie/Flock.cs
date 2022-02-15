using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


class SortClosest : IComparer<AI> {

    Vector3 target;

    public SortClosest(Vector3 target) {
        this.target = target;
    }

    public int Compare(AI x, AI y) {

        var aMag = Mathf.Abs((x.transform.position - target).magnitude);
        var bMag = Mathf.Abs((y.transform.position - target).magnitude);
        return aMag.CompareTo(bMag);
    }
}