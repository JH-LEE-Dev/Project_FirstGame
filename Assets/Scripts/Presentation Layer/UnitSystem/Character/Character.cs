using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : Unit, ICharacterData
{
    /// <summary>
    /// 시스템 속성 존.----------------------------------
    /// </summary>
    
    //이벤트.
    public event Action PlayerAttackEvent;
    public event Action PlayerAttackFinishedEvent;
    public event Action CharacterStatChangedEvent;

    //인터페이스 선언부.
    public ICombatEffectReceiver combatEffectReceiver => statComponent;
    public IBulletEffectReceiver bulletEffectReceiver => combatComponent;

    //외부 의존성
    IOrbitPathProvider orbitPathProvider;

    //내부 의존성
    PVisualComponentCoordinator visualComponentCoordinator; //Visual 로직 통신을 담당하는 객체.
    private PMoveComponent moveComponent;
    private PCombatComponent combatComponent;
    private CutsceneComponent cutsceneComponent;
    private PStatComponent statComponent;
    private DamageCalcComponent damageCalcComponent;
    private AttackComponent attackComponent;






    /// <summary>
    /// 구현 속성 존 ------------------------------------------
    /// </summary>

    public CharacterType characterType {  get; private set; }
    public bool bCanAttack { get; private set; }
    private Vector2 mousePos;
    private Vector2 fireDir;

    [SerializeField] private Character_Visual character_Visual;







    /// <summary>
    ///  시스템 코드 존.-----------------------------------------
    /// </summary>

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize_Character(InputManager _inputManager, IOrbitPathProvider _orbitPathProvider, GameServiceLocator _gameServiceLocator)
    {
        base.Initialize(_inputManager, _gameServiceLocator);

        orbitPathProvider = _orbitPathProvider;

        combatComponent = GetComponent<PCombatComponent>();
        moveComponent = GetComponent<PMoveComponent>();
        cutsceneComponent = GetComponent<CutsceneComponent>();
        visualComponentCoordinator = new PVisualComponentCoordinator();
        statComponent = GetComponent<PStatComponent>();
        damageCalcComponent = new DamageCalcComponent();
        attackComponent = GetComponent<AttackComponent>();

        //Visual 로직에 필요한 의존성을 추가해주면 됨.
        visualComponentCoordinator.Initialize(character_Visual, combatComponent, moveComponent, cutsceneComponent);
        moveComponent.Initialize(ctx, orbitPathProvider, visualComponentCoordinator);
        combatComponent.Initialize(ctx, visualComponentCoordinator,statComponent, damageCalcComponent,attackComponent);
        cutsceneComponent?.Initialize(this, visualComponentCoordinator, orbitPathProvider, character_Visual);
        damageCalcComponent.Initialize(statComponent, combatComponent);
        attackComponent.Initialize(statComponent, combatComponent, damageCalcComponent);
        statComponent.Initialize();

        BindEvent();

        bCanAttack = false;
        characterType = CharacterType.Rumy;

        character_Visual?.Bind(this, cutsceneComponent);
    }

    private void BindEvent()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.MoveEvent += OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetMousePos;
        inputManager.inputReader.PointerPositionEvent += SetMousePos;
        inputManager.inputReader.FireButtonPressedEvent -= Fire;
        inputManager.inputReader.FireButtonPressedEvent += Fire;

        combatComponent.AttackFinishedEvent -= PlayerAttackFinished;
        combatComponent.AttackFinishedEvent += PlayerAttackFinished;
    }

    private void ReleaseEvent()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetMousePos;
        inputManager.inputReader.FireButtonPressedEvent -= Fire;
        combatComponent.AttackFinishedEvent -= PlayerAttackFinished;
    }

    public bool IsCutScene()
    {
        if (cutsceneComponent == null) return false;

        return cutsceneComponent.IsCutscene;
    }

    protected override void OnDestroy()
    {
        ReleaseEvent();
        PlayerAttackFinishedEvent = null;
    }

    //bCanAction이 True일 때만 캐릭터가 움직이거나 발사할 수 있음.
    //이 여부는 상위 시스템에 의해 결정.
    public override void SetbCanAction()
    {
        bCanAction = true;
        PlayerAttackTurnStarted();
    }

    public override void ResetbCanAction()
    {
        bCanAction = false;
    }

    public void SetbCanAttack(bool boolean)
    {
        bCanAttack = boolean;
    }

    //입력 시스템에 의해서 호출되는 움직임 함수.
    private void OnMove(Vector2 move)
    {
        //시스템에 의해 플레이어가 공격 가능한 턴/타이밍에만 실행되게 적용.
        if (bCanAction == false)
        {
            moveComponent.SetMoveDirection(Vector2.zero);
            return;
        }

        //키보드 <-, -> 에 따른 이동 방향임. Vector2(1,0) Vector2(-1,0)
        moveDirection = move;
        moveComponent.SetMoveDirection(moveDirection);
    }

    //데미지 입는 함수 - 미구현.
    public override void TakeDamage(float damage, bool bCritical, Vector2 pos, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null)
    {

    }

    public float GetMaxHealth()
    {
        return healthComponent.GetMaxHealth();
    }

    public float GetCurrentHealth()
    {
        return healthComponent.GetCurrentHealth();
    }

    //combatComponent에서 총알의 공격 작업이 모두 끝나면 호출됨.
    //그리고 이 신호를 상위 모듈로 전파함.
    private void PlayerAttackFinished()
    {
        combatComponent.ResetComponent();
        statComponent.DecreaseAttackCnt();

        if (statComponent.attackCnt == 0)
        {
            PlayerAttackFinishedEvent?.Invoke();
            statComponent.ResetStat();
            CharacterStatChangedEvent?.Invoke();
        }
        else
        {
            bCanAction = true;
        }
    }



    /// <summary>
    /// 구현 코드 존.--------------------------------------------
    /// </summary>

    protected override void Update()
    {
        base.Update();

        UpdateAimLine();
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
    }

    //마우스 좌표 받아오는 함수 -> 마우스 좌표가 바뀔 때마다 호출됨.
    public void SetMousePos(Vector2 move)
    {
        mousePos = move;
    }

    //발사하는 함수, 공격턴에 마우스 좌클릭을 누르면 호출됨.
    private void Fire()
    {
        if (bCanAction == false)
            return;

        if (bCanAttack)
        {
            //발사가 끝나면 움직임을 제한해야 하므로 zero를 넣어줌.
            moveComponent.SetMoveDirection(Vector2.zero);
            bCanAction = false;

            //combatComponent에서 실질적인 발사를 함.
            combatComponent.Fire(fireDir);
            PlayerAttackEvent?.Invoke();

            //Sound.Play("Fire", transform.position);
        }
        else
        {
            Debug.LogWarning("고유 카드가 장착되지 않으면 발사할 수 없습니다.");
            combatComponent.AttackFinished();
            return;
        }
    }








    // YW 구현존

    // 상황에 따른 컷씬 동작 수행. (컷씬중에는 움직일 수 없게끔 장치를 해두었음.)
    public void PlayCutscene(CutsceneSignal _signal)
    {
        switch (_signal)
        {
            // 카드 드로우 되었을 때 해주면 됨.
            case CutsceneSignal.TurnStart_Start:
                cutsceneComponent.TurnStart();
                break;


            // Turn 종료 버튼을 눌렀을 때 해주면 됨.
            case CutsceneSignal.TurnEnd_Start:
                cutsceneComponent.TurnEnd();
                break;

        }
    }

    public void PlayerTurnStarted()
    {
        PlayCutscene(CutsceneSignal.TurnStart_Start);
    }

    public void PlayerAttackTurnStarted()
    {
        PlayCutscene(CutsceneSignal.TurnEnd_Start);
    }

    public ICharacterStatProvider GetStatProvider()
    {
        return statComponent;
    }

    public IBulletEffectProvider GetBulletEffectProvider()
    {
        return combatComponent;
    }
}
