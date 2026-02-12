using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcDischarge : Bullet
{
    [Header("Main Settings")]
    public SpriteRenderer sr { get; private set; }

    public Collider2D firstTarget { get; set; }

    public List<GameObject> activatedVfxList { get; set; }
    [field: SerializeField] public ObjectPoolingSystem vfxPooling { get; set; }

    [field: SerializeField] public int maxTransference { get; set; } = 2;
    [field: SerializeField] public float finderRadius { get; set; } = 20f;
    [field: SerializeField] public float chainDelay { get; set; } = 0.1f;

    public override void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(_characterStatProvider, _bulletEffectProvider, _damageSystem);

        activatedVfxList = new List<GameObject>(vfxPooling.maxPoolSize);
    }

    public void AllDeActivateVFX()
    {
        foreach (GameObject vfx in activatedVfxList)
        {
            if (null == vfx || !vfx.activeSelf)
                return;

            ParticleSystem particle = vfx.GetComponentInChildren<ParticleSystem>();
            particle?.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            vfxPooling.Pool.Release(vfx);
        }

        activatedVfxList.Clear();
    }

    private ParticleSystem GetVFX()
    {
        GameObject obj = vfxPooling.Pool.Get();
        if (null == obj)
            return null;

        obj.SetActive(true);
        ParticleSystem particle = obj.GetComponentInChildren<ParticleSystem>();
        activatedVfxList.Add(obj);

        return particle;
    }

    public void PlayVFX(Vector2 _startPos, Vector2 _endPos)
    {
        ParticleSystem particle = GetVFX();
        if (null == particle)
            return;

        ParticleSystem.EmitParams newEmitter = new ParticleSystem.EmitParams();
        newEmitter.position = _startPos;
        particle.Emit(newEmitter, 1);

        newEmitter.position = _endPos;
        particle.Emit(newEmitter, 1);

        particle.Play(true);
    }
}
