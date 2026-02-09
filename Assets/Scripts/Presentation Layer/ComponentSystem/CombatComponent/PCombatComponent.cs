using System;
using System.Collections.Generic;
using UnityEngine;

public class PCombatComponent : CombatComponent, IBulletEffectReceiver, IBulletEffectProvider
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    //이벤트
    public event Action BulletEffectIsFinishedEvent;

    //외부 의존성
    ICharacterStatProvider characterStatProvider;

    //인터페이스 선언부
    List<BulletElementData> IBulletEffectProvider.currentBulletElementTypes => currentEffectElements;
    List<DebuffElementData> IBulletEffectProvider.currentDebuffElementTypes => throw new NotImplementedException();

    [SerializeField] private Bullet bulletPrefab;
    private Bullet bulletObject;
    private BulletType bulletType;
    private bool bUpgraded = false;
    BulletType IBulletEffectProvider.bulletType => bulletType;
    bool IBulletEffectProvider.bUpgraded => bUpgraded;


    private List<BulletElementData> currentEffectElements = new List<BulletElementData>(SYSTEM_VAR.maxDebuffElementCount);
    private List<DebuffElementData> currentDebuffElementTypes = new List<DebuffElementData>(SYSTEM_VAR.maxDebuffElementCount);

    /// <summary>
    /// 구현 속성 존. ---------------------------------------------
    /// </summary>











    /// <summary>
    /// 시스템 코드 존. ---------------------------------------------
    /// </summary>

    public void Initialize(UnitContext _ctx, ICombatSignalHandler _combatSignalHandler, ICharacterStatProvider _characterStatProvider)
    {
        base.Initialize(_ctx, _combatSignalHandler);

        characterStatProvider = _characterStatProvider;

        bulletObject = Instantiate(bulletPrefab, transform);
        bulletObject.gameObject.SetActive(false);
        bulletObject.Initialize(characterStatProvider,this);

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
        bulletObject.BulletEffectIsFinishedEvent -= BulletEffectIsFinished;
        bulletObject.BulletEffectIsFinishedEvent += BulletEffectIsFinished;
    }

    private void ReleaseEvent()
    {
        bulletObject.BulletEffectIsFinishedEvent -= BulletEffectIsFinished;
    }

    protected override void OnDestroy()
    {
        ReleaseEvent();
        BulletEffectIsFinishedEvent = null;
    }

    public virtual void Fire(Vector2 dir)
    {
        bulletObject.transform.position = transform.position;
        bulletObject.gameObject.SetActive(true);
        bulletObject.Fire(dir);
    }

    public void BulletEffectIsFinished()
    {
        BulletEffectIsFinishedEvent?.Invoke();
        ResetBulletType();
    }

    public void SetBulletType(BulletType _type, bool bUpgraded)
    {
        bulletType =_type;
    }

    public void ResetBulletType()
    {
        bulletType = BulletType.Normal;
        bUpgraded = false;
    }

    public void ApplyBulletElementType(BulletElementData _effectElementData)
    {
        currentEffectElements.Add(_effectElementData);
    }

    public void UndoBulletElementApply(BulletElementData _effectElementData)
    {
        currentEffectElements.Remove(_effectElementData);
    }

    public void ApplyDebuffElementType(DebuffElementData _debuffElementData)
    {
        currentDebuffElementTypes.Add(_debuffElementData);
    }

    public void UndoDebuffElementApply(DebuffElementData _debuffElementData)
    {
        currentDebuffElementTypes.Remove(_debuffElementData);
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
