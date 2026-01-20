using DG.Tweening;
using UnityEngine;
using TMPro;


public class SocketVisual : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform visual;
    [SerializeField] private CountUI countUI;

    [Header("Layout Follow (Root moves)")]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;
    private Tween layoutTween;
    private Vector3 targetRootLocalPos;

    [Header("Hover (Visual only)")]
    [SerializeField] private float hoverAmplitude = 0.028f;
    [SerializeField] private float hoverDuration = 2.2f;
    private Tween hoverTween;
    private float baseVisualLocalY;
    private float baseVisualZ;     
    private float hoverPhase0;


    [Header("Wobble Rotate (Visual only)")]
    [SerializeField] private float wobbleDeg = 3f;      
    [SerializeField] private float wobbleDuration = 2.4f;
    private Tween rotateTween;
    private float wobblePhase0;


    [Header("Equip Impact")]
    [SerializeField] private float impactTotal = 0.4f;

    [SerializeField] private float impactScaleUp = 1.3f; 
    [SerializeField] private float scaleUpTime = 0.02f;  

    [SerializeField] private float hitPressY = 0.04f;    
    [SerializeField] private float hitPressTime = 0.05f; 

    [SerializeField] private float shakeAmp = 0.18f;     
    [SerializeField] private float shakeFreq = 15f;      
    [SerializeField] private float shakeRotDeg = 5f;     

    private Sequence impactSeq;

    [Header("Unequip (0.4s)")]
    [SerializeField] private float unequipTotal = 0.4f;
    [SerializeField] private float unequipDropY = 0.12f;     // 아래로 내려가는 거리
    [SerializeField] private float unequipDownTime = 0.08f;  // 빠르게 내려감
    [SerializeField] private Ease unequipDownEase = Ease.OutQuad;
    [SerializeField] private Ease unequipReturnEase = Ease.OutCubic;

    private Tween unequipTween;


    private Vector3 baseVisualLocalPos;
    private Vector3 baseVisualLocalScale;

    private void Awake()
    {
        targetRootLocalPos = transform.localPosition;

        if (visual != null)
        {
            baseVisualLocalPos = visual.localPosition;
            baseVisualLocalScale = visual.localScale;
            baseVisualZ = visual.localEulerAngles.z; // 프리팹 기본 각도(예: 20도)
            baseVisualLocalY = baseVisualLocalPos.y; // 기존 Hover가 쓰던 값 유지
        }

        // 시작점만 다르게 (박자 동일)
        hoverPhase0 = Random.Range(0f, Mathf.PI * 2f);
        wobblePhase0 = Random.Range(0f, Mathf.PI * 2f);


        CountTypeSetting(CountUIType.HideWhenZero, 0);
    }

    private void OnEnable()
    {
        StartIdle();
    }

    private void OnDisable()
    {
        KillIdleTween();
    }



    public void SetTargetLocalPosition(Vector3 rootLocalPos, bool snap = false)
    {
        targetRootLocalPos = rootLocalPos;

        layoutTween?.Kill();

        if (snap)
        {
            transform.localPosition = targetRootLocalPos;
            return;
        }

        layoutTween = transform
            .DOLocalMove(targetRootLocalPos, moveDuration)
            .SetEase(moveEase)
            .SetTarget(this);
    }

    private void StartIdle()
    {
        StartHover();
        StartWobbleRotate();
    }

    private void StartHover()
    {
        hoverTween?.Kill();
        if (visual == null) return;

        float phase = hoverPhase0;

        hoverTween = DOTween.To(() => phase, v =>
        {
            phase = v;

            var p = visual.localPosition;
            p.y = baseVisualLocalY + Mathf.Sin(phase) * hoverAmplitude;
            visual.localPosition = p;

        }, hoverPhase0 + Mathf.PI * 2f, hoverDuration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart)
        .SetTarget(this);
    }

    private void StartWobbleRotate()
    {
        rotateTween?.Kill();
        if (visual == null) return;

        float phase = wobblePhase0;

        rotateTween = DOTween.To(() => phase, v =>
        {
            phase = v;

            float z = baseVisualZ + Mathf.Sin(phase) * wobbleDeg; // 기본 20도 + ±3도
            visual.localRotation = Quaternion.Euler(0f, 0f, z);

        }, wobblePhase0 + Mathf.PI * 2f, wobbleDuration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart)
        .SetTarget(this);
    }

    private void KillIdleTween()
    {
        layoutTween?.Kill();
        hoverTween?.Kill();
        rotateTween?.Kill();

        impactSeq?.Kill();
        unequipTween?.Kill();

        layoutTween = null;
        hoverTween = null;
        rotateTween = null;
        impactSeq = null;
        unequipTween = null;
    }


    public void PlayImpactSlam()
    {
        if (visual == null) return;

        impactSeq?.Kill();

        hoverTween?.Kill();
        rotateTween?.Kill();
        hoverTween = null;
        rotateTween = null;

        visual.localPosition = baseVisualLocalPos;
        visual.localScale = baseVisualLocalScale;
        visual.localRotation = Quaternion.Euler(0f, 0f, baseVisualZ);

        float tUp = Mathf.Min(scaleUpTime, impactTotal);
        float tDown = Mathf.Max(0f, impactTotal - tUp);

        float pressT = Mathf.Min(hitPressTime, impactTotal * 0.25f);

        float shakeT = impactTotal;

        float elapsed = 0f;
        float seedX = Random.Range(0f, 1000f);
        float seedY = Random.Range(0f, 1000f);
        float seedR = Random.Range(0f, 1000f);

        impactSeq = DOTween.Sequence().SetTarget(this);
        impactSeq.Append(
            visual.DOLocalMoveY(baseVisualLocalPos.y - hitPressY, pressT)
                  .SetEase(Ease.OutQuad)
        );

        impactSeq.Join(
            visual.DOScale(baseVisualLocalScale * impactScaleUp, tUp)
                  .SetEase(Ease.OutBack)
        );

        if (tDown > 0f)
        {
            impactSeq.Append(
                visual.DOScale(baseVisualLocalScale, tDown)
                      .SetEase(Ease.OutCubic)
            );
        }

        impactSeq.Join(
            DOTween.To(() => elapsed, v =>
            {
                elapsed = v;

                float nT = elapsed * shakeFreq;

                float damp01 = 1f - Mathf.Clamp01(elapsed / shakeT);
                float amp = shakeAmp * damp01 * damp01;

                float nx = (Mathf.PerlinNoise(seedX, nT) * 2f - 1f);
                float ny = (Mathf.PerlinNoise(seedY, nT + 31.7f) * 2f - 1f);

                Vector3 p = baseVisualLocalPos;
                p.x += nx * amp;
                p.y += ny * amp;
                visual.localPosition = p;

                if (shakeRotDeg > 0f)
                {
                    float nr = (Mathf.PerlinNoise(seedR, nT + 79.3f) * 2f - 1f);
                    float z = baseVisualZ + nr * shakeRotDeg * damp01;
                    visual.localRotation = Quaternion.Euler(0f, 0f, z);
                }

            }, shakeT, shakeT)
            .SetEase(Ease.Linear)
        );

        impactSeq.AppendCallback(() =>
        {
            visual.localPosition = baseVisualLocalPos;
            visual.localScale = baseVisualLocalScale;
            visual.localRotation = Quaternion.Euler(0f, 0f, baseVisualZ);

            float curY = visual.localPosition.y;
            float sinH = (hoverAmplitude <= 0f) ? 0f : (curY - baseVisualLocalY) / hoverAmplitude;
            float h = AsinClamped(sinH);
            hoverPhase0 = WrapRad(h);

            float curZ = visual.localEulerAngles.z;
            float dz = Mathf.DeltaAngle(baseVisualZ, curZ);
            float sinR = (wobbleDeg <= 0f) ? 0f : dz / wobbleDeg;
            wobblePhase0 = WrapRad(AsinClamped(sinR));

            StartIdle();
        });
    }

    private float WrapRad(float x)
    {
        float twoPi = Mathf.PI * 2f;
        x %= twoPi;
        if (x < 0f) x += twoPi;
        return x;
    }

    private float AsinClamped(float x)
    {
        return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
    }


    public void PlayUnequip()
    {
        if (visual == null) return;

        impactSeq?.Kill();
        unequipTween?.Kill();

        hoverTween?.Kill();
        rotateTween?.Kill();
        hoverTween = null;
        rotateTween = null;

        float baseY = baseVisualLocalPos.y;

        float tDown = Mathf.Min(unequipDownTime, unequipTotal);
        float tUp = Mathf.Max(0f, unequipTotal - tDown);

        float targetDownY = baseY - unequipDropY;

        Sequence seq = DOTween.Sequence().SetTarget(this);

        seq.AppendCallback(() =>
        {
            var p = visual.localPosition;
            p.y = baseY;
            visual.localPosition = p;

        });

        seq.Append(visual.DOLocalMoveY(targetDownY, tDown).SetEase(unequipDownEase));

        if (tUp > 0f)
            seq.Append(visual.DOLocalMoveY(baseY, tUp).SetEase(unequipReturnEase));
        else
            seq.AppendCallback(() =>
            {
                var p = visual.localPosition;
                p.y = baseY;
                visual.localPosition = p;
            });

        seq.AppendCallback(() =>
        {
            visual.localPosition = new Vector3(visual.localPosition.x, baseY, visual.localPosition.z);
            hoverPhase0 = 0f;
            wobblePhase0 = 0f;

            StartIdle();
        });

        unequipTween = seq;
    }

    public Transform GetSocketVisualTransform()
    {
        return visual;
    }




    // For Count
    public void CountTypeSetting(CountUIType _type, int _count = -1)
    {
        countUI?.TypeSetting(_type, _count);
    }

    public void SetOverlapCount(int count)
    {
        countUI?.SetCount(count);
    }

    public int GetOverlapCount()
    {
        return countUI.GetCount();
    }
}
