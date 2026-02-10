using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletStateMachine : MonoBehaviour
{
    //외부 의존성
    ICharacterStatProvider characterStatProvider;
    IBulletEffectProvider bulletEffectProvider;
    DamageCalcComponent damageCalcComponent;
    Bullet bullet;

    private BulletState currentState;
    private Dictionary<Type, BulletState> states = new Dictionary<Type, BulletState>();
    private BulletStateCtx ctx;

    private Dictionary<BulletType, BulletBehaviorData> bulletBehaviors = new Dictionary<BulletType, BulletBehaviorData>((int)BulletType.MAX);
    [SerializeField] private List<BulletBehaviorData> behaviorDatas = new List<BulletBehaviorData>((int)BulletType.MAX);

    public void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectReceiver,
        Bullet _bullet, DamageCalcComponent _damageCalcComponent)
    {
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectReceiver;
        bullet = _bullet;
        damageCalcComponent = _damageCalcComponent;

        ctx = new BulletStateCtx();
        ctx.Initialize(this, characterStatProvider, bulletEffectProvider, bullet, damageCalcComponent, bulletBehaviors);

        BS_BeforeFire bs_BeforeFire = new BS_BeforeFire();
        BS_Fly bs_Fly = new BS_Fly();
        BS_Hit bs_Hit = new BS_Hit();
        bs_BeforeFire.Initialize(ctx);
        bs_Fly.Initialize(ctx);
        bs_Hit.Initialize(ctx);

        states[bs_BeforeFire.GetType()] = bs_BeforeFire;
        states[bs_Fly.GetType()] = bs_Fly;
        states[bs_Hit.GetType()] = bs_Hit;

        for (int i = 0; i < behaviorDatas.Count; ++i)
        {
            bulletBehaviors[behaviorDatas[i].bulletType] = behaviorDatas[i];
        }

        foreach (KeyValuePair<BulletType, BulletBehaviorData> pair in bulletBehaviors)
        {
            pair.Value.behavior_BeforeFire.Initialize(bullet, characterStatProvider, bulletEffectProvider,damageCalcComponent);
            pair.Value.behavior_Fly.Initialize(bullet, characterStatProvider, bulletEffectProvider, damageCalcComponent);
            pair.Value.behavior_Hit.Initialize(bullet, characterStatProvider, bulletEffectProvider, damageCalcComponent);
        }
    }

    public void ChangeState<T>() where T : BulletState
    {
        var type = typeof(T);

        if (!states.ContainsKey(type))
            throw new Exception($"{type} State is not registered.");

        currentState?.Exit();
        currentState = states[type];
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.UpdateState();
    }

    public bool IsState<T>() where T : BulletState
    {
        return currentState != null && currentState.GetType() == typeof(T);
    }
}
