using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autohand;
using UnityEngine;

public enum UnpinEffect
{
    Normal,
    Big
}

public static class UnpinEffectExtensions
{
    public static float unpinFactor(this UnpinEffect effect)
    {
        switch (effect)
        {
         case UnpinEffect.Big: return 0.8f;
         case UnpinEffect.Normal: return 0.4f;
        }

        return 0.5f;
    }

    public static float unpinTime(this UnpinEffect effect)
    {
        switch (effect)
        {
            case UnpinEffect.Big: return 0.9f;
            case UnpinEffect.Normal: return 0.8f;
        }

        return 0.5f;
    }

    public static float minimumUnpin(this UnpinEffect effect)
    {
        switch (effect)
        {
            case UnpinEffect.Big: return 0.2f;
            case UnpinEffect.Normal: return 0.5f;
        }

        return 0.5f;
    }
}

// Let's first check if the item doen't cause special sort of damage, like pierce.
public class Damageable : MonoBehaviour, CollisionBroadcasterListener
{
    public float damageFactor = 0.5f;
    public float bluntFactor = 0.5f;
    public bool turnOffDamage = false;
    public bool filterDamageableCollidersOnly = false;
    public UnpinEffect unpinEffect = UnpinEffect.Normal;

    public int priority
    {
        get
        {
            return 1;
        }
    }

    //TODOREFACTOR
    public bool damageOnThrowback = false;
    [HideInInspector] public bool throwbackOnImpact = false;

    private FixedSizedQueue<Vector3> lastVelocities = new FixedSizedQueue<Vector3>();
    private Rigidbody rigidbody;

    private float cooldownTimer = 0;

    private TimerUtility timer;

    private float meleeCooldown = 0.6f;

    private void Start()
    {
        timer = gameObject.AddComponent<TimerUtility>();
        lastVelocities.Limit = 7;
        rigidbody = GetComponent<Rigidbody>();
        timer.addNewTimer("MeleeDamageTimerCooldown");

        var exclusiveColBroadcaster = GetComponent<ExclusiveCollisionBroadcaster>();

        if (exclusiveColBroadcaster != null)
        {
            exclusiveColBroadcaster.listeners.Add(this);
        }
    }

    private void FixedUpdate()
    {
        cooldownTimer += Time.deltaTime;

        var vel = rigidbody.velocity;

        //NOT SO GOOD, BUT OK FOR NOW
        // WE JUST WANT THE VELOCITY OF THE HAND MOTION MINUS PLAYER SPEED
        if (GetComponent<MovableGrabbable>()?.isGrabbed == true || damageOnThrowback)
        {
            vel -= GameObject.Find("MainPlayer").GetComponent<Rigidbody>().velocity;
        }
        
        lastVelocities.Enqueue(vel);
    }
    
    public bool hitOnImpact()
    {
        if (turnOffDamage)
        {
            return false;
        }
        
        var pierce = GetComponent<PierceAction>();
        if (pierce != null)
        {
            if (pierce.joint != null)
            {
                return false;
            }
        }
        
    
        if (lastVelocities.Count() == 0)
        {
            return false;
        }
        
        var mean = lastVelocities.q.ToArray().GetMeanVector();
        
        return mean.magnitude > 1.5f && timer.getTime("MeleeDamageTimerCooldown") >= meleeCooldown;
    }
    
    public bool DidCollide(Collision other)
    {
        return handleCollision(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (GetComponent<ExclusiveCollisionBroadcaster>() == null)
        {
            handleCollision(other);
        }
    }

    private bool handleCollision(Collision other)
    {
        var canPierce = GetComponent<PierceAction>();

        if (filterDamageableCollidersOnly)
        {
            var foundDamageableCollider = false;
            foreach (ContactPoint col in other.contacts)
            {
                if (col.thisCollider.gameObject.name == "Damageable")
                {
                    foundDamageableCollider = true;
                    break;
                }
            }

            if (!foundDamageableCollider) return false;
        }
        
        if (canPierce != null && canPierce.joint != null) return false;
        
        var dismember = other.gameObject.GetComponent<EnemyDamageable>();
        
        if (dismember == null)
            return false;
        
        var angle = Vector3.Angle(lastVelocities.q.ToArray().GetMeanVector(), -other.contacts[0].normal);
        
        if (angle >= 90)
        {
            return false;
        }

        var canTakeDamage = dismember.getCanTakeDamageParent();

        if (hitOnImpact() == false)
        {
            if (isOnCooldown() == false)
            {
                StartThrowbackOnImpact();

                if (damageOnThrowback)
                {
                    GameObject.Find("MainPlayer").GetComponent<MainPlayer>().damage(33, dismember.gameObject);
                }
            }

            return false;
        }

        var movable = GetComponent<MovableGrabbable>();

        if (movable && movable.isGrabbed)
        {
            var grabber = movable.heldByGrabbers.First();
            //Gate to make sure that you're actually moving a hand
            if (grabber && grabber.CurrentControllerVel().magnitude < 1.5f)
            {
                return false;
            }
        }

        var hand = GetComponent<Hand>();

        if (hand)
        {
            var controller = hand.left ? XRSugar.Left() : XRSugar.Right();

            if (controller.HasValue)
            {
                if (XRSugar.Velocity(controller.Value).magnitude < 1.5f)
                {
                    return false;
                }
            }
        }

        canTakeDamage.takeDamage(dismember, 10.0f * damageFactor, this.gameObject);
        timer.zeroTimer("MeleeDamageTimerCooldown");

        return true;
    }

    public bool isOnCooldown()
    {
        lastVelocities.q.Clear();
        return timer.getTime("MeleeDamageTimerCooldown") <= meleeCooldown;
    }
    

    private bool IsGrabbed()
    {

        if (gameObject.name == "Hand")
            return true;
        
        var movable = GetComponent<MovableGrabbable>();

        if (movable != null)
        {
            return movable.isGrabbed;
        }

        return false;
    }

    public bool IsThrown()
    {
        var movable = GetComponent<MovableGrabbable>();

        if (movable == null)
        {
            return false;
        }

        return movable.heldByGrabbers.Count == 0;
    }

    //Was hit by enemy
    //There's a problem where an object hit by enemy springs and damages enemy back even though the player didn't move his hand
    public void StartThrowbackOnImpact()
    {
        lastVelocities.q.Clear();
        timer.zeroTimer("MeleeDamageTimerCooldown");
    }
}
