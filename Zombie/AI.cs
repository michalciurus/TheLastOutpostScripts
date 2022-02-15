using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.Dynamics;
using System;
using System.Linq;
using RootMotion;
using Technie.VirtualConsole;
using Unity.Collections;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using Math = System.Math;
using Random = UnityEngine.Random;

public enum AIState
{
    Stand,
    Patrol,
    DieBackwards,
    Follow,
    GoToLastKnownLocation,
    Attack,
    AttackMirrored,
    Chase,
    Pushback,
    PushbackAndFall,
    Random,
    StandUpSupine,
    Fallen,
    Stun,
    StandUp,
    Dead,
    Scream,
    Agony,
    BiteStart,
    Puppet,
    BothArmsAttack,
    HyperchaseStart,
    NeckBite,
    BittenIn,
    Crawl,
    Blinded
}

public enum SightDistance: int
{
    VeryClose = 2,
    Medium = 8,
    VeryFar = 12
}

public interface CanHear
{
    void MadeSound(GameObject go, float range, bool vertical);
}

[RequireComponent(typeof(Rigidbody))]
public class AI : MonoBehaviour, CanHear, CanTakeDamage
{
    private GameObject player;
    private Rigidbody rigidBody;
    private NavMeshAgent agent;
    private Animator animator;

    public PuppetMaster puppetMaster;

    public BehaviourPuppet puppetBehaviour;

    public PatrolPath patrolPath;

    public dismembermentManager dismemberManag;

    [HideInInspector] public Vector3 lastKnownPlayerPosition;

    [HideInInspector] public Transform followTarget;

    [HideInInspector] public AIState currentState;

    [HideInInspector] public bool clearLineOfSight = false;

    [HideInInspector] public bool inLineOfSight = false;

    [HideInInspector] public bool isAFlockLeader = false;

    [HideInInspector] public bool followsAFlockMember = false;
    
    public bool canReachPlayer = false;

    public bool gotDamage = false;

    public bool shouldIdleFlag = false;

    public bool readyToGoFromIdle = false;
    
    public bool wantsToCheckOutLastKnownPosition = false;

    public bool shouldStand = false;

    public bool isBlinded = false;
    
    private GameObject head;

    public bool isBittenIn = false;
    
    private const int sightAngle = 65;
    private const int sightDistance = 10;

    public bool manualRotation = false;

    public float staggerFactor = 100;

    public bool isChasing = false;

    Vector3 lastHitVelocity = Vector3.zero;

    public bool isDead = false;

    public float suspiciousLevel = 0;

    [HideInInspector] private Vector3 hipsForward;
    [HideInInspector] private Vector3 hipsUp;
    
    public bool hasLeftHand = true;
    public bool hasRightHand = true;

    public GameObject rightHand;

    public bool STAND_DEBUG = false;

    public float speedMultiplier = 1f;
    
    public int currentPatrolPoint = 0;

    [HideInInspector] public bool isControlledByPuppetBehavior = false;

    [HideInInspector] public float attackCooldown = 0.0f;

    [HideInInspector] public float biteInCooldown = 0.0f;

    [HideInInspector] public bool noLegs = false;


    private float speedRandomizer = 1.0f;
    
    void Start()
    {
        head = transform.parent.FindDeepChild("Head").gameObject;
        
        var k = head.transform;
        
        STAND_DEBUG = DEBUG.ENEMIES_STAND;

        animator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        player = GameObject.Find("MainPlayer");

        agent.Warp(transform.position);

        speedRandomizer = Random.Range(0.7f, 1.0f);
        
        var children = transform.parent.gameObject.GetComponentsInChildren<Rigidbody>();

        agent.updateRotation = true;
        agent.updateUpAxis = false;
        agent.isStopped = true;

        foreach (Rigidbody childRigidBody in children)
        {
            var dis = childRigidBody.gameObject.GetComponent<EnemyDamageable>();
            if(dis) dis.canTakeDamageParent = this.gameObject;
        }

        //transform.parent.gameObject.GetComponentInChildren<DismemberManager>().OnDie.AddListener(OnDie);

        staggerFactor = 100;

        hipsForward = Quaternion.Inverse(puppetMaster.muscles[0].transform.rotation) * puppetMaster.targetRoot.forward;
        hipsUp = Quaternion.Inverse(puppetMaster.muscles[0].transform.rotation) * puppetMaster.targetRoot.up;
        
    }
    
