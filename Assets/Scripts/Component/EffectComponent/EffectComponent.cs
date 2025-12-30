using UnityEngine;

public class EffectComponent : EntityComponent
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
}
