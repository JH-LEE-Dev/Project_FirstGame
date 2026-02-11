using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PCombatComponent : CombatComponent, IBulletEffectReceiver, IBulletEffectProvider
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    //이벤트
    public event Action AttackFinishedEvent;

    //외부 의존성
    ICharacterStatProvider characterStatProvider;
    private AttackComponent attackComponent;

    //내부 의존성
    private DamageCalcComponent damageCalcComponent;


    //인터페이스 선언부
    IReadOnlyDictionary<BulletElementType, BulletElementData> IBulletEffectProvider.currentEffectElements => currentEffectElements;
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> IBulletEffectProvider.currentDebuffElementTypes => currentDebuffElementTypes;

    private BulletType bulletType;
    private bool bUpgraded = false;
    BulletType IBulletEffectProvider.bulletType => bulletType;
    bool IBulletEffectProvider.bUpgraded => bUpgraded;



    protected Dictionary<DebuffElementEffectType, DebuffElementData> currentDebuffElementTypes =
        new Dictionary<DebuffElementEffectType, DebuffElementData>(SYSTEM_VAR.maxDebuffElementCount);

    protected Dictionary<BulletElementType, BulletElementData> currentEffectElements =
        new Dictionary<BulletElementType, BulletElementData>(SYSTEM_VAR.maxDebuffElementCount);



    /// <summary>
    /// 구현 속성 존. ---------------------------------------------
    /// </summary>











    /// <summary>
    /// 시스템 코드 존. ---------------------------------------------
    /// </summary>

    public void Initialize(UnitContext _ctx, ICombatSignalHandler _combatSignalHandler, ICharacterStatProvider _characterStatProvider,
        DamageCalcComponent _damageCalcComponent,AttackComponent _attackComponent)
    {
        base.Initialize(_ctx, _combatSignalHandler);

        damageCalcComponent = _damageCalcComponent;
        characterStatProvider = _characterStatProvider;
        attackComponent = _attackComponent;

        BindEvent();
    }

    public void ResetComponent()
    {
        ResetBulletType();
        currentEffectElements.Clear();
        currentDebuffElementTypes.Clear();
    }

    protected override void Awake()
    {
    }

    private void BindEvent()
    {
        attackComponent.AttackFinishedEvent -= AttackFinished;
        attackComponent.AttackFinishedEvent += AttackFinished;
    }

    private void ReleaseEvent()
    {
        attackComponent.AttackFinishedEvent -= AttackFinished;
    }

    protected override void OnDestroy()
    {
        ReleaseEvent();
        AttackFinishedEvent = null;
    }

    public virtual void Fire(Vector2 dir)
    {
        attackComponent.Fire(bulletType, 1, dir, transform.position);
    }

    public void AttackFinished()
    {
        AttackFinishedEvent?.Invoke();
        ResetBulletType();
    }

    public void SetBulletType(BulletType _type, bool _bUpgraded)
    {
        bulletType = _type;
        bUpgraded = _bUpgraded;
    }

    public void ResetBulletType()
    {
        bulletType = BulletType.Normal;
        bUpgraded = false;
    }

    public void ApplyBulletElementType(BulletElementData _effectElementData)
    {
        if (currentEffectElements.ContainsKey(_effectElementData.bulletElementType))
        {
            var data = currentEffectElements[_effectElementData.bulletElementType];
            data.nestingCnt += _effectElementData.nestingCnt;
            currentEffectElements[_effectElementData.bulletElementType] = data;
        }
        else
        {
            currentEffectElements[_effectElementData.bulletElementType] = _effectElementData;
        }
    }

    public void UndoBulletElementApply(BulletElementData _effectElementData)
    {
        if (currentEffectElements[_effectElementData.bulletElementType].nestingCnt > _effectElementData.nestingCnt)
        {
            var data = currentEffectElements[_effectElementData.bulletElementType];
            data.nestingCnt -= _effectElementData.nestingCnt;
            currentEffectElements[_effectElementData.bulletElementType] = data;
        }
        else
        {
            currentEffectElements.Remove(_effectElementData.bulletElementType);
        }
    }

    public void ApplyDebuffElementType(DebuffElementData _debuffElementData)
    {
        if (currentDebuffElementTypes.ContainsKey(_debuffElementData.debuffElementType))
        {
            var data = currentDebuffElementTypes[_debuffElementData.debuffElementType];
            data.turnCnt += _debuffElementData.turnCnt;
            currentDebuffElementTypes[_debuffElementData.debuffElementType] = data;
        }
        else
        {
            currentDebuffElementTypes[_debuffElementData.debuffElementType] = _debuffElementData;
        }
    }

    public void UndoDebuffElementApply(DebuffElementData _debuffElementData)
    {
        if (currentDebuffElementTypes[_debuffElementData.debuffElementType].turnCnt > _debuffElementData.turnCnt)
        {
            var data = currentDebuffElementTypes[_debuffElementData.debuffElementType];
            data.turnCnt -= _debuffElementData.turnCnt;
            currentDebuffElementTypes[_debuffElementData.debuffElementType] = data;
        }
        else
        {
            currentDebuffElementTypes.Remove(_debuffElementData.debuffElementType);
        }
    }

    /// <summary>
    /// 구현 코드 존. ----------------------------------------------------
    /// </summary>

    protected override void FixedUpdate()
    {

    }

    protected override void Update()
    {

    }

    protected override void Start()
    {

    }
}