    public void MadeSound(GameObject go, float range, bool vertical)
    {

        var delta = transform.position - go.transform.position;

        //Prevent hearing across floors
        if (!vertical && delta.y >= 2.0f)
        {
            return;
        }
        
        if ((delta).magnitude <= range)
        {
            wantsToCheckOutLastKnownPosition = true;
            lastKnownPlayerPosition = go.transform.position;
        }
    }

    public void ChangeToState(AIState state, bool forceUpdate, float blend = 1.0f)
    {
        if (currentState == state && !forceUpdate)
        {
            return;
        }
        
        var stateName = state.ToString();

        Animator animator = this.GetComponent<Animator>();

        currentState = state;
        
        animator.CrossFadeInFixedTime(stateName, blend);
        
        var feetLayerAnimations = new AIState[]
            {AIState.Follow, AIState.Patrol, AIState.Chase, AIState.Pushback, AIState.NeckBite, AIState.HyperchaseStart, AIState.GoToLastKnownLocation, AIState.Attack, AIState.AttackMirrored};

        animator.SetLayerWeight(1, feetLayerAnimations.Contains(currentState) ? 1.0f : 0.0f);
        
        gameObject.name = state.ToString();
        
        if (state == AIState.NeckBite)
        {
            startNeckBite();
        }
    }

    private Action deleteBiteJoint;

    //hack
    
    private void startNeckBite()
    {
        if (deleteBiteJoint != null) return;
        
        biteInCooldown = 0.0f;

        if (head.GetComponent<NeckBiteListener>() != null)
        {
            Destroy(head.GetComponent<NeckBiteListener>());
        }

        var neckBiteListener = head.AddComponent<NeckBiteListener>();

        neckBiteListener.didBite.AddListener(() =>
        {
            Destroy(neckBiteListener);
            
            Debug.Log("DID COLLIDE AND CREATED JOINT");


            var biteJoint = gameObject.AddComponent<ConfigurableJointWrapper>();
            biteJoint.SetMoveSpring(4000, 400);
            biteJoint.joint.connectedBody = player.GetComponent<Rigidbody>();
            biteJoint.joint.autoConfigureConnectedAnchor = true;
            biteJoint.joint.connectedAnchor = Vector3.zero;
            
            biteInCooldown = 0.0f;

            deleteBiteJoint = () =>
            {
                if(biteJoint != null) Destroy(biteJoint);
                if(!neckBiteListener.IsNullOrDestroyed()) Destroy(neckBiteListener);
                
                biteInCooldown = 0.0f;
            };

        });

    }

    public void pierce(EnemyDamageable bodypart, bool dieOnPierce, float damage, GameObject source)
    {
        
        if (dieOnPierce)
        {
            OnDie();
        } 
        
        PlayBloodParticle(bodypart.transform.position, source.transform);
        
        PlayHitSound();
        
    }

