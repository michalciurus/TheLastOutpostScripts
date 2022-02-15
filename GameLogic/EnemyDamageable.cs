using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CanTakeDamage
{
     void takeDamage(EnemyDamageable bodypart, float damage, GameObject source);
     void blind(int seconds);
     void pierce(EnemyDamageable bodypart, bool dieOnPierce, float damage, GameObject source);
}

public class EnemyDamageable : MonoBehaviour
{
    public int health = 100;
    public bool dieOnDismember = false;
    public bool dieOnPierce = false;

    public GameObject attachedTo;
    
    public GameObject canTakeDamageParent;
    
    public CanTakeDamage getCanTakeDamageParent()
    {
        return canTakeDamageParent.GetComponent(typeof(CanTakeDamage)) as CanTakeDamage;
    }

    public void Pierce(float damage, GameObject source)
    {
        getCanTakeDamageParent().pierce(this, dieOnPierce, damage, source);
    }

    public void Update()
    {
        var limb = GetComponent<limbDM>();
        if (limb && health <= 0)
        {
            limb.dismemberMe();
        }
    }
}
