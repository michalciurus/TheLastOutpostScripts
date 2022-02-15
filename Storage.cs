
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ItemSlot {

    public Transform slot;
    public MovableGrabbable item;

    public ItemSlot(Transform s, MovableGrabbable m) {
        slot = s;
        item = m;
    }

}

public class Storage : MonoBehaviour {

    List<ItemSlot> itemSlots = new List<ItemSlot>();

    public Material itemMaterial;

    List<MovableGrabbable> dropCandidates = new List<MovableGrabbable>();

    AreaColliderObserver areaColliderObserver;

    // Start is called before the first frame update
    void Awake()
    {
        areaColliderObserver = gameObject.AddComponent<AreaColliderObserver>();
        areaColliderObserver.type = typeof(GameItem);
        areaColliderObserver.radius = 0.25f;
        areaColliderObserver.parentTransform = transform;
        
        areaColliderObserver.didEnter.AddListener(didEnter);
        areaColliderObserver.didExit.AddListener(didExit);

        // float scaling = 0.5f;
        Vector3[] pts = PointsOnSphere(12);
        List<GameObject> uspheres = new List<GameObject>();
        int i = 0;

        foreach (Vector3 value in pts) {
            var gc = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gc.transform.localScale *= 0.2f;
            uspheres.Add(gc);
            uspheres[i].transform.parent = transform;
            uspheres[i].transform.localPosition = value;

            Destroy(gc.GetComponent<SphereCollider>());
            Destroy(gc.GetComponent<MeshRenderer>());

            gc.GetComponent<MeshRenderer>().material = itemMaterial;

            var itemSlot = new ItemSlot(gc.transform, null);

            itemSlots.Add(itemSlot);

            i++;
        }
    }

    void didDropInTheArea(MovableGrabbable movable) {

        insertObject(movable.gameObject);
    }

    void didEnter(GameObject gameObject) {

        var movable = gameObject.GetComponentInParent<MovableGrabbable>();
        
        if (movable.gameObject == this.transform.parent.gameObject) { return; }
        
        movable.didEndGrabbing.AddListener(didDropInTheArea);
    }

    void didExit(GameObject gameObject)
    {
        var movable = gameObject.GetComponentInParent<MovableGrabbable>();
        
        if (movable.gameObject == this.transform.parent.gameObject) { return; }
        
        movable.didEndGrabbing.RemoveListener(didDropInTheArea);
    }

    public void removeItems() {
        foreach(ItemSlot slot in itemSlots) {
            if(slot.item != null) {
                Destroy(slot.item.gameObject);
            }

            slot.item = null;
        }
    }

    public void Open(bool open)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(open);
        }

        GetComponent<MeshRenderer>().enabled = open;
    }

    Vector3[] PointsOnSphere(int n) {

        List<Vector3> upts = new List<Vector3>();

        var heights = new float[] { 0.3f, -0.3f, 0.0f };

        foreach(float height in heights ) {

            var radius = Mathf.Sqrt(Mathf.Pow(0.5f, 2) - Mathf.Pow(height, 2));

            for(int i = 0; i < n; i++) {

                var theta = (2 * Mathf.PI / n) * i;

                upts.Add(new Vector3(Mathf.Cos(theta) * radius, height, Mathf.Sin(theta) * radius));
            }

            //DebugPlus.LogOnScreen(radius);

        }

        return upts.ToArray();
    }

    public void insertItem(string prefabName) {

        var go = UnitySugar.instantiatePrefab(prefabName);

        insertObject(go);
    }

    private void Update() {

        
    }
    
    ItemSlot getFirstEmptySlot() {

        foreach(ItemSlot slot in itemSlots) {
             if(slot.item == null) {
                return slot;
            }
        }

        return null;
    }
    
    void insertObject(GameObject go)
    {

        if (gameObject.activeSelf == false) return;
        if (go.GetComponent<MovableGrabbable>().isInContainer) return;
        if (go.GetComponent<GameItem>() == null) return;

        AudioSource.PlayClipAtPoint((AudioClip) Resources.Load("Sounds/" + "eq"), transform.position);

        go.GetComponent<Rigidbody>().useGravity = false;
        
        go.SetLayerRecursively(LayerMask.NameToLayer("IgnoreAllCollision"));

        go.GetComponent<Scaleable>().ScaleBy(0.5f, 0.5f);

        go.GetComponent<MovableGrabbable>().isInContainer = true;

        var joint = go.AddComponent<FixedJoint>();

        joint.connectedBody = this.GetComponent<Rigidbody>();
        joint.autoConfigureConnectedAnchor = false;

        var firstEmpty = getFirstEmptySlot();

        joint.connectedAnchor = firstEmpty.slot.localPosition;

        var index = itemSlots.FindIndex(ind => ind.slot.localPosition == firstEmpty.slot.localPosition);

        itemSlots[index] = new ItemSlot(firstEmpty.slot, go.GetComponent<MovableGrabbable>());

        joint.connectedMassScale = 0.01f;

        go.GetComponent<MovableGrabbable>().needsCloseGrab = true;

        go.transform.parent = this.transform;

        UnityAction<MovableGrabbable> action = (MovableGrabbable) => {

            GameObject.Destroy(joint);

            go.GetComponent<Scaleable>().SetOriginalScale(0.5f);
            
            AudioSource.PlayClipAtPoint((AudioClip) Resources.Load("Sounds/" + "eq"), transform.position);

            itemSlots[index] = new ItemSlot(firstEmpty.slot, null);
            
            go.gameObject.layer = LayerMask.NameToLayer("GrabbableDynamicObject");

            go.GetComponent<Rigidbody>().useGravity = true;

            go.transform.parent = null;

            go.GetComponent<MovableGrabbable>().isInContainer = false;

            go.GetComponent<MovableGrabbable>().needsCloseGrab = false;

            go.GetComponent<MovableGrabbable>().forceGrabAfterHoverHold = false;
            
            go.GetComponent<MovableGrabbable>().didStartGrabbing.RemoveAllListeners();
        };

         go.GetComponent<MovableGrabbable>().forceGrabAfterHoverHold = true;

         go.GetComponent<MovableGrabbable>().didStartGrabbing.AddListener(action);
    }
    

}