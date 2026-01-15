using UnityEngine;

public class UIView_Unit : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    ICharacterData characterData;

    [Header("BulletSocket")]
    // 불릿 슬롯을 관리하는 시스템
    [SerializeField] private BulletSocketSystem bulletSocketSystem;


    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection()
    {
       
    }

    public void Initialize(ICharacterData _characterData)
    {
        Debug.Log("1");
        characterData = _characterData;
        // 임시로 시작은 2개 연출중. 나중에 매개변수로 캐릭터 타입 넣어주면 될듯.
        SetBulletSocket();
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

    private void SetBulletSocket()
    {
        // 임시로 시작은 2개
        bulletSocketSystem.Init(characterData.GetTransform(), 2);
    }
}
