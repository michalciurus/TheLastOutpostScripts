using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class GrabEvent : UnityEvent<MovableGrabbable> { }
public class PointedAtEvent: UnityEvent<MovableGrabbable, bool> {  }

[DefaultExecutionOrder(100000)]
[RequireComponent(typeof(Rigidbody))]
public class MovableGrabbable : MonoBehaviour {

    public bool canFlick = false;

    public bool snapRotation = false;
    public Vector3 snapRotationAngle = Vector3.zero;

    public GrabType grabType;
    
    [HideInInspector] public bool canGetStuck = false;
    
    private int grabCount = 0;

    Material selectedMaterial;

    Material originalMaterial;

    //I'm using this for EQ for now - u need to be close to the item to grab it
    public bool needsCloseGrab = false;
    
    public bool isGrabbed = false;

    public bool breakable = false;

    [HideInInspector] public GrabEvent didStartGrabbing = new GrabEvent();
    [HideInInspector] public GrabEvent didEndGrabbing = new GrabEvent();
    [HideInInspector] public PointedAtEvent pointedAtEvent = new PointedAtEvent();
    [HideInInspector] public GrabEvent didDestroy = new GrabEvent();

    public UnityEvent<bool> switchedGrabState = new UnityEvent<bool>();

    [HideInInspector] public bool isPointedAt { get; private set; }

    public int priority = 1;

    //FOR EQ
    [HideInInspector] public bool forceGrabAfterHoverHold = false;

    private bool isHovering = false;

    [HideInInspector] public float isHoveringTimer = 0;

    public float throwMultiplier = 1.0f;
    public float torqueMultiplier = 1.0f;
    
    [HideInInspector] public float maxVelocity = 17;

    public float durabilityFactor = 1.0f;

    private float durability = 100;

    
    [HideInInspector] public float wasThrownTimestamp = 99999;

    private TimerUtility timerUtility;

    public bool isInContainer = false;

    [HideInInspector] public List<Grabber> heldByGrabbers = new List<Grabber>();

    private void Awake()
    {
        //gameObject.AddComponent<Scaleable>();

        selectedMaterial = Resources.Load("SelectedPolygonMaterial", typeof(Material)) as Material;

        timerUtility = gameObject.AddComponent<TimerUtility>();

        if (gameObject.layer == 0)
        {
            gameObject.ReplaceLayerRecurs( LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("GrabbableDynamicObject"));
        }

        var slice = gameObject.AddComponent<SliceableObject>();
        slice.DefaultSliceMaterial = selectedMaterial;

        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

    }

     public void grabStart(Grabber grabber)
    {
        heldByGrabbers.Add(grabber);
        
        didStartGrabbing.Invoke(this);
        switchedGrabState.Invoke(true);

        var rigidBody = GetComponent<Rigidbody>();
        
        grabCount++;

        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        isGrabbed = true;

        setHiglighted(false);
    }

    public void didDamage()
    {
        if (!breakable) return;
        
        durability -= 20 * durabilityFactor;

        if(durability <= 0 && GetComponent<SliceableObject>() != null)
        {
            DelayedTasker.instance.AddTask(0.3f, () =>
            {
                GetComponent<SliceableObject>().sliceInHalf();
                didDestroy.Invoke(this);
                
                AudioClip clip = (AudioClip)Resources.Load("Sounds/broken");
                
                AudioSource.PlayClipAtPoint(clip, transform.position);
            });

        }
    }

    public void letGoObject()
    {
        didDestroy.Invoke(this);
    }
    
     public void setPointedAt(bool pointedAt) {

        if(pointedAt != isPointedAt) {

            setHiglighted(pointedAt);

            isPointedAt = pointedAt;
            pointedAtEvent.Invoke(this, pointedAt);
        }
    }

    void setHiglighted(bool highlighted) {
        if (highlighted && selectedMaterial != null && isGrabbed == false) {
            originalMaterial = GetComponent<MeshRenderer>().material;
            GetComponent<MeshRenderer>().material = selectedMaterial;
        }
        else if (originalMaterial != null) {
            GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }



    public void setIsHovering(bool isHovering) {
        this.isHovering = isHovering;
    }

    private void Update() {
        
        if(isHovering) {
            isHoveringTimer += Time.deltaTime;
        } else {
            isHoveringTimer = 0;
        }

        wasThrownTimestamp += Time.deltaTime;
    }


     public void grabEnd(Grabber grabber, Vector3 linearVelocity, Vector3 angularVelocity)
     {
         heldByGrabbers.Remove(grabber);
        didEndGrabbing.Invoke(this);
        switchedGrabState.Invoke(false);

        grabCount--;

        wasThrownTimestamp = 0;

        var mainPlayerVelocityNegative = GameObject.Find("MainPlayer").GetComponent<Rigidbody>().velocity * -1;

        GetComponent<Rigidbody>().velocity *= 2 * throwMultiplier;

        GetComponent<Rigidbody>().angularVelocity *= torqueMultiplier;

        GetComponent<Rigidbody>().maxAngularVelocity = 10;

        isGrabbed = grabCount != 0;
        
        if (!isGrabbed) {
            var rigidBody = GetComponent<Rigidbody>();
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
    
    private void OnDestroy() {
        didDestroy.Invoke(this);
    }
    
}

public class FixedSizedQueue<T> {
    public Queue<T> q = new Queue<T>();

    public int Limit { get; set; }
    public void Enqueue(T obj) {
        q.Enqueue(obj);

        while (q.Count > Limit) q.Dequeue();
    }

    public int Count()
    {
        return q.Count;
    }

    public T first()
    {
        return q.ToArray()[0];
    }

    public T Peek() {
        return q.Peek();
    }
}

public enum GrabType
{
    Strong,
    Hinge,
    Climb,
    StrongPositionZeroRotation
}