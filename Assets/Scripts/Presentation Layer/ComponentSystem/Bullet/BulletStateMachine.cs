using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletStateMachine : MonoBehaviour
{
    //외부 의존성
    private ICharacterStatProvider characterStatProvider;
    private IBulletEffectProvider bulletEffectProvider;
    private IDamageSystem damageSystem;
    private BulletBehaviorData bulletBehaviorData;

    private BulletState currentState;
    private Dictionary<Type, BulletState> states = new Dictionary<Type, BulletState>();
    private BulletStateCtx ctx;

    public void Initialize(ICharacterStatProvider _characterStatProvider,
        IBulletEffectProvider _bulletEffectReceiver, IDamageSystem _damageSystem,
        Bullet _bullet, BulletBehaviorData _data)
    {
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectReceiver;
        damageSystem = _damageSystem;
        bulletBehaviorData = _data;

        ctx = new BulletStateCtx();
        ctx.Initialize(this, characterStatProvider, bulletEffectProvider, bulletBehaviorData, _bullet);

        BS_BeforeFire bs_BeforeFire = new BS_BeforeFire();
        BS_Fly bs_Fly = new BS_Fly();
        BS_Hit bs_Hit = new BS_Hit();
        bs_BeforeFire.Initialize(ctx);
        bs_Fly.Initialize(ctx);
        bs_Hit.Initialize(ctx);

        states[bs_BeforeFire.GetType()] = bs_BeforeFire;
        states[bs_Fly.GetType()] = bs_Fly;
        states[bs_Hit.GetType()] = bs_Hit;
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
