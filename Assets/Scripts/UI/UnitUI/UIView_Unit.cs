using UnityEngine;

public class UIView_Unit : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    IPlayerData playerData;

    [Header("BulletSocket")]
    // 불릿 슬롯을 관리하는 시스템
    [SerializeField] private BulletSocketSystem bulletSocketSystem;

    // 외부 테스트
    [Header("BulletSocketTest")]
    float temp = 0f;
    int itp = 3;


    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection(IPlayerData _playerData)
    {
        playerData = _playerData;
        // 임시로 시작은 2개 연출중. 나중에 매개변수로 캐릭터 타입 넣어주면 될듯.
        SetBulletSocket();
    }

    public override void Update()
    {
        base.Update();

        Test();
    }

    private void Test()
    {
        temp += Time.deltaTime;

        if (temp > 5f)
        {
            if (itp == 2)
            {
                bulletSocketSystem.SetCount(itp);
                itp = 3;
            }
            else
            {
                bulletSocketSystem.SetCount(itp);
                itp = 2;
            }
            temp = 0f;
        }
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
        bulletSocketSystem.Init(playerData.GetTransform(), 2);
    }
}
