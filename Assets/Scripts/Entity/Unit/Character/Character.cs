using System;
using UnityEngine;

public class Character : Unit, ICharacterData
{
    /// <summary>
    /// 시스템 속성 존.----------------------------------
    /// </summary>
    public ICombatEffectReceiver combatEffectReceiver => combatComponent;
    public event Action PlayerAttackFinishedEvent;

    private CombatComponent combatComponent;

    [Header("aim Object")]
    private LineRenderer lineRenderer;
    [SerializeField] private float aimLength = 10f;




    /// <summary>
    /// 구현 속성 존 ------------------------------------------
    /// </summary>


    private Vector2 mousePos;
    private Vector2 fireDir;

    [SerializeField] private Character_Visual character_Visual;






    /// <summary>
    ///  시스템 코드 존.-----------------------------------------
    /// </summary>

    protected override void Awake()
    {
        base.Awake();

        combatComponent = GetComponent<CombatComponent>();  
    }

    public void Initialize_Character(InputManager _inputManager, GameServiceLocator _gameServiceLocator)
    {
        base.Initialize(_inputManager, _gameServiceLocator);

        lineRenderer = GetComponent<LineRenderer>();

        BindEvent();
        character_Visual.Bind(this);
    }

    private void BindEvent()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.MoveEvent += OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetMousePos;
        inputManager.inputReader.PointerPositionEvent += SetMousePos;
        inputManager.inputReader.FireButtonPressedEvent -= Fire;
        inputManager.inputReader.FireButtonPressedEvent += Fire;

        combatComponent.BulletEffectIsFinishedEvent -= PlayerAttackFinished;
        combatComponent.BulletEffectIsFinishedEvent += PlayerAttackFinished;
    }

    private void ReleaseEvent()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetMousePos;
        inputManager.inputReader.FireButtonPressedEvent -= Fire;
        combatComponent.BulletEffectIsFinishedEvent -= PlayerAttackFinished;
    }
    protected override void OnDestroy()
    {
        ReleaseEvent();
    }

    //bCanAction이 True일 때만 캐릭터가 움직이거나 발사할 수 있음.
    //이 여부는 상위 시스템에 의해 결정.
    public override void SetbCanAction()
    {
        lineRenderer.enabled = true;
        bCanAction = true;
    }

    public override void ResetbCanAction()
    {
        lineRenderer.enabled = false;
        bCanAction = false;
    }


    /// <summary>
    /// 구현 코드 존.--------------------------------------------
    /// </summary>

    protected override void Update()
    {
        base.Update();

        UpdateAimLine();


    }

    //데미지 입는 함수 - 미구현.
    public override void TakeDamage(float damage)
    {

    }

    // 캐릭터 조준선 그리는 함수
    public void UpdateAimLine()
    {
        if (bCanAction == false)
            return;

        Camera mainCam = gameServiceLocator.GetMainCamera();
        Vector2 origin = transform.position;

        Vector2 mouseWorldPos =
            mainCam.ScreenToWorldPoint(mousePos);

        Vector2 dir = (mouseWorldPos - origin).normalized;
        fireDir = dir;

        Vector2 endPos = origin + dir * aimLength;

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
    }

    //마우스 좌표 받아오는 함수 -> 마우스 좌표가 바뀔 때마다 호출됨.
    public void SetMousePos(Vector2 move)
    {
        mousePos = move;
    }

    //발사하는 함수, 공격턴에 마우스 좌클릭을 누르면 호출됨.
    private void Fire()
    {
        if (bCanAction == true)
        {
            //발사가 끝나면 움직임을 제한해야 하므로 zero를 넣어줌.
            moveComponent.SetMoveDirection(Vector2.zero);
            bCanAction = false;

            //combatComponent에서 실질적인 발사를 함.
            combatComponent.Fire(fireDir);

            //Sound.Play("Fire", transform.position);
        }
    }

    //combatComponent에서 총알의 공격 작업이 모두 끝나면 호출됨.
    //그리고 이 신호를 상위 모듈로 전파함.
    private void PlayerAttackFinished()
    {
        bCanAction = false;
        PlayerAttackFinishedEvent?.Invoke();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetMaxHealth()
    {
        return healthComponent.GetMaxHealth();
    }

    public float GetCurrentHealth()
    {
        return healthComponent.GetCurrentHealth();
    }
}
