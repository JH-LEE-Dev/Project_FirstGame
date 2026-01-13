using System;
using UnityEngine;

public class AEH_Impact : MonoBehaviour
{
    private event Action BulletEffectIsFinishedEvent;

    private Animator animator;
    private Bullet bulletObjcet;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        bulletObjcet = GetComponentInParent<Bullet>();
        BulletEffectIsFinishedEvent -= bulletObjcet.BulletEffectIsFinished;
        BulletEffectIsFinishedEvent += bulletObjcet.BulletEffectIsFinished;
    }

    public void ImpactFinished()
    {
        GameObject parentObj = gameObject.transform.parent.gameObject;
        parentObj.SetActive(false);
        animator.SetBool("bImpact", false);

        BulletEffectIsFinishedEvent?.Invoke();
    }

    private void OnDestroy()
    {
        BulletEffectIsFinishedEvent = null;
    }
}
