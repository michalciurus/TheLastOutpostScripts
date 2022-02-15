
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Numerics;
using UnityEngine.XR;
using Autohand;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[DefaultExecutionOrder(101)]
public class Grabber : MonoBehaviour
{

    public bool rightController = true;
    public Transform trackingSpace;
    public Transform cameraRig;

    protected List<MovableGrabbable> grabCandidates = new List<MovableGrabbable>();

    [HideInInspector] public MovableGrabbable grabbedObject;

    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    public MovableGrabbable distanceGrabbableObject;

    public Autohand.Hand hand;
    
    private ConfigurableJoint handMovingJoint;
    
    private AreaColliderObserver closeColliderObserver;

    private void Awake()
    {
        handMovingJoint = GetComponent<ConfigurableJoint>();

        closeColliderObserver = gameObject.AddComponent<AreaColliderObserver>();
        closeColliderObserver.parentTransform = transform;
        closeColliderObserver.radius = 0.04f;
        
        TimerUtility.GetSharedTimer().addNewTimer("DistanceGrabbableTimer");
    }
    
    private Vector3 CurrentControllerPosition() {
        var dev = rightController ? XRSugar.Right() : XRSugar.Left();

        if (dev.HasValue) {
            return XRSugar.Position(dev.Value);
        }
        else {
            return Vector3.zero;
        }
    }

    private Vector3 CurrentControllerAngularVel() {
        var dev = rightController ? XRSugar.Right() : XRSugar.Left();

        if (dev.HasValue) {
            return XRSugar.AngularVelocity(dev.Value);
        } else {
            return Vector3.zero;
        }
    }

    public Vector3 CurrentControllerVel() {
        var dev = rightController ? XRSugar.Right() : XRSugar.Left();

        if (dev.HasValue) {
            return XRSugar.Velocity(dev.Value);
        }
        else {
            return Vector3.zero;
        }
    }

    private Quaternion CurrentControllerRotation() {
        var dev = rightController ? XRSugar.Right() : XRSugar.Left();

        if (dev.HasValue) {
            return XRSugar.Rotation(dev.Value);
        }
        else {
            return Quaternion.identity;
        }
    }

    private float CurrentGrip() {
        var dev = rightController ? XRSugar.Right() : XRSugar.Left();
        if (dev.HasValue) {
            return XRSugar.Grip(dev.Value);
        }
        else {
            return 0;
        }
    }
    
    public float CurrentTrigger() {
        var dev = rightController ? XRSugar.Right() : XRSugar.Left();
        if (dev.HasValue)
        {
            return XRSugar.Trigger(dev.Value);
        }
        else {
            return 0;
        }
    }

    protected Vector3 centerStartPosition = Vector3.zero;

    float previousPlayerCapsuleHeight = 1.8f;
    
    private RaycastHit[] hits = new RaycastHit[1000];

    public void AddCandidates(AreaColliderObserver colliderObserver, bool ignoreNeedsCloseGrab)
    {
        foreach(Collider other in colliderObserver.collidersArray) {
            
            if(other == null) continue;

            var rigidBodyParent = other.GetColliderBodyParent();
            
            if(rigidBodyParent == null) continue;

            var grabbable = rigidBodyParent.GetComponent<MovableGrabbable>();

            if (grabbable == null) continue;
            
            if (ignoreNeedsCloseGrab && grabbable.needsCloseGrab) continue;

            grabCandidates.Add(grabbable);
            grabbable.setIsHovering(true);
        }
    }

    private bool lastFlexState = false;

