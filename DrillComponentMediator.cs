using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DrillComponentMediator : MonoBehaviour
{
    private AudioSource drillAudioSource;
    
    void Start()
    {
        var pierceAction = gameObject.AddComponent<PierceAction>();

        var collisionInformer = gameObject.AddComponent<CollisionEventInformer>();
        collisionInformer.meanVectorTreshold = 0.4f;
        collisionInformer.objectOrientationGateVector = Vector3.right;

        collisionInformer.strongCollision.AddListener((Collision collision) =>
        {
            if (collision.gameObject.GetComponent<EnemyDamageable>() != null && anyTriggerOn)
            {
                pierceAction.Pierce(collision);
            }
        });

        drillAudioSource = gameObject.AddComponent<AudioSource>();
        drillAudioSource.clip = Resources.Load<AudioClip>("Sounds/drilling");
        drillAudioSource.Stop();
        drillAudioSource.loop = true;
    }

    public bool anyTriggerOn = false;
    public bool isPlayingPiercingDrillClip = false;
    private void Update()
    {
        
        
        var movable = GetComponent<MovableGrabbable>();
        
        foreach (Grabber grabber in movable.heldByGrabbers)
        {
            if (grabber.CurrentTrigger() > 0.3f)
            {
                anyTriggerOn = true;
            }
        }

        if (movable.heldByGrabbers.Count > 0)
        {
            AudioClip currentClip;
            if (GetComponent<PierceAction>().joint != null)
            {
                currentClip = Resources.Load<AudioClip>("Sounds/drill_grind");
                if (isPlayingPiercingDrillClip == false)
                {
                    isPlayingPiercingDrillClip = true;
                    drillAudioSource.clip = currentClip;
                }
            }
            else
            {
                currentClip = Resources.Load<AudioClip>("Sounds/drilling");

                if (isPlayingPiercingDrillClip)
                {
                    isPlayingPiercingDrillClip = false;
                    drillAudioSource.clip = currentClip;
                }
            }
            

            
            if (anyTriggerOn)
            {
                
                var pierceAction = GetComponent<PierceAction>();
                if (pierceAction.currentEnemyDamageable != null)
                {
                    var canTakeDamageParent = pierceAction.currentEnemyDamageable.GetComponent<EnemyDamageable>()
                        .getCanTakeDamageParent();

                    if (canTakeDamageParent != null)
                    {
                        canTakeDamageParent.takeDamage(
                            pierceAction.currentEnemyDamageable.GetComponent<EnemyDamageable>(), 1.0f, null);

                    }
                }
                
                if (!drillAudioSource.isPlaying)
                {
                    drillAudioSource.Play();
                }
            }
            else
            {
                drillAudioSource.Stop();
            }
        }
        else
        {
            drillAudioSource.Stop();
        }
    }
}
