using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using Technie.VirtualConsole;

[RequireComponent(typeof(Rigidbody))]
[DefaultExecutionOrder(100)]
public class MainPlayer : MonoBehaviour
{

    public Transform cameraRig;
    public float maxVelocityChange = 10.0f;

    public bool keyboardControls = false;

    private bool canTurn = true;

    public Vector3 velocityChange;

    public Vector3 lastCenterEyePosition = Vector3.zero;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    
    private float health = 100f;

    public float handsHeightCorrection = 0f;

    public Rigidbody sphereRigid;
    
    public PlayerInterface playerInterface;
    
    private AudioSource stepsAudioSource;
    
    private float runTimeLeft;

    private void OnCollisionEnter(Collision other)
    {
        var queue = other.gameObject.GetComponent<VelocityQueue>();
        
        if (queue != null)
        {
            if (queue.GetMeanVector().magnitude > 3.0f)
            {
                damage(20, other.gameObject);
            }
        }
    }

    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        
        GetComponent<CapsuleCollider>().height = C.PLAYER_HEIGHT;

        var go = new GameObject();
        go.transform.parent = this.transform;
        go.transform.localPosition = new Vector3(0, -1, 0);

        stepsAudioSource = go.AddComponent<AudioSource>();
        stepsAudioSource.loop = true;
        runTimeLeft = C.PLAYER_SPRINT_DURATION;
    }

    private void Start() {
        TimerUtility.GetSharedTimer().addNewTimer("PlayerDamageThrottle");
        TimerUtility.GetSharedTimer().addNewTimer("PlayerExhausted");
    }

    protected Vector3 centerStartPosition = Vector3.zero;

    void didEnter(GameObject gameObject)
    {
        damage(33, gameObject);
    }

    public void damage(float dmg, GameObject go, bool forceDamage = false)
    {
        if(TimerUtility.GetSharedTimer().getTime("PlayerDamageThrottle") < 1.0f) {
            return;
        }
        
        if (!forceDamage)
        {
            var queue = go.GetComponent<VelocityQueue>();
            if (queue == null) return;
            
            if (queue.GetMeanVector().magnitude < 3.0f)
            {
                return;
            }
        }

        TimerUtility.GetSharedTimer().zeroTimer("PlayerDamageThrottle");
        
        DebugPlus.LogOnScreen("DAMAGED PLAYER " + go.GetComponent<Rigidbody>().velocity.magnitude.ToString()).duration = 0.5f;

        var random = new System.Random();
        var sounds = new[] { "rip1", "rip2", "rip3" };
        var current = random.Next(0, sounds.Length);

        AudioClip clip = (AudioClip)Resources.Load("Sounds/" + sounds[current]);

        GetComponent<AudioSource>().PlayOneShot(clip);

        health -= dmg;

        if (health < 0 && !DEBUG.INVINCIBLE) {
            health = 100;
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    void didExit(GameObject gameObject) {
    

    }

    private Task crouchTask;
    private Task rotateTask;

    private void Update() {

        // if (!backpack.GetComponent<MovableGrabbable>().isGrabbed) {
        //
        //     backpack.transform.parent.position = cameraRig.transform.position;
        //     backpack.transform.parent.rotation = transform.rotation;
        //     backpack.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //     backpack.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //     backpack.transform.localPosition = new Vector3(-0.45f, 0, -0.35f);
        // }
        
        playerInterface.SetHealth((int)health);
        playerInterface.SetSprint( (int)( runTimeLeft /  C.PLAYER_SPRINT_DURATION * 100 ) );
        
        if (XRSugar.wasRightSecondaryButtonPressedDown) {
            var pos = cameraRig.transform.localPosition;

            if (cameraRig.transform.localPosition.y > 1.3f)
            {
                if(crouchTask != null) crouchTask.finished = true;

                crouchTask = Coroutiner.instance.AddTask(this.gameObject, delta =>
                { 
                    var position = cameraRig.transform.localPosition;

                    var oldValue = position.y;
                    
                    position.y = Mathf.MoveTowards(cameraRig.transform.localPosition.y, C.PLAYER_HEIGHT / 2.0f, delta * 3 );

                    var dif = position.y - oldValue; 
                    
                    handsHeightCorrection += dif;
                    
                    cameraRig.transform.localPosition = position;

                    var finished = cameraRig.transform.localPosition.y <= C.PLAYER_HEIGHT / 2.0f;

                    if (finished)
                    {
                        crouchTask = null;
                    }

                    return finished;
                });
            }

            else {
                
                if(crouchTask != null) crouchTask.finished = true;

                crouchTask = Coroutiner.instance.AddTask(this.gameObject, delta =>
                { 
                    var position = cameraRig.transform.localPosition;

                    var oldValue = position.y;
                    
                    position.y = Mathf.MoveTowards(cameraRig.transform.localPosition.y, C.PLAYER_HEIGHT, delta * 2 );

                    var dif = position.y - oldValue; 
                    
                    handsHeightCorrection += dif;
                    
                    cameraRig.transform.localPosition = position;

                    var finished = cameraRig.transform.localPosition.y >= C.PLAYER_HEIGHT;

                    if (finished)
                    {
                        crouchTask = null;
                    }

                    return finished;
                });
            }
        }

        var caps = GetComponent<CapsuleCollider>();

        caps.height = cameraRig.transform.localPosition.y;

        var bottomOfCapsule = transform.position;
        bottomOfCapsule.y -= caps.height / 2.0f;

        cameraRig.transform.parent.position = bottomOfCapsule;
        
        var ss = FindObjectsOfType<MonoBehaviour>().OfType<CanHear>();


        if (GetComponent<Rigidbody>().velocity.magnitude > 1.5 && !isCrouching()) {
            foreach (CanHear s in ss) {
                s.MadeSound(this.gameObject, 5.0f, false);
            }
        }

        if (GetComponent<Rigidbody>().velocity.magnitude > 1.5f && isCrouching())
        {
            foreach (CanHear s in ss) {
                s.MadeSound(this.gameObject, 2.5f, false);
            }
        }
        
        VrDebugStats.SetStat("Gameplay", "PlayerSpeed", GetComponent<Rigidbody>().velocity.magnitude.ToString());
    }

    public bool isCrouching()
    {

        return GetComponent<CapsuleCollider>().height < 1.3;
    }

    
    private void FixedUpdate()
    {
        var rigidbody = GetComponent<Rigidbody>();

        if (keyboardControls) {

            cameraRig.gameObject.GetComponent<UnityEngine.SpatialTracking.TrackedPoseDriver>().enabled = false;

            yaw += 2.0f * Input.GetAxis("Mouse X");
            pitch -= 2.0f * Input.GetAxis("Mouse Y");
             
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

            if (Input.GetKey("w")) {
                rigidbody.velocity = transform.rotation * (Vector3.forward * 10);
            } else if(Input.GetKey("s")) {
                rigidbody.velocity = transform.rotation * (Vector3.forward * -10);
            } else if (Input.GetKey("a")) {
                rigidbody.velocity = transform.rotation * (Vector3.left * 10);
            } else if (Input.GetKey("d")) {
                rigidbody.velocity = transform.rotation * (Vector3.left * -10);
            } else if (Input.GetKey("space")) {
                rigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
            } else {
                var newVel = rigidbody.velocity;
                newVel.x = 0;
                newVel.z = 0;
                rigidbody.velocity = newVel;
            }

            return;
        }

        var centerEye = XRSugar.CenterEye();

        var delta = Vector3.zero;

        if (centerEye.HasValue) {

            var centereEyePosition = XRSugar.Position(centerEye.Value);

            if (centerStartPosition == Vector3.zero) {
                centerStartPosition = XRSugar.Position(centerEye.Value);
                cameraRig.transform.localPosition = centereEyePosition - centerStartPosition + new Vector3(0, 1.8f, 0);
            }

            if (lastCenterEyePosition != Vector3.zero) {
                delta = centereEyePosition - lastCenterEyePosition;
                var position = cameraRig.transform.localPosition;

                position.y += delta.y;

                if (position.y > C.PLAYER_HEIGHT) {
                    handsHeightCorrection -= position.y - C.PLAYER_HEIGHT;
                    position.y = C.PLAYER_HEIGHT;
                }

                if (position.y < (C.PLAYER_HEIGHT / 4.0f)) {
                    //TODO: hands correction
                    //  position.y = C.PLAYER_HEIGHT / 4.0f;
                }

                cameraRig.transform.localPosition = position;

                delta.y = 0;
            }

            lastCenterEyePosition = centereEyePosition;
        }

        var a = XRSugar.LeftAxis();

        
        var speedMultiplier = C.PLAYER_SPEED_MULTIPLIER;
        AudioClip clip = (AudioClip)Resources.Load("Sounds/Slowsteps-3");
        
        if (XRSugar.RightPrimaryButton() && runTimeLeft > 0 && TimerUtility.sharedInstance.getTime("PlayerExhausted") > 3f)
        {
            clip = (AudioClip)Resources.Load("Sounds/Walking-3");
            speedMultiplier *= 1.5f;
            runTimeLeft -= Time.deltaTime;

            if (runTimeLeft <= 0)
            {
                TimerUtility.sharedInstance.zeroTimer("PlayerExhausted");
                AudioClip breathingClip = (AudioClip)Resources.Load("Sounds/breathing");
                GetComponent<AudioSource>().PlayOneShot(breathingClip);
            }
        }
        else
        {
            runTimeLeft = Math.Min(C.PLAYER_SPRINT_DURATION, runTimeLeft + Time.deltaTime);
        }

        if (XRSugar.LeftPrimaryButton() && rigidbody.velocity.y < 0.03f)
        {
            rigidbody.AddForce(new Vector3(0, 40, 0), ForceMode.Impulse);
        }

        if (rigidbody.velocity.magnitude > 0.3f)
        {
            if (stepsAudioSource.clip == null || clip.name != stepsAudioSource.clip.name)
            {
                stepsAudioSource.clip = clip;
            }
            if(!stepsAudioSource.isPlaying)
            stepsAudioSource.Play();
        }
        else
        {
            stepsAudioSource.Stop();
        }

        Vector3 targetVelocity = new Vector3(a.x / 2.0f, 0, a.y / 2.0f);

        targetVelocity *= speedMultiplier;

        if (isCrouching())
        {
            targetVelocity *= 0.6f;
        }

        Vector3 velocityChange = targetVelocity;

        var center = cameraRig;
        var k = Quaternion.Euler(0, center.eulerAngles.y, 0);

        velocityChange = k * velocityChange;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = rigidbody.velocity.y;

        if (targetVelocity.magnitude == 0) {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        } else {
            sphereRigid.freezeRotation = false;
            rigidbody.velocity = Vector3.Slerp(rigidbody.velocity, velocityChange, Time.deltaTime * 10);
            //rigidbody.velocity = velocityChange;
        }

        transform.position = transform.position + (transform.rotation * delta);

        var euler = transform.rotation.eulerAngles;

        if (XRSugar.RightAxis().x < -0.2f) {
            if (canTurn) {
                euler.y -= 45;
                canTurn = false;
            }
        }
        else if (XRSugar.RightAxis().x > 0.2f) {
            if (canTurn) {
                euler.y += 45;
                canTurn = false;
            }
        }
        else {
            canTurn = true;
        }
        
        if (euler != transform.rotation.eulerAngles)
        {
            if (rotateTask != null) rotateTask.finished = true;
            rotateTask = Coroutiner.instance.AddTask(this.gameObject, time =>
            {
                cameraRig.transform.parent.rotation = Quaternion.Slerp(cameraRig.transform.parent.rotation, Quaternion.Euler(euler), Time.deltaTime * 40);
                return cameraRig.transform.parent.rotation == Quaternion.Euler(euler);
            });
        }

        var caps = GetComponent<CapsuleCollider>();

        var anchor = GetComponent<ConfigurableJoint>().anchor;

        anchor.y = - caps.height / 2.0f + 0.2f;

        GetComponent<ConfigurableJoint>().anchor = anchor;

        transform.rotation = Quaternion.Euler(euler);
    }

}