    private void FindDistanceGrabbable()
    {
        var size = Physics.SphereCastNonAlloc(transform.position + transform.forward * 0.5f, 0.2f, transform.rotation * Vector3.forward, hits, 2.0f);

        if (hits.Length > 0) {

            MovableGrabbable movableGrab = null;

            for(int i = 0; i < size; i++)
            {
                var hit = hits[i];

                var mov = hit.collider.gameObject.GetComponent<MovableGrabbable>();
                
                if (mov == null && hit.collider.gameObject.transform.parent != null) {

                    mov = hit.collider.gameObject.transform.parent.gameObject.GetComponent<MovableGrabbable>();
                }

                if (mov != null && mov.canFlick) {
                    movableGrab = mov;
                    break;
                }

            }

            if (movableGrab != null && TimerUtility.GetSharedTimer().getTime("DistanceGrabbableTimer") > 1.0f) {

                if (movableGrab != distanceGrabbableObject) {
                    var dev = rightController ? XRSugar.Right() : XRSugar.Left();
                    dev?.SendHapticImpulse(0, 0.1f, 0.1f);

                    movableGrab.setPointedAt(true);

                    if (distanceGrabbableObject != null) {
                        distanceGrabbableObject.setPointedAt(false);
                    }

                    distanceGrabbableObject = movableGrab;
                }
            }
            else {
                if (distanceGrabbableObject != null) {
                    distanceGrabbableObject.setPointedAt(false);
                    distanceGrabbableObject = null;
                }
            }
        }
    }

    private void MoveHand()
    {
        var delta = Vector3.zero;

        if (centerStartPosition == Vector3.zero) {

            var centerEye = XRSugar.CenterEye();

            if (centerEye.HasValue) {
                centerStartPosition = XRSugar.Position(centerEye.Value);
            }

        }

        if (centerStartPosition != Vector3.zero) {
            var centerEye = XRSugar.CenterEye();
            if (centerEye.HasValue) 
                delta = centerStartPosition - XRSugar.Position(centerEye.Value);

        }

        delta.y = 0;

        var controller = CurrentControllerPosition();
        var rot = CurrentControllerRotation();

        var centerFix = centerStartPosition;

        var player = GameObject.Find("MainPlayer");

        var currentHeight = player.GetComponent<CapsuleCollider>().height;

        var heightFix = previousPlayerCapsuleHeight - currentHeight;

        var camera = cameraRig.transform.localPosition.y;

        delta.y = heightFix / 2.0f + player.GetComponent<MainPlayer>().handsHeightCorrection;

        handMovingJoint.autoConfigureConnectedAnchor = false;
        handMovingJoint.connectedAnchor = controller - centerFix + delta + new Vector3(0, C.PLAYER_HEIGHT / 2.0f, 0);
        handMovingJoint.enableCollision = false;

        //handMovingJoint.connectedMassScale = 0.0001f;
        
        var goal = trackingSpace.transform.rotation * rot;
        GetComponent<Rigidbody>().MoveRotation(goal.normalized);
    }
    
    private void FixedUpdate()
    {
        
        foreach(MovableGrabbable oldCandidate in grabCandidates) {
            oldCandidate.setIsHovering(false);
        }

        float flex = CurrentGrip();
        float trigger = CurrentTrigger();
        var didFlex =  flex >= grabBegin;

        if (!lastFlexState && didFlex)
        {
            TimerUtility.sharedInstance.addNewTimer("GrabWindow");
        }
        
        lastFlexState = didFlex;

        hand.SetGrip(flex, trigger);

        grabCandidates = new List<MovableGrabbable>();

        AddCandidates(closeColliderObserver, false);

        MoveHand();
        
        var grabWindow = TimerUtility.sharedInstance.getTime("GrabWindow");

        FindDistanceGrabbable();
        
        if (grabCandidates.Count > 0)
        {
            if (grabWindow < 0.2f && grabbedObject == null)
            {
                var highestPriority = int.MinValue;
                MovableGrabbable currentCandidate = null;

                foreach (MovableGrabbable candidate in grabCandidates) {
                    if (candidate.priority > highestPriority && !candidate.forceGrabAfterHoverHold) {
                        currentCandidate = candidate;
                        highestPriority = candidate.priority;
                    }

                    if (candidate.forceGrabAfterHoverHold && candidate.isHoveringTimer >= 0.5f) {
                        currentCandidate = candidate;
                        highestPriority = int.MaxValue;
                    }
                }

                if(currentCandidate != null) {
                    startGrabbing(currentCandidate);
                }
            }
        } else if (grabbedObject == null && distanceGrabbableObject != null) {
            if (didFlex) {
                distanceGrabbableObject.CreateComponentIfMissing<FlyTowards>().startFlying(hand.gameObject);
                distanceGrabbableObject.setPointedAt(false);
                distanceGrabbableObject = null;
                TimerUtility.GetSharedTimer().addNewTimer("DistanceGrabbableTimer");
            }
        }

        // Force grab logic 
        if(grabbedObject != null && flex >= grabBegin && !grabbedObject.forceGrabAfterHoverHold) {

            foreach (MovableGrabbable candidate in grabCandidates) {

                if (candidate.forceGrabAfterHoverHold && candidate.isHoveringTimer >= 0.5f) {
                    releaseObject();
                    startGrabbing(candidate);
                }
            }
        }  

        if (flex <= grabEnd && grabbedObject != null)
        {
            releaseObject();
        }
        
        CreateJoinIfNeeded();
        
    }

    private void CreateJoinIfNeeded()
    {
        if (grabbedObject == null) return;

        var movableGrabbable = grabbedObject.GetComponent<MovableGrabbable>();
        
        if (movableGrabbable != null)
        {
            if (holdingMovableJointWrapper == null)
            {
                holdingMovableJointWrapper = hand.gameObject.AddComponent<ConfigurableJointWrapper>();
                holdingMovableJointWrapper.joint.connectedBody = movableGrabbable.GetComponent<Rigidbody>();

                var handPoint = hand.transform.position + holdingMovableJointWrapper.joint.anchor;
                var direction = (movableGrabbable.transform.position - handPoint).normalized;
                
                holdingMovableJointWrapper.jointDestroyed.AddListener(joint =>
                {
                    releaseObject();
                });
                
                holdingMovableJointWrapper.SetRotateSpring(100000,
                    10000);

                holdingMovableJointWrapper.SetMoveSpring(5000, 500);

                var handJoint = hand.GetComponent<ConfigurableJoint>();

                if (movableGrabbable.grabType == GrabType.Hinge)
                {
                    handJoint.SetMoveSpring(50,5);
                    handJoint.SetRotateSpring(0,0);
                }

                if (movableGrabbable.grabType == GrabType.Strong)
                {
                    handJoint.SetMoveSpring(3000,300);
                    handJoint.SetRotateSpring(3000,300);
                }
                
                if (movableGrabbable.grabType == GrabType.Climb)
                {
                    handJoint.SetMoveSpring(10000,1000);
                    handJoint.SetRotateSpring(0,0);
                }

                if (movableGrabbable.grabType == GrabType.StrongPositionZeroRotation)
                {
                    handJoint.SetMoveSpring(200,20);
                    handJoint.SetRotateSpring(150,15);
                }

                holdingMovableJointWrapper.joint.projectionMode = JointProjectionMode.None;
                
                movableGrabbable.GetComponent<Rigidbody>().solverIterations = 20;

                if(movableGrabbable.snapRotation)
                {
                    holdingMovableJointWrapper.SetRotation(movableGrabbable.snapRotationAngle);
                }
            }
        }
    }

    private void startGrabbing(MovableGrabbable candidate)
    {
        grabbedObject = candidate;
        grabbedObject.grabStart(this);
        grabbedObject.didDestroy.AddListener(onDestroyObject);
        hand.gameObject.SetLayerRecursively(8);
    }

    private void onDestroyObject(MovableGrabbable destroyed) {
       if (holdingMovableJointWrapper != null) {
            Destroy(holdingMovableJointWrapper);
            grabbedObject = null;
            hand.gameObject.SetLayerRecursively(15);
       }
    }

    private ConfigurableJointWrapper holdingMovableJointWrapper = null;

    public void releaseObject()
    {
        Destroy(holdingMovableJointWrapper);
        if (grabbedObject == null) return;
        grabbedObject.grabEnd(this, hand.GetComponent<Rigidbody>().velocity ,hand.GetComponent<Rigidbody>().velocity);
        grabbedObject = null;
        hand.gameObject.SetLayerRecursively(15);
        
        var handJoint = hand.GetComponent<ConfigurableJoint>();
        handJoint.SetMoveSpring(700,80);
        handJoint.SetRotateSpring(500,50);
    }
}
