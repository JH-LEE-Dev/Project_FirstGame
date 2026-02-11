using UnityEngine;

public class BulletVisualEffectComponent : EntityComponent
{
    private Animator animator;
    [SerializeField] private GameObject effectObject;

    protected override void Awake()
    {
        animator = effectObject.GetComponentInChildren<Animator>();    
    }

    public void PlayExplosionEffect()
    {
        animator.SetBool("bExplode", true);
        Sound.Play("EnemyExplosion",transform.position,1f,false);
    }

    public void PlayImpactEffect()
    {
        animator.SetBool("bImpact", true);
        Sound.Play("Impact", transform.position, 1f, false);
    }

    public void PlayFireEffect()
    {
        animator.SetBool("bFired", true);
        Sound.Play("Fire", transform.position, 1f, false);
    }
}
