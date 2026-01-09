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


    [Header("Hover")]
    [SerializeField] private float hoverAmplitude = 0.0075f; 
    [SerializeField] private float hoverDuration = 2.2f;
    [SerializeField] private Ease hoverEase = Ease.InOutSine;


    [Header("Sway")]
    [SerializeField] private float bodyRotateDeg = 2.0f;
    [SerializeField] private float bodyRotateDuration = 3.0f;

    [Header("Rings")]
    [SerializeField] private float ringWobbleDeg = 3.5f;       // 링만 따로 흔들면 더 귀여움
    [SerializeField] private float ringWobbleDuration = 2.4f;


    [Header("Misc")]
    [SerializeField] private bool useUnscaledTime = false;     // 타임스케일 0에서도 떠있게 할지


    [Header("Blink")]
    [SerializeField] private bool enableBlink = true;   // 눈 깜빡임
    [SerializeField] private Vector2 blinkInterval = new Vector2(3f, 6f); // 대기시간
    [SerializeField] private Vector2 closeHold = new Vector2(0.09f, 0.15f); // 눈 감는 시간
    [SerializeField] private Vector2 betweenDoubleBlink = new Vector2(0.07f, 0.08f); // 두번 깜빡 사이
    [SerializeField, Range(0f, 1f)] private float doubleBlinkChance = 0.5f; // 두번 깜빡 확률



    private Vector3 bodyBaseLocalPos;
    private float bodyBaseLocalZ;

    private Vector3 backRingBaseLocalPos;
    private float backRingBaseLocalZ;

    private Vector3 frontRingBaseLocalPos;
    private float frontRingBaseLocalZ;

    private float ringZDelta;

    private Sequence idleSeq;
    private Coroutine blinkCo;


    private void Awake()
    {
        if (body == null) body = transform;

        bodyBaseLocalPos = body.localPosition;
        bodyBaseLocalZ = body.localEulerAngles.z;

        if (backRings != null) backRingBaseLocalZ = backRings.localEulerAngles.z;
        if (frontRings != null) frontRingBaseLocalZ = frontRings.localEulerAngles.z;
    }

    private void OnEnable()
    {
        StartIdle();
        StartBlink();
    }

    private void OnDisable()
    {
        StopBlink();
        StopIdle(resetPose: false);
    }

    private void OnDestroy()
    {
        StopBlink();
        StopIdle(resetPose: false);
    }

    public void Bind(Character character)
    {
        owner = character;
    }


    public void StartIdle()
    {
        StopIdle(resetPose: true);

        float phase = Random.Range(0f, 1f);

        float ampMul = Random.Range(0.9f, 1.12f);
        float durMul = Random.Range(0.92f, 1.08f);

        float hAmp = hoverAmplitude * ampMul;
        float hDur = hoverDuration * durMul;

        float rotDeg = bodyRotateDeg * Random.Range(0.85f, 1.15f);
        float rotDur = bodyRotateDuration * Random.Range(0.9f, 1.1f);

        float ringDeg = ringWobbleDeg * Random.Range(0.85f, 1.15f);
        float ringDur = ringWobbleDuration * Random.Range(0.9f, 1.1f);

        float t0 = phase * Mathf.PI * 2f;
        Vector3 basePos = bodyBaseLocalPos;

        // 초기 포즈
        body.localPosition = basePos + Vector3.up * (Mathf.Sin(t0) * hAmp);
        body.localRotation = Quaternion.Euler(0f, 0f, bodyBaseLocalZ + Mathf.Sin(t0) * rotDeg);

        ringZDelta = Mathf.Sin(t0) * ringDeg;
        ApplyRingPose(ringZDelta);

        idleSeq = DOTween.Sequence()
            .SetTarget(this)
            .SetUpdate(useUnscaledTime);

        // Body hover (sin 기반: 튐 없음)
        float time = t0;
        Tween hoverTween = DOTween.To(() => time, v =>
        {
            time = v;
            var p = basePos;
            p.y += Mathf.Sin(time) * hAmp;
            body.localPosition = p;
        }, t0 + Mathf.PI * 2f, hDur)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);

        // Body rotate
        Tween bodyRotTween = body.DOLocalRotate(new Vector3(0f, 0f, bodyBaseLocalZ + rotDeg), rotDur * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Rings wobble (back/front 동일 delta)
        Tween ringTween = DOTween.To(() => ringZDelta, v =>
        {
            ringZDelta = v;
            ApplyRingPose(ringZDelta);
        }, ringDeg, ringDur * 0.5f)
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);

        idleSeq.Join(hoverTween);
        idleSeq.Join(bodyRotTween);
        idleSeq.Join(ringTween);

        idleSeq.Play();
    }



    public void StopIdle(bool resetPose)
    {
        if (idleSeq != null && idleSeq.IsActive())
        {
            idleSeq.Kill();
            idleSeq = null;
        }

        // StartIdle에서 SetTarget(this) 걸었으니 안전하게 this 타겟만 정리
        DOTween.Kill(this);

        if (!resetPose) return;

        if (body != null)
        {
            body.localPosition = bodyBaseLocalPos;
            body.localRotation = Quaternion.Euler(0f, 0f, bodyBaseLocalZ);
        }

        if (backRings != null)
            backRings.localRotation = Quaternion.Euler(0f, 0f, backRingBaseLocalZ);

        if (frontRings != null)
            frontRings.localRotation = Quaternion.Euler(0f, 0f, frontRingBaseLocalZ);
    }


    private void ApplyRingPose(float zDelta)
    {
        if (backRings != null)
            backRings.localRotation = Quaternion.Euler(0f, 0f, backRingBaseLocalZ + zDelta);

        if (frontRings != null)
            frontRings.localRotation = Quaternion.Euler(0f, 0f, frontRingBaseLocalZ + zDelta);
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

        // 비활성화 될 때 눈이 감긴 상태로 멈추는 거 싫으면 기본으로 복구
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
}