    public void slomo()
    {
        var orig = Time.fixedDeltaTime;
        
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * .02f;

        DelayedTasker.instance.AddTask(0.4f, () =>
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = orig;
        });
    }

    private void OnDie()
    {

        //slomo();

        isDead = true;

        puppetMaster.state = PuppetMaster.State.Dead;

        agent.isStopped = true;
    }

    float raycastGate = 0.0f;

    private bool isWalkingAnimation = false;

    private float biteInDamageCooldown = 0;

    void Update()
    {
        
        var distanceToPlayer = (player.transform.position - transform.position).magnitude;

        shouldStand = (patrolPath == null || patrolPath.flockPatrolPoints.Count == 0) && followTarget == null;
        agent.updatePosition = false;

        isBittenIn = deleteBiteJoint != null;

        if (puppetBehaviour.gameObject.active && puppetBehaviour.state != RootMotion.Dynamics.BehaviourPuppet.State.Puppet)
        {
            isControlledByPuppetBehavior = true;
            animator.SetLayerWeight(1, 0);
        }
        else
        {
            isControlledByPuppetBehavior = false;
        }

        blindTimer = Math.Max(0, blindTimer - Time.deltaTime);

        isBlinded = blindTimer > 0;

        if (currentState != AIState.NeckBite && currentState != AIState.BittenIn)
        {
            if (deleteBiteJoint != null)
            {
                deleteBiteJoint();
                deleteBiteJoint = null;
            }
        }

        biteInCooldown += Time.deltaTime;
        biteInDamageCooldown += Time.deltaTime;

        if (deleteBiteJoint != null && biteInDamageCooldown > 1.0f)
        {
            player.GetComponent<MainPlayer>().damage(20.0f, this.gameObject, true);
            biteInDamageCooldown = 0.0f;
        }
        
        hasLeftHand = dismemberManag.LForearm;
        hasRightHand = dismemberManag.RForearm;

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;
        float angle = Vector3.Angle(direction, transform.forward);

        if (Mathf.Abs(angle) <= sightAngle && direction.magnitude < sightDistance)
        {
            raycastGate += Time.deltaTime;

            if (raycastGate > 0.4f)
            {
                raycastGate = 0.0f;
                expensiveRaycastClearLineOfSight();
            }

            inLineOfSight = true;
        }
        else
        {
            clearLineOfSight = false;
            inLineOfSight = false;
        }

        var enemyPlayerDistance = (player.transform.position - transform.position).magnitude;
        
        canReachPlayer = enemyPlayerDistance <= 2.0f;

        agent.nextPosition = transform.position;

        if (clearLineOfSight)
        {
            var susMultiplier = 400;
            
            if (enemyPlayerDistance > (int) SightDistance.VeryClose)
            {
                susMultiplier = 16;
            }

            if (enemyPlayerDistance > (int) SightDistance.Medium)
            {
                susMultiplier = 12;
            }
            
            if (enemyPlayerDistance > (int) SightDistance.VeryFar)
            {
                susMultiplier = 8;
            }

            if (!player.GetComponent<MainPlayer>().isCrouching())
            {
                susMultiplier *= 2;
            }

            var lastSus = suspiciousLevel;
            
            suspiciousLevel += Time.deltaTime * susMultiplier;

            if (lastSus < 35 && suspiciousLevel >= 35)
            {
                wantsToCheckOutLastKnownPosition = true;
                lastKnownPlayerPosition = player.transform.position;
                Debug.Log("CHCKOUT");
            }

            suspiciousLevel = Mathf.Min(suspiciousLevel, 100);

            outOfSightTimer = 0;
        }
        else
        {
            suspiciousLevel -= Time.deltaTime * 3;

            if (suspiciousLevel < 0)
            {
                suspiciousLevel = 0;
            }

            outOfSightTimer += Time.deltaTime;
        }

        damageTimer += Time.deltaTime;

        staggerFactor = Mathf.Min(100f, staggerFactor + Time.deltaTime * 5);
        staggerFactor = Mathf.Max(0, staggerFactor);

        if (staggerFactor <= 60)
        {
            puppetBehaviour.collisionResistance = 0.5f;
            puppetBehaviour.minimumUnpinValue = 0.3f;
        }
        else
        {
            puppetBehaviour.collisionResistance = 1.0f;
            puppetBehaviour.minimumUnpinValue = currentItemHitMinimumUnpinValue;
        }

        //DEBUG DISPLAY

        // Transform display = transform.parent.FindDeepChild("DEBUG_DISPLAY");
        //
        // if (display != null)
        // {
        //     display.GetComponent<DebugDisplay>().transform.FindDeepChild("Text").GetComponent<Text>().text =
        //         "STGR: " + staggerFactor.ToString() + "\n SUS:" + suspiciousLevel.ToString();
        // }
        
        if (!isControlledByPuppetBehavior)
        {
            puppetMaster.gameObject.SetActive(distanceToPlayer < 2.0f);
            puppetBehaviour.gameObject.SetActive(distanceToPlayer < 2.0f);
        }
        else
        {
            staggerFactor = 100;
        }
        
    }

    private void FixedUpdate()
    {
        if (!agent.isStopped && agent.remainingDistance > agent.stoppingDistance)
        {

            var multiplier = speedMultiplier;

            if (noLegs)
            {
                multiplier /= 1.0f / 2.5f;
            }

                        
            if (staggerFactor < 50)
            {
                multiplier *= 0.45f;
            } else if (staggerFactor < 80)
            {
                multiplier *= 0.7f;
            }

            multiplier  *= staggerFactor / 100.0f;

            multiplier *= speedRandomizer;
            
            var newVelocity = new Vector3(agent.velocity.normalized.x * multiplier , rigidBody.velocity.y, agent.velocity.normalized.z * multiplier );

            if (currentState == AIState.Patrol)
            {
                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, newVelocity, Time.deltaTime * 10);
            }
            else
            {
                rigidBody.velocity = newVelocity;
            }

        }
        
        //DRAG
        var drag = 0.95f;
        var vel = rigidBody.velocity;
        vel.x *= drag;
        vel.z *= drag;
        rigidBody.velocity = vel;
        
        // //DRAG

        if (manualRotation)
        {
            float rotationSpeed = 4f;
            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (rigidBody.velocity.magnitude > 0.1f)
        {
            if (!isWalkingAnimation)
            {
                isWalkingAnimation = true;
                animator.CrossFadeInFixedTime("Patrol", 0.2f, 1);
            }
        }
        else 
        {
            if (isWalkingAnimation)
            {
                isWalkingAnimation = false;
                animator.CrossFadeInFixedTime("Stand", 0.2f, 1);
            }
        }

        if (rigidBody.GetComponent<Rigidbody>().velocity.y < -2.0f)
        {
            puppetBehaviour.collisionLayers.AddToMask(new string[] {"Ground"});
        }
        else
        {
            puppetBehaviour.collisionLayers.RemoveFromMask(new string[] {"Ground"});
        }
    }

    private float currentItemHitMinimumUnpinValue = 0.5f;

    private void LateUpdate()
    {
        gotDamage = false;
    }

    public Vector3 getCurrentPatrolPoint() {
        return patrolPath.flockPatrolPoints[currentPatrolPoint];
    }

    Double outOfSightTimer = 0;

    public bool canChase()
    {
        var toChase = new AIState[]
            {AIState.Patrol, AIState.Pushback, AIState.Follow, AIState.GoToLastKnownLocation, AIState.Stand};

        return toChase.Contains(currentState);
    }

    private Double blindTimer = 0.0f;

    public void blind(int seconds)
    {
        blindTimer = seconds;
    }

    public void incrementPatrolPoint()
    {
        if (patrolPath == null) return;
        
        currentPatrolPoint++;
        
        if (currentPatrolPoint >= patrolPath.flockPatrolPoints.Count) {
            currentPatrolPoint = 0;
        }
    }

    public Transform threat;

    private float ik = 0;
    
    private void OnAnimatorIK(int layerIndex)
    {
        var h = GameObject.Find("RightGrabber").GetComponent<Grabber>().hand.GetComponent<Rigidbody>().velocity
            .magnitude;

        if (h > 1.0f)
        {
            ik = Math.Min( ik + Time.deltaTime * 5, 1.0f) ;
        }
        else
        {
            ik = Math.Max(0, ik - Time.deltaTime) ;
        }
        
      //  animator.SetIKPosition(AvatarIKGoal.LeftHand, threat.position);
      //  animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ik);
    }

    public void expensiveRaycastClearLineOfSight()
    {
        var lookAtTransform = new GameObject();

        lookAtTransform.name = "Calculation";
        lookAtTransform.transform.position = player.transform.position;
        lookAtTransform.transform.LookAt(transform);

        Animator animator = this.GetComponent<Animator>();

        var headPoint = transform.position;
        headPoint.y += GetComponent<CapsuleCollider>().height - 0.5f;
        var forward = transform.rotation * new Vector3(0, 0, 0.4f);
        headPoint = headPoint + forward;

        var playerHeight = player.GetComponent<CapsuleCollider>().height;
        var playerWidth = player.GetComponent<CapsuleCollider>().radius;

        var leftCorner = new Vector3(-playerWidth, playerHeight / 2.0f, 0);
        var rightCorner = new Vector3(playerWidth, playerHeight / 2.0f, 0);
        var leftBottomCorner = new Vector3(-playerWidth, -playerHeight / 2.0f, 0);
        var rightBottomCorner = new Vector3(playerWidth, -playerHeight / 2.0f, 0);

        var hitResolution = 3;
        var horizontalDif = (leftCorner - rightCorner).magnitude / hitResolution;
        var verticalDif = (leftCorner - leftBottomCorner).magnitude / hitResolution;

        List<Vector3> raycastDirections = new List<Vector3>();

        var results = new NativeArray<RaycastHit>(12, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(12, Allocator.TempJob);

        var ind = 0;

        for (int i = 0; i < hitResolution; i++)
        {
            var nextPoint = new Vector3(leftCorner.x + i * horizontalDif, leftCorner.y, 0);
            raycastDirections.Add(nextPoint);

            for (int j = 0; j < hitResolution; j++)
            {
                var nextVerticalPoint = new Vector3(nextPoint.x, nextPoint.y - j * verticalDif, 0);
                raycastDirections.Add(nextVerticalPoint);
            }
        }
        
        var hitCount = 0;

        foreach (Vector3 point in raycastDirections)
        {
            var pointInWorld = lookAtTransform.transform.position - lookAtTransform.transform.rotation * point;
            pointInWorld = pointInWorld - headPoint;

            //DebugPlus.DrawRay(headPoint, pointInWorld.normalized * 10).duration = 3;

            commands[ind] = new RaycastCommand(headPoint, pointInWorld.normalized, (int)SightDistance.VeryFar);
            ind++;
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));

        handle.Complete();

        foreach (RaycastHit hit in results)
        {
            if (hit.collider != null && hit.collider.gameObject.name == "MainPlayer")
            {
                hitCount++;
            }
        }

        results.Dispose();
        commands.Dispose();

        clearLineOfSight = hitCount > 3;

        Destroy(lookAtTransform);
    }

    public void ApplyPushbackForce()
    {
        var l = lastHitVelocity;
        l.y = 0;
        
        rigidBody.AddForce(l.normalized * 150, ForceMode.Impulse);

    }

    float damageTimer = 0;

    private Task addDamageVelocityTask;
    
    private void PlayBloodParticle(Vector3 point, Transform damagingObject)
    {
        var k = UnitySugar.instantiatePrefab("FX_BloodSplat_01");
        var c = Instantiate(k);

        c.transform.LookAt(damagingObject.transform);
        
        c.transform.parent = transform;
        c.transform.position = point;
    }

    public void PlayHitSound()
    {
        var random = new System.Random();
        var sounds = new[] {"hit1", "hit2", "hit3", "hit4", "hit5"};
        var current = random.Next(0, sounds.Length);

        AudioClip clip = (AudioClip) Resources.Load("Sounds/" + sounds[current]);

        GetComponent<AudioSource>().PlayOneShot(clip);
    }
    

    public void takeDamage(EnemyDamageable bodypart, float damage, GameObject source)
    {
        Debug.Log("--- Zombie take damage to " + bodypart.name + " by: " + source.name);

        if (source != null)
        {
            PlayHitSound();
        }

        lastHitVelocity = source.GetComponent<Rigidbody>().velocity;

        Damageable damageable = null;
        if (source != null)
        {
           damageable = source.GetComponent<Damageable>();
        }

        MovableGrabbable movable = source.GetComponent<MovableGrabbable>();

        if (movable != null)
        {
            movable.didDamage();
        }
        
        currentItemHitMinimumUnpinValue = damageable.unpinEffect.minimumUnpin();
        puppetBehaviour.minimumUnpinValue = currentItemHitMinimumUnpinValue;

        if (bodypart)
        {
            bodypart.health -= (int) damage;
            if (bodypart.health <= 0 && bodypart.dieOnDismember)
            {
                OnDie();
            }
        }

        wantsToCheckOutLastKnownPosition = true;
        lastKnownPlayerPosition = player.transform.position;
        if (suspiciousLevel < 60)
        {
            suspiciousLevel = 60;
        }

        PlayBloodParticle(bodypart.transform.position, bodypart.transform);
        //PlayHitSound();

        if (deleteBiteJoint != null)
        {
            deleteBiteJoint();
            deleteBiteJoint = null;
        }

        if (damageable != null)
        {

            if (damageable.IsThrown())
            {
                staggerFactor = 79;
            }
            

            var broadcaster = bodypart.gameObject.GetComponent<MuscleCollisionBroadcaster>();

            if (broadcaster != null)
            {
                broadcaster.Hit(damageable.unpinEffect.unpinFactor(), Vector3.zero , Vector3.zero);
            }
            
            var timer = 0.0f;
            
            puppetBehaviour.disableRegainingPin = true;

            if (addDamageVelocityTask != null) addDamageVelocityTask.finished = true;

            addDamageVelocityTask = Coroutiner.instance.AddTask(this.gameObject, time =>
            {
                timer += time;

                var progress = timer / damageable.unpinEffect.unpinTime();
                
                bodypart.GetComponent<Rigidbody>().AddForce(lastHitVelocity.normalized * 500 * (1 - progress), ForceMode.Force);

                var finished = timer > damageable.unpinEffect.unpinTime();

                if (finished)
                {
                    puppetBehaviour.disableRegainingPin = false;
                }
                
                return finished;
            });

        }
        
        gotDamage = true;

        if (damageable)
        {
            staggerFactor -= Math.Max(0, C.STAGGER_FACTOR_INCREMENT * damageable.bluntFactor);
        }
    }
}