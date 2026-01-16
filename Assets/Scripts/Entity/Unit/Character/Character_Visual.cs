using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Character_Visual : MonoBehaviour
{
    private Character owner;
    private CutsceneComponent cutsceneComponent;

    [SerializeField] private Character_Face face;

    // base pose
    private Vector3 bodyBasePos;
    private float bodyBaseZ;
    private Vector3 bodyBaseScale;

    private Vector3 backRingBasePos;
    private float backRingBaseZ;

    private Vector3 frontRingBasePos;
    private float frontRingBaseZ;


    [Header("Targets")]
    [SerializeField] private Transform body;
    [SerializeField] private Transform backRings;
    [SerializeField] private Transform frontRings;

    [Header("Time")]
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Body Hover")]
    [SerializeField] private float hoverAmplitude = 0.004f;
    [SerializeField] private float hoverDuration = 2.2f;

    [Header("Body Z Rotate")]
    [SerializeField] private float bodyRotateAmplitude = 1.5f;
    [SerializeField] private float bodyRotateDuration = 3.0f;

    [Header("Rings Z Rotate")]
    [SerializeField] private float ringRotateAmplitude = 1f;
    [SerializeField] private float ringRotateDuration = 2.6f;

    [Header("Rings Hover")]
    [SerializeField] private float ringHoverAmplitude = 0.003f;
    [SerializeField] private float ringHoverDuration = 2f;

    [Header("Blink")]
    [SerializeField] private bool enableBlink = true;
    [SerializeField] private Vector2 blinkInterval = new Vector2(3f, 6f);
    [SerializeField] private Vector2 closeHold = new Vector2(0.09f, 0.15f);
    [SerializeField] private Vector2 betweenDoubleBlink = new Vector2(0.07f, 0.08f);
    [SerializeField, Range(0f, 1f)] private float doubleBlinkChance = 0.5f;
    private Coroutine blinkCo;


    [Header("Flip / Rings Lean")]
    [SerializeField] private float ringLeanAngle = 25f;   
    private float ringLeanZ;                              
    private float ringLeanZTarget;                        
    [SerializeField] private float ringLeanTime = 0.2f;  

    private Dir currentDir = Dir.Left;


    [Header("Wall Push Feedback")]
    [SerializeField] private float maxWallOffsetX = 0.03f;
    [SerializeField, Range(0f, 0.25f)] private float maxSquashRatio = 0.2f;
    [SerializeField, Range(0f, 0.20f)] private float maxSpreadRatio = 0.15f;
    private bool isPushingWall = false;
    private int pushingSign = 0;
    private float pressure;
    private float pressureTarget;
    [SerializeField] private float pressureBuildTime = 0.3f;
    [SerializeField] private float releaseDur = 0.04f;


    [Header("Cutscene Lean")]
    private float cutsceneRingLeanZ;        // current
    private float cutsceneRingLeanZTarget;  // target
    [SerializeField] private float cutsceneLeanTime = 0.10f;
    private float cutsceneBodyLeanZ;        // current
    private float cutsceneBodyLeanZTarget;  // target
    [SerializeField] private float cutsceneBodyLeanTime = 0.10f;


    [Header("Move Visual")]
    [SerializeField] private float moveRingLeanExtra = 18f;   // 이동 중 링 추가 기울림(도)
    [SerializeField] private float moveBodyLeanExtra = 6f;    // 이동 중 바디 추가 기울림(도)
    [SerializeField] private float moveLeanTime = 0.12f;      // 이동/정지 수렴 속도(작을수록 빠름)
    private float moveRingLeanZ;        // current
    private float moveRingLeanZTarget;  // target
    private float moveBodyLeanZ;        // current
    private float moveBodyLeanZTarget;  // target



    private void Awake()
    {
        if (body == null) body = transform;

        bodyBasePos = body.localPosition;
        bodyBaseZ = body.localEulerAngles.z;
        bodyBaseScale = body.localScale;

        if (backRings != null)
        {
            backRingBasePos = backRings.localPosition;
            backRingBaseZ = backRings.localEulerAngles.z;
        }

        if (frontRings != null)
        {
            frontRingBasePos = frontRings.localPosition;
            frontRingBaseZ = frontRings.localEulerAngles.z;
        }

        SnapFlip(Dir.Left);
    }

    private void OnEnable()
    {
        StartBlink();
    }

    private void OnDisable()
    {
        StopBlink();
    }

    private void OnDestroy()
    {
        StopBlink();
    }

    public void Bind(Character _character, CutsceneComponent _cutsceneComponent)
    {
        owner = _character;
        cutsceneComponent = _cutsceneComponent;
    }

    private void LateUpdate()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        UpdateSmoothing(dt);

        float wallX = ComputeWallX();

        UpdateBody(t, wallX);
        UpdateRings(t, wallX);
    }

    private void UpdateSmoothing(float dt)
    {
        // 링 기본 lean
        ringLeanZ = ExpSmoothing(ringLeanZ, ringLeanZTarget, dt, ringLeanTime);

        // 컷씬 lean (CutsceneComponent가 target을 갱신)
        cutsceneRingLeanZ = ExpSmoothing(cutsceneRingLeanZ, cutsceneRingLeanZTarget, dt, cutsceneLeanTime);
        cutsceneBodyLeanZ = ExpSmoothing(cutsceneBodyLeanZ, cutsceneBodyLeanZTarget, dt, cutsceneBodyLeanTime);

        // pressure: pushing이면 buildTime, 아니면 releaseDur로 0으로 수렴
        float timeConst = (pressureTarget > pressure) ? pressureBuildTime : releaseDur;
        pressure = ExpSmoothing(pressure, pressureTarget, dt, timeConst);

        moveRingLeanZ = ExpSmoothing(moveRingLeanZ, moveRingLeanZTarget, dt, moveLeanTime);
        moveBodyLeanZ = ExpSmoothing(moveBodyLeanZ, moveBodyLeanZTarget, dt, moveLeanTime);

        // 완전히 풀리면 sign 정리
        if (pressure < 0.0001f && pressureTarget < 0.0001f)
            pushingSign = 0;
    }

    private float ComputeWallX()
    {
        if (pushingSign == 0 || pressure <= 0.0001f) return 0f;
        return pushingSign * maxWallOffsetX * pressure;
    }

    private void UpdateBody(float t, float wallX)
    {
        float hoverPhase = (t / hoverDuration) * Mathf.PI * 2f;
        float rotPhase = (t / bodyRotateDuration) * Mathf.PI * 2f;

        Vector3 bPos = bodyBasePos;
        bPos.y += Mathf.Sin(hoverPhase) * hoverAmplitude;
        bPos.x += wallX;
        body.localPosition = bPos;

        //float bZ = bodyBaseZ + Mathf.Sin(rotPhase) * bodyRotateAmplitude + cutsceneBodyLeanZ;
        float bZ = bodyBaseZ + Mathf.Sin(rotPhase) * bodyRotateAmplitude
         + cutsceneBodyLeanZ + moveBodyLeanZ;

        body.localRotation = Quaternion.Euler(0f, 0f, bZ);

        // 찌부/퍼짐 (pressure 기반)
        float signX = body.localScale.x >= 0f ? 1f : -1f;
        float absBaseX = Mathf.Abs(bodyBaseScale.x);
        float baseY = bodyBaseScale.y;

        float xMul = 1f + (maxSpreadRatio * pressure);
        float yMul = 1f - (maxSquashRatio * pressure);

        Vector3 bs = body.localScale;
        bs.x = signX * absBaseX * xMul;
        bs.y = baseY * yMul;
        bs.z = bodyBaseScale.z;
        body.localScale = bs;
    }

    private void UpdateRings(float t, float wallX)
    {
        float ringRotPhase = (t / ringRotateDuration) * Mathf.PI * 2f;
        float ringHoverPhase = (t / ringHoverDuration) * Mathf.PI * 2f;

        float ringZDelta = Mathf.Sin(ringRotPhase) * ringRotateAmplitude;
        float ringYDelta = Mathf.Sin(ringHoverPhase) * ringHoverAmplitude;

        //float finalRingLean = ringLeanZ + cutsceneRingLeanZ;
        float finalRingLean = ringLeanZ + cutsceneRingLeanZ + moveRingLeanZ;
        ApplyRing(backRings, backRingBasePos, backRingBaseZ, ringZDelta, ringYDelta, finalRingLean, wallX);
        ApplyRing(frontRings, frontRingBasePos, frontRingBaseZ, ringZDelta, ringYDelta, finalRingLean, wallX);
    }
    // 움직임 핵심 함수
    private float ExpSmoothing(float current, float target, float dt, float timeConstant)
    {
        if (timeConstant <= 0f) return target;
        float k = 1f - Mathf.Exp(-dt / timeConstant);
        return Mathf.LerpUnclamped(current, target, k);
    }

    private void ApplyRing(Transform ring, Vector3 basePos, float baseZ, float zDelta, float yDelta, float leanZ, float wallX)
    {
        if (ring == null) return;

        var p = basePos;
        p.y += yDelta;
        p.x += wallX;
        ring.localPosition = p;

        ring.localRotation = Quaternion.Euler(0f, 0f, baseZ + zDelta + leanZ);
    }


    public void SetFace(FaceExpression expr)
    {
        face?.SetExpression(expr);
    }

    public void StartBlink()
    {
        StopBlink();
        if (!enableBlink) return;

        blinkCo = StartCoroutine(BlinkLoop());
    }

    public void StopBlink()
    {
        if (blinkCo != null)
        {
            StopCoroutine(blinkCo);
            blinkCo = null;
        }
        SetFace(FaceExpression.Idle);
    }

    // Blink
    private IEnumerator BlinkLoop()
    {
        SetFace(FaceExpression.Idle);

        while (true)
        {
            float wait = Random.Range(blinkInterval.x, blinkInterval.y);
            yield return useUnscaledTime ? new WaitForSecondsRealtime(wait) : new WaitForSeconds(wait);

            bool doDouble = Random.value < doubleBlinkChance;

            // 1회 깜빡
            SetFace(FaceExpression.Close);
            float hold1 = Random.Range(closeHold.x, closeHold.y);
            yield return useUnscaledTime ? new WaitForSecondsRealtime(hold1) : new WaitForSeconds(hold1);
            SetFace(FaceExpression.Idle);

            if (doDouble)
            {
                float gap = Random.Range(betweenDoubleBlink.x, betweenDoubleBlink.y);
                yield return useUnscaledTime ? new WaitForSecondsRealtime(gap) : new WaitForSeconds(gap);

                SetFace(FaceExpression.Close);
                float hold2 = Random.Range(closeHold.x, closeHold.y);
                yield return useUnscaledTime ? new WaitForSecondsRealtime(hold2) : new WaitForSeconds(hold2);
                SetFace(FaceExpression.Idle);
            }
        }
    }

    public void Flip(Dir _dir)
    {
        if (currentDir == _dir) return;
        currentDir = _dir;

        // body 즉시 반전
        Vector3 bs = body.localScale;
        float absX = Mathf.Abs(bs.x);
        bs.x = (_dir == Dir.Left) ? absX : -absX;
        body.localScale = bs;

        ringLeanZTarget = (_dir == Dir.Left) ? +ringLeanAngle : -ringLeanAngle;
    }

    private void SnapFlip(Dir dir)
    {
        currentDir = dir;

        if (body != null)
        {
            Vector3 bs = body.localScale;
            float absX = Mathf.Abs(bs.x);
            bs.x = (dir == Dir.Left) ? absX : -absX;
            body.localScale = bs;
        }

        float lean = (dir == Dir.Left) ? +ringLeanAngle : -ringLeanAngle;

        ringLeanZ = lean;
        ringLeanZTarget = lean;
    }

    public void SetWallPushing(bool pushing, Dir dir)
    {
        int sign = (dir == Dir.Left) ? -1 : +1;

        if (pushing)
        {
            if (!isPushingWall)
            {
                StopBlink();
                SetFace(FaceExpression.Angry);
            }

            isPushingWall = true;
            pushingSign = sign;
            pressureTarget = 1f;
        }
        else
        {
            if (!isPushingWall && pressure <= 0.0001f) return;

            if (isPushingWall)
            {
                StartBlink();
                SetFace(FaceExpression.Idle);
            }

            isPushingWall = false;
            pressureTarget = 0f;
        }
    }

    public void ForceStableVisualState()
    {
        pressure = 0f;
        isPushingWall = false;
        pushingSign = 0;

        // 정상화
        SetFace(FaceExpression.Idle);
        StartBlink();
        StopMovingVisual();

        // body 스케일 복구
        if (body != null)
        {
            float signX = body.localScale.x >= 0f ? 1f : -1f;
            float absBaseX = Mathf.Abs(bodyBaseScale.x);

            Vector3 bs = body.localScale;
            bs.x = signX * absBaseX;
            bs.y = bodyBaseScale.y;
            bs.z = bodyBaseScale.z;
            body.localScale = bs;
        }
    }

    public void SetCutsceneLeanTargets(float ringLeanTarget, float bodyLeanTarget)
    {
        cutsceneRingLeanZTarget = ringLeanTarget;
        cutsceneBodyLeanZTarget = bodyLeanTarget;
    }

    public void ClearCutsceneLeanTargets()
    {
        cutsceneRingLeanZTarget = 0f;
        cutsceneBodyLeanZTarget = 0f;
    }

    public void MovingVisual(Dir dir)
    {
        float sign = (dir == Dir.Left) ? +1f : -1f;

        moveRingLeanZTarget = sign * moveRingLeanExtra;
        moveBodyLeanZTarget = sign * moveBodyLeanExtra;
    }

    public void StopMovingVisual()
    {
        moveRingLeanZTarget = 0f;
        moveBodyLeanZTarget = 0f;
    }
}

