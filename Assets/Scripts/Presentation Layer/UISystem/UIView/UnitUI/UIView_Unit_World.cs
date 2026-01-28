using UnityEngine;
using System;
using System.ComponentModel.Design;
using NaughtyAttributes;

public class UIView_Unit_World : UIView
{
    public event Action<int> UnEquipBulletCardEvent;
    public event Action CancelCardPreviewEvent;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    ICharacterData characterData;


    [Header("Systems")]
    [SerializeField] private GameObject bulletSocketSystemPrefab;
    private BulletSocketSystem bulletsocketSystem;

    [SerializeField] private GameObject clickCatchSystemPrefab;
    private ClickCatchSystem clickCatchSystem;

    [Header("DamageNumSystem Settings")]
    [SerializeField] private DamageNumberSystem damageNumberSystem;


    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection()
    {
       
    }

    public void Initialize(ICharacterData _characterData)
    {
        characterData = _characterData;

        InitializeBulletSocketSystem();
        InitializeClickCatchSystem();
    }

    private void InitializeBulletSocketSystem()
    {
        GameObject go = Instantiate(bulletSocketSystemPrefab, characterData.GetTransform());
        bulletsocketSystem = go.GetComponent<BulletSocketSystem>();

        // 임시 2개
        bulletsocketSystem.Init(2, this);
    }

    private void InitializeClickCatchSystem()
    {
        GameObject go = Instantiate(clickCatchSystemPrefab, this.transform);
        clickCatchSystem = go.GetComponent<ClickCatchSystem>();

        clickCatchSystem.Init(this);
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



    // For BulletSocketSystem

    public Transform GetSocketTransform(int _index)
    {
        if (bulletsocketSystem == null) return null;

        return bulletsocketSystem.GetSocketTransform(_index);
    }


    // 인게임 중에, 플레이어의 카드 슬롯 개수를 변경해주는 함수. 아직은 필요가 없다.
    public void SetBulletSocketCount(int _count)
    {
        bulletsocketSystem?.SetCount(_count);
    }

    public void EquipBulletCard(int _index, CardDataInstance _data = null)
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

    public void EnemyTakeDamage(IEnemyData _enemyData,float _damage)
    {
        Debug.Log("적이 데미지를 입었습니다.");
        damageNumberSystem?.SpawnBasicDamageNumber(_damage, _enemyData.GetTransform().position);
    }
}
