using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Character_Visual : MonoBehaviour
{
    private Character owner;

    [SerializeField] private Character_Face face;


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

    // base pose
    private Vector3 bodyBasePos;
    private float bodyBaseZ;
    private Vector3 bodyBaseScale;

    private Vector3 backRingBasePos;
    private float backRingBaseZ;

    private Vector3 frontRingBasePos;
    private float frontRingBaseZ;


    [Header("Flip / Rings Lean")]
    [SerializeField] private float ringLeanAngle = 26f;     // 기울기 각도(도)
    [SerializeField] private float ringLeanDuration = 0.75f;
    [SerializeField] private Ease ringLeanEase = Ease.OutQuint;

    private float ringLeanZ = 0f;
    private Tween ringLeanTween;
    private Dir currentDir = Dir.Left;


    [Header("Wall Push Feedback")]
    [SerializeField] private float pressureBuildTime = 2f;
    [SerializeField] private float maxWallOffsetX = 0.02f;
    [SerializeField, Range(0f, 0.25f)] private float maxSquashRatio = 0.2f;
    [SerializeField, Range(0f, 0.20f)] private float maxSpreadRatio = 0.15f;
    [SerializeField] private float releaseDur = 0.18f;

    private bool isPushingWall = false;
    private int pushingSign = 0;          // Left=-1, Right=+1
    private float pressure = 0f;          // 0..1

    private Tween pressureTween;
    private Tween releaseTween;


    private Coroutine blinkCo;


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
        ringLeanTween?.Kill();
        pressureTween?.Kill();
        releaseTween?.Kill();
        StopBlink();
    }

    private void OnDestroy()
    {
        ringLeanTween?.Kill();
        pressureTween?.Kill();
        releaseTween?.Kill();
        StopBlink();
    }

    public void Bind(Character character)
    {
        owner = character;
    }

    private void LateUpdate()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;

        float wallX = 0f;

        if (pushingSign != 0 && pressure > 0.0001f)
            wallX = pushingSign * maxWallOffsetX * pressure;


        // Body base
        float hoverPhase = (t / hoverDuration) * Mathf.PI * 2f;
        float rotPhase = (t / bodyRotateDuration) * Mathf.PI * 2f;

        Vector3 bPos = bodyBasePos;
        bPos.y += Mathf.Sin(hoverPhase) * hoverAmplitude;

        bPos.x += wallX;
        body.localPosition = bPos;

        float bZ = bodyBaseZ + Mathf.Sin(rotPhase) * bodyRotateAmplitude;
        body.localRotation = Quaternion.Euler(0f, 0f, bZ);

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



        // Rings (hover/rotate + lean)
        float ringRotPhase = (t / ringRotateDuration) * Mathf.PI * 2f;
        float ringHoverPhase = (t / ringHoverDuration) * Mathf.PI * 2f;

        float ringZDelta = Mathf.Sin(ringRotPhase) * ringRotateAmplitude;
        float ringYDelta = Mathf.Sin(ringHoverPhase) * ringHoverAmplitude;

        ApplyRing(backRings, backRingBasePos, backRingBaseZ, ringZDelta, ringYDelta, ringLeanZ, wallX);
        ApplyRing(frontRings, frontRingBasePos, frontRingBaseZ, ringZDelta, ringYDelta, ringLeanZ, wallX);
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

    private void StartBlink()
    {
        StopBlink();
        if (!enableBlink) return;

        blinkCo = StartCoroutine(BlinkLoop());
    }

    private void StopBlink()
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

        // Body 즉시
        if (body != null)
        {
            Vector3 bs = body.localScale;
            float absX = Mathf.Abs(bs.x);
            bs.x = (_dir == Dir.Left) ? absX : -absX;
            body.localScale = bs;
        }

        // Rings lean은 서서히
        float targetLean = (_dir == Dir.Left) ? +ringLeanAngle : -ringLeanAngle;

        ringLeanTween?.Kill();
        ringLeanTween = DOTween.To(() => ringLeanZ, v => ringLeanZ = v, targetLean, ringLeanDuration)
            .SetEase(ringLeanEase);
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

        ringLeanZ = (dir == Dir.Left) ? +ringLeanAngle : -ringLeanAngle;
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


            pushingSign = sign;
            isPushingWall = true;

            bool alreadyBuilding = pressureTween != null && pressureTween.IsActive() && pressureTween.IsPlaying();
            if (alreadyBuilding) return;

            releaseTween?.Kill();
            releaseTween = null;

            pressureTween?.Kill();
            pressureTween = DOTween.To(() => pressure, v => pressure = v, 1f, pressureBuildTime)
                .SetEase(Ease.OutCubic);
        }
        else
        {
            if (!isPushingWall && pressure <= 0.0001f)
                return;

            // 밀기 상태에서 빠져나오는 순간
            if (isPushingWall)
            {
                StartBlink();
                SetFace(FaceExpression.Idle);
            }

            isPushingWall = false;

            pressureTween?.Kill();
            pressureTween = null;

            releaseTween?.Kill();
            releaseTween = DOTween.To(() => pressure, v => pressure = v, 0f, releaseDur)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    pushingSign = 0;
                });
        }
    }

    public void ForceStableVisualState()
    {
        pressureTween?.Kill();
        pressureTween = null;

        releaseTween?.Kill();
        releaseTween = null;

        pressure = 0f;
        isPushingWall = false;
        pushingSign = 0;

        // 정상화
        SetFace(FaceExpression.Idle);
        StartBlink();

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
}

