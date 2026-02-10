using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletStateMachine : MonoBehaviour
{
    //외부 의존성
    ICharacterStatProvider characterStatProvider;
    IBulletEffectProvider bulletEffectProvider;
    Bullet bullet;

    private BulletState currentState;
    private Dictionary<Type, BulletState> states = new Dictionary<Type, BulletState>();
    [SerializeField] private List<BulletState> bulletStates = new List<BulletState>(3);
    private BulletStateCtx ctx;

    public void Initialize(ICharacterStatProvider _characterStatProvider,IBulletEffectProvider _bulletEffectReceiver,
        Bullet _bullet)
    {
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectReceiver;
        bullet = _bullet;

        ctx = new BulletStateCtx();
        ctx.Initialize(this,characterStatProvider,bulletEffectProvider,bullet);

        for(int i = 0;i< bulletStates.Count;++i)
        {
            bulletStates[i].Initialize(ctx);
            states[bulletStates[i].GetType()] = bulletStates[i];
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
