using UnityEngine;

public class AquaBurst : Bullet
{
    public float speed;

    [HideInInspector] public Animator animator;

    [SerializeField] public GameObject AquaExplosion;

    [HideInInspector] public FxAutoHideOnAnimEnd bigFx;

    private bool fxPoolReady = false;

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider, _damageSystem);

        originRange = 0.45f;
        originExplosionRange = 3f;
        speed = 10f;

        animator = GetComponentInChildren<Animator>();
        EnsureFxPool();

    }

    public void EnsureFxPool()
    {
        if (fxPoolReady) return;
        fxPoolReady = true;

        if (AquaExplosion)
        {
            AquaExplosion.SetActive(false);

            var bigGo = Instantiate(AquaExplosion, AquaExplosion.transform.parent);
            bigGo.SetActive(false);

            bigFx = bigGo.GetComponent<FxAutoHideOnAnimEnd>();
            if (!bigFx) bigFx = bigGo.AddComponent<FxAutoHideOnAnimEnd>();
        }


    }
}
