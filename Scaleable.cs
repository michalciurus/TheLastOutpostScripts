using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Scaleable : MonoBehaviour
{
    private Tweener currentTween;

    private Vector3 originalScale = Vector3.positiveInfinity;

    public bool isScaled = false;

    public void ScaleBy(float scale, float duration)
    {
        currentTween?.Complete();

        //Can scale only once
        if (isScaled) return;

        isScaled = true;

        originalScale = transform.localScale;
        
        currentTween = transform.DOScale(transform.localScale * scale, duration);
    }

    public void SetOriginalScale(float duration)
    {
        if (originalScale != Vector3.positiveInfinity)
        {
            isScaled = false;
            
            currentTween?.Complete();

            currentTween = transform.DOScale(originalScale, duration);
        }
    }
}
