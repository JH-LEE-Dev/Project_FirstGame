using UnityEngine;


[CreateAssetMenu(menuName = "Strategy/Move/PlayerNormalMove")]
public class PlayerNormalMoveStrategy : PlayerMoveStrategy
{
    [Header("Normalized path value (0~1)")]
    [SerializeField, Range(0f, 1f)]
    private float movingValue = 0.5f;

    [Header("Speed")]
    [SerializeField] private float maxSpeedPerSec = 0.5f;

    [Header("Feel (seconds)")]
    [SerializeField] private float accelTime = 0.05f; // 출발 감각
    [SerializeField] private float decelTime = 0.05f; // 정지 감각

    [Header("Feel Curve")]
    [SerializeField] private AnimationCurve accelCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve decelCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // 내부 상태
    private float inputAxis;     // -1, 0, +1
    private float currentSpeed;  // 현재 속도
    private float targetSpeed;   // 목표 속도


    public override void Initialize(Unit _unit, IOrbitPathProvider _orbitPathProvider)
    {
        unit = _unit;
        orbitPathProvider = _orbitPathProvider;
        ApplyPosition();
    }

    private void ApplyPosition()
    {
        unit.transform.position = orbitPathProvider.GetPathPosition(movingValue);
    }

    public override void Move(Vector2 direction)
    {
        if (unit == null || orbitPathProvider == null)
            return;

        float dt = Time.deltaTime;

        inputAxis = -Mathf.Clamp(direction.x, -1f, 1f);

        targetSpeed = inputAxis * maxSpeedPerSec;

        if (Mathf.Approximately(movingValue, 0f) && targetSpeed < 0f)
            targetSpeed = 0f;
        if (Mathf.Approximately(movingValue, 1f) && targetSpeed > 0f)
            targetSpeed = 0f;

        bool stopping = Mathf.Approximately(targetSpeed, 0f);
        float timeConstant = stopping ? decelTime : accelTime;

        float lerpFactor = 1f - Mathf.Exp(-dt / Mathf.Max(timeConstant, Mathf.Epsilon));
        float curved = stopping ? decelCurve.Evaluate(lerpFactor) : accelCurve.Evaluate(lerpFactor);

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, curved);

        movingValue += currentSpeed * dt;

        float clamped = Mathf.Clamp01(movingValue);
        if (!Mathf.Approximately(clamped, movingValue))
        {
            movingValue = clamped;
            currentSpeed = 0f;
        }
        else
        {
            movingValue = clamped;
        }

        ApplyPosition();
    }
}
