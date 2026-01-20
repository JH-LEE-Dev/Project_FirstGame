using UnityEngine;
using System.Threading.Tasks;

public abstract class PlayerMoveStrategy : ScriptableObject
{
    protected Unit unit;
    protected IOrbitPathProvider orbitPathProvider;
    protected IMoveSignalHandler moveSignalHandler;


    [Header("Normalized path value (0~1)")]
    [SerializeField, Range(0f, 1f)]
    protected float movingValue = 0.5f;

    [Header("Speed")]
    [SerializeField] protected float maxSpeedPerSec = 0.5f;

    [Header("Feel (seconds)")]
    [SerializeField] protected float accelTime = 0.05f; // 출발 감각
    [SerializeField] protected float decelTime = 0.05f; // 정지 감각

    [Header("Feel Curve")]
    [SerializeField] protected AnimationCurve accelCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] protected AnimationCurve decelCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // 내부 상태
    protected float inputAxis;     // -1, 0, +1
    protected float currentSpeed;  // 현재 속도
    protected float targetSpeed;   // 목표 속도


    //Initialize함수.
    public abstract void Initialize(Unit unit, IOrbitPathProvider orbitPathProvider, IMoveSignalHandler moveSignalHandler);

    //움직임 함수. 실질적으로 매 프레임마다 호출됨.
    public abstract void Move(Vector2 direction);


    // 캐릭터의 모든 움직임 리셋 및 센터로 이동
    public void ResetCharacterPosition()
    {
        inputAxis = currentSpeed = targetSpeed = 0f;
        movingValue = 0.5f;
        unit.transform.position = orbitPathProvider.GetPathPosition(movingValue);
    }

    // 레일의 센터 위치를 받을 수 있는 기능
    public Vector3 GetCharacterResetPosition()
    {
        return orbitPathProvider.GetPathPosition(0.5f);
    }

    // movingValue 값에 따라 위치 시켜주는 핵심 함수
    protected void ApplyPosition()
    {
        unit.transform.position = orbitPathProvider.GetPathPosition(movingValue);
    }
}