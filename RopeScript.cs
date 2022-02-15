using UnityEngine;
using System.Collections;

// Require a Rigidbody and LineRenderer object for easier assembly
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]

public class RopeScript : MonoBehaviour
{
    /*========================================
	==  Physics Based Rope				==
	==  File: Rope.js					  ==
	==  Original by: Jacob Fletcher		==
	==  Use and alter Freely			 ==
	==  CSharp Conversion by: Chelsea Hash  ==
	==========================================
	How To Use:
	 ( BASIC )
	 1. Simply add this script to the object you want a rope teathered to
	 2. In the "LineRenderer" that is added, assign a material and adjust the width settings to your likeing
	 3. Assign the other end of the rope as the "Target" object in this script
	 4. Play and enjoy!
 
	 ( Advanced )
	 1. Do as instructed above
	 2. If you want more control over the rigidbody on the ropes end go ahead and manually
		 add the rigidbody component to the target end of the rope and adjust acordingly.
	 3. adjust settings as necessary in both the rigidbody and rope script
 
	 (About Character Joints)
	 Sometimes your rope needs to be very limp and by that I mean NO SPRINGY EFFECT.
	 In order to do this, you must loosen it up using the swingAxis and twist limits.
	 For example, On my joints in my drawing app, I set the swingAxis to (0,0,1) sense
	 the only axis I want to swing is the Z axis (facing the camera) and the other settings to around -100 or 100.
 
 
	*/

    public GameObject hangingObject;
    public Transform target;
    public float resolution = 0.5F;                           //  Sets the amount of joints there are in the rope (1 = 1 joint for every 1 unit)
    public float ropeDrag = 0.1F;                                //  Sets each joints Drag
    public float ropeMass = 0.1F;                           //  Sets each joints Mass
    public float ropeColRadius = 0.5F;                  //  Sets the radius of the collider in the SphereCollider component
                                                        //public float ropeBreakForce = 25.0F;					 //-------------- TODO (Hopefully will break the rope in half...
    private Vector3[] segmentPos;           //  DONT MESS!	This is for the Line Renderer's Reference and to set up the positions of the gameObjects
    private GameObject[] joints;            //  DONT MESS!	This is the actual joint objects that will be automatically created
    private LineRenderer line;                          //  DONT MESS!	 The line renderer variable is set up when its assigned as a new component
    private int segments = 0;                   //  DONT MESS!	The number of segments is calculated based off of your distance * resolution
    private bool rope = false;                       //  DONT MESS!	This is to keep errors out of your debug window! Keeps the rope from rendering when it doesnt exist...

    //Joint Settings
    public Vector3 swingAxis = new Vector3(1, 1, 1);                 //  Sets which axis the character joint will swing on (1 axis is best for 2D, 2-3 axis is best for 3D (Default= 3 axis))
    public float lowTwistLimit = -100.0F;                   //  The lower limit around the primary axis of the character joint. 
    public float highTwistLimit = 100.0F;                   //  The upper limit around the primary axis of the character joint.
    public float swing1Limit = 20.0F;                   //	The limit around the primary axis of the character joint starting at the initialization point.

    void Awake()
    {
        BuildRope();
    }

    void LateUpdate()
    {
        if (rope)
        {
            for (int i = 0; i < segments; i++)
            {
                if (i == 0)
                {
                    line.SetPosition(i, transform.position);
                }
                else
                {
                    line.SetPosition(i, joints[i].transform.position);
                }
            }
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }



    void BuildRope()
    {
        line = gameObject.GetComponent<LineRenderer>();

        segments = (int)(Vector3.Distance(transform.position, target.position) * resolution);

        line.positionCount = segments;

        segmentPos = new Vector3[segments];
        joints = new GameObject[segments];
        segmentPos[0] = Vector3.zero;
        segmentPos[segments - 1] = target.position;

        // Find the distance between each segment
        var segs = segments - 1;
        var seperation = ((target.position - transform.position) / segs);
  

        for (int s = 1; s < segments; s++)
        {
            // Find the each segments position using the slope from above
            Vector3 vector = (seperation * s);
            segmentPos[s] = vector;

            //Add Physics to the segments
            AddJointPhysics(s);
        }

        rope = true;

        hangingObject.transform.position = segmentPos[segmentPos.Length - 1] + transform.position;
        var joint = hangingObject.GetComponent<CharacterJoint>();
        joint.connectedBody = joints[joints.Length - 1].gameObject.GetComponent<Rigidbody>();

        joints[joints.Length - 1].gameObject.GetComponent<SphereCollider>().enabled = false;
        
    }

    void AddJointPhysics(int n)
    {
        joints[n] = new GameObject("Joint_" + n);
        joints[n].transform.SetParent(transform);
        Rigidbody rigid = joints[n].AddComponent<Rigidbody>();
        SphereCollider col = joints[n].AddComponent<SphereCollider>();
        CharacterJoint ph = joints[n].AddComponent<CharacterJoint>();
        ph.swingAxis = swingAxis;
        SoftJointLimit limit_setter = ph.lowTwistLimit;
        limit_setter.limit = lowTwistLimit;
        ph.lowTwistLimit = limit_setter;
        limit_setter = ph.highTwistLimit;
        limit_setter.limit = highTwistLimit;
        ph.highTwistLimit = limit_setter;
        limit_setter = ph.swing1Limit;
        limit_setter.limit = swing1Limit;
        ph.swing1Limit = limit_setter;

        joints[n].transform.position = segmentPos[n] + transform.position;

        rigid.drag = ropeDrag;
        rigid.mass = ropeMass;
        col.radius = ropeColRadius;


        if (n == 1)
        {
            ph.connectedBody = gameObject.GetComponent<Rigidbody>();
        }
        else
        {
            ph.connectedBody = joints[n - 1].gameObject.GetComponent<Rigidbody>();
        }

    }

    void DestroyRope()
    {
        rope = false;
        for (int dj = 0; dj < joints.Length - 1; dj++)
        {
            Destroy(joints[dj]);
        }

        segmentPos = new Vector3[0];
        joints = new GameObject[0];
        segments = 0;
    }
}