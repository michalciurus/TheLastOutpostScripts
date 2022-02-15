using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapMovableOnDrop : MonoBehaviour
{
    private ConfigurableJointWrapper jointWrapper;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnDrop(MovableGrabbable movable)
    {
        jointWrapper = gameObject.AddComponent<ConfigurableJointWrapper>();
        
        var joint = jointWrapper.joint;
        joint.connectedBody = movable.GetComponent<Rigidbody>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.zero;
        joint.anchor = Vector3.zero;
        jointWrapper.SetRotation(Vector3.zero);
        
        movable.didStartGrabbing.AddListener(OnUnsnap);
        
        Coroutiner.instance.AddTask(this.gameObject, time =>
            {
                if (jointWrapper == null || joint == null) return true;

                jointWrapper.SetMoveSpring(Mathf.MoveTowards(joint.xDrive.positionSpring, 1100, time * 800), 10);
                jointWrapper.SetRotateSpring(Mathf.MoveTowards(joint.slerpDrive.positionSpring, 1100, time * 800), 10);
                
                return joint.xDrive.positionSpring >= 1000;
            }
        );

    }

    private void OnUnsnap(MovableGrabbable movable)
    {
        movable.didStartGrabbing.RemoveListener(OnUnsnap);
        Destroy(jointWrapper);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
