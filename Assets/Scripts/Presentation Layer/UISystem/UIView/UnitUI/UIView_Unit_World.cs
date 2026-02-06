using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIView_Unit_World : UIView
{
    public event Action<int> UnEquipBulletCardEvent;
    public event Action CancelCardPreviewEvent;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    ICharacterData characterData;
    IReadOnlyList<IEnemyData> enemyDatas;


    [Header("Systems")]
    [SerializeField] private GameObject bulletSocketSystemPrefab;
    private BulletSocketSystem bulletsocketSystem;

    [SerializeField] private GameObject clickCatchSystemPrefab;
    private ClickCatchSystem clickCatchSystem;

    [SerializeField] private GameObject bulletLineSystemPrefab;
    private BulletLineSystem bulletLineSystem;


    [Header("DamageNumSystem Settings")]
    [SerializeField] private DamageNumberSystem damageNumberSystem;


    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection(ICharacterData _characterData,IReadOnlyList<IEnemyData> _enemyDatas)
    {
        characterData = _characterData;
        enemyDatas = _enemyDatas;

        InitializeBulletSocketSystem();
        InitializeClickCatchSystem();
        InitializeBulletLineSystem();
    }

    private void InitializeBulletSocketSystem()
    {
        GameObject go = Instantiate(bulletSocketSystemPrefab, characterData.GetTransform());
        bulletsocketSystem = go.GetComponent<BulletSocketSystem>();

        // 임시 2개
        bulletsocketSystem.Init(this, viewCtx.cardLocalizationSystem);
    }

    private void InitializeClickCatchSystem()
    {
        GameObject go = Instantiate(clickCatchSystemPrefab, this.transform);
        clickCatchSystem = go.GetComponent<ClickCatchSystem>();

        clickCatchSystem.Init(this);
    }

    private void InitializeBulletLineSystem()
    {
        GameObject go = Instantiate(bulletLineSystemPrefab, this.transform);
        bulletLineSystem = go.GetComponent<BulletLineSystem>();
        bulletLineSystem.Init(this, characterData.GetTransform(), viewCtx.inputManager);
    }


    public override void Update()
    {
        base.Update();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }




    // For ClickCatchSystem

    public void CancelPreview()
    {
        CancelCardPreviewEvent?.Invoke();
    }

    // For BulletLine

    public void SetAiming(bool aiming)
    {
        bulletLineSystem?.SetAiming(aiming);
    }



    // For BulletSocketSystem

    public Transform GetSocketTransform(int _index)
    {
        if (bulletsocketSystem == null) return null;

        return bulletsocketSystem.GetSocketTransform(_index);
    }

    public void SetBulletSocketCount(int _count)
    {
        bulletsocketSystem?.SetCount(_count);
    }

    public void EquipBulletCard(int _index, ICardDataInstanceProvider _data = null)
    {
        bulletsocketSystem?.EquipBulletCard(_index, _data);
    }

    // 이쪽에서 장착을 취소하면, 이게 호출됨. UIView_CardSystem에서 UnEquipBulletCard가 불려야함.
    public void UnEquipBulletCard(int _index)
    {
        UnEquipBulletCardEvent?.Invoke(_index);
    }

    // 쏠때 이거 불러주면 소켓 카드 전부 초기화.
    public void UnEquipBulletCardForShoot()
    {
        bulletsocketSystem?.UnEquipBulletCardForShoot();
    }

    public void EnemyTakeDamage(IEnemyData _enemyData, float _damage, bool bCritical)
    {
        damageNumberSystem?.SpawnBasicDamageNumber(_damage, bCritical, _enemyData.GetTransform().position);
    }
}
