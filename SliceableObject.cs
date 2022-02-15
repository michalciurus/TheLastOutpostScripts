using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.ObjectSlicer;


public class SliceableObject : BzSliceableObjectBase {
	// Start is called before the first frame update

	bool gowno = false;

	public void sliceInHalf() {

		var oldRot = transform.rotation;

		var gowno = GetComponent<MeshRenderer>().bounds.center;

		var total = oldRot.inv() * transform.TransformPoint(gowno);

		DebugPlus.DrawSphere(gowno, 0.1f).duration = 20.0f;

		var plane = (new Plane( oldRot * Quaternion.Euler(0,45,0) * Vector3.left , gowno));

		Slice(plane, 321, null);
	}

	protected override BzSliceTryData PrepareData(Plane plane) {

		// colliders that will be participating in slicing
		var colliders = gameObject.GetComponentsInChildren<Collider>();

		// return data
		return new BzSliceTryData() {
			// componentManager: this class will manage components on sliced objects
			componentManager = new StaticComponentManager(gameObject, plane, colliders),
			plane = plane,
		};
	}

	protected override void OnSliceFinished(BzSliceTryResult result) {
		var a = result.outObjectNeg.GetComponent<MovableGrabbable>();
		var b = result.outObjectPos.GetComponent<MovableGrabbable>();
		
		a.GetComponent<Rigidbody>().mass /= 2;
		b.GetComponent<Rigidbody>().mass /= 2;

		a.breakable = false;
		b.breakable = false;
	}
}
