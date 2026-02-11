using System.Collections.Generic;
using UnityEngine;

public class PrismBolt : Bullet
{

    public float speed;

    [HideInInspector] public Animator animator;

    [SerializeField] public GameObject prismExplosion;
    [SerializeField] public GameObject prismSubExplosion;

    [HideInInspector] public FxAutoHideOnAnimEnd bigFx;
    [HideInInspector] public FxAutoHideOnAnimEnd[] subFx;

    public float originExplosionSubRange {  get; private set; }

    private bool fxPoolReady = false;

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider, _damageSystem);

        originRange = 0.3f;
        originExplosionRange = 1f;
        originExplosionSubRange = 0.3f;
        speed = 5f;

        animator = GetComponentInChildren<Animator>();

        EnsureFxPool();
    }

    public void EnsureFxPool()
    {
        if (fxPoolReady) return;
        fxPoolReady = true;

        if (prismExplosion)
        {
            prismExplosion.SetActive(false);

            var bigGo = Instantiate(prismExplosion, prismExplosion.transform.parent);
            bigGo.SetActive(false);

            bigFx = bigGo.GetComponent<FxAutoHideOnAnimEnd>();
            if (!bigFx) bigFx = bigGo.AddComponent<FxAutoHideOnAnimEnd>();
        }


        // ¼­ºê 8°³.

        subFx = new FxAutoHideOnAnimEnd[8];

        if (prismSubExplosion)
        {
            prismSubExplosion.SetActive(false);

            for (int i = 0; i < 8; i++)
            {
                var go = Instantiate(prismSubExplosion, prismSubExplosion.transform.parent);
                go.SetActive(false);

                var fx = go.GetComponent<FxAutoHideOnAnimEnd>();
                if (!fx) fx = go.AddComponent<FxAutoHideOnAnimEnd>();
                subFx[i] = fx;
            }
        }
    }
}
