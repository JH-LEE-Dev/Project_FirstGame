using DG.Tweening;
using UnityEngine;

public class SocketVisual : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform visual;

    [Header("Layout Follow (Root moves)")]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;

    [Header("Hover (Visual only)")]
    [SerializeField] private float hoverAmplitude = 0.004f;
    [SerializeField] private float hoverDuration = 2.2f;

    [Header("Wobble Rotate (Visual only)")]
    [SerializeField] private float wobbleDeg = 3f;      
    [SerializeField] private float wobbleDuration = 2.4f;


    // --- internal ---
    private Tween layoutTween;
    private Tween hoverTween;
    private Tween rotateTween;

    private Vector3 targetRootLocalPos;

    private float baseVisualLocalY;   // visual의 기본 Y(대개 0)
    private float baseVisualZ;        // 프리팹에서 준 기본 20도 유지

    private float hoverPhase0;         
    private float wobblePhase0;    

    private void Awake()
    {
        if (visual == null)
        {
            // 안전망: 자식 0번을 visual로 간주 (가능하면 인스펙터로 꽂아줘)
            if (transform.childCount > 0) visual = transform.GetChild(0);
        }

        targetRootLocalPos = transform.localPosition;

        if (visual != null)
        {
            baseVisualLocalY = visual.localPosition.y;
            baseVisualZ = visual.localEulerAngles.z; // 자식 SlotVisual이 가진 기본 20도
        }

        // 시작점만 다르게 (박자 동일)
        hoverPhase0 = Random.Range(0f, Mathf.PI * 2f);
        wobblePhase0 = Random.Range(0f, Mathf.PI * 2f);
    }

    private void OnEnable()
    {
        StartHover();
        StartWobbleRotate();
    }

    private void OnDisable()
    {
        KillAllTweens();
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

    private void KillAllTweens()
    {
        layoutTween?.Kill();
        hoverTween?.Kill();
        rotateTween?.Kill();

        layoutTween = null;
        hoverTween = null;
        rotateTween = null;
    }
}
