using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class CardMotion : MonoBehaviour
{
    private CardInstance owner;
    private RectTransform rt;

    [Header("MainMoving")]
    [SerializeField] private float followFreq = 18f;
    [SerializeField, Range(0f, 1.2f)] private float followDamp = 0.85f;
    [SerializeField] private float rotateLerp = 18f;
    [SerializeField] private float snapDist = 0.05f;

    private Vector2 targetPos;
    private float targetAngleZ;
    private Vector2 velocity;

    [Header("Hover Scale")]
    [SerializeField] private float hoverScale = 1.3f;
    [SerializeField] private float hoverDuration = 0.15f;
    private Tween hoverTween;
    private Vector3 originScale;

    [Header("Preview")]
    [SerializeField] private float previewScale = 2f;
    [SerializeField] private float previewMoveDuration = 0.3f;
    [SerializeField] private float previewScaleDuration = 0.3f;
    [SerializeField] private float previewEndScaleDur = 0.6f;
    private Tween previewMoveTween;
    private Tween previewScaleTween;
    private Tween previewRotateTween;
    private Tween previewEndScaleTween;

    [Header("Reject Shake")]
    [SerializeField] private float rejectTotal = 0.2f;  
    [SerializeField] private float rejectScale = 0.95f;   
    [SerializeField] private float rejectAngle = 5f;    
    private Sequence rejectSeq;

    [Header("Grave Motion")]
    [SerializeField] private float graveDuration = 0.4f;
    [SerializeField] private float graveTiltZ = 80f;     // 왼쪽으로 기울기(도)
    [SerializeField] private float graveScale = 0.3f;    // 줄어드는 비율
    private Tween flyTween;
    private Tween flyRotateTween;
    private Tween flyScaleTween; 


    public void AllKillTweens()
    {
        hoverTween.Kill();

        previewMoveTween.Kill();
        previewScaleTween.Kill();
        previewRotateTween.Kill();
        previewEndScaleTween.Kill();

        rejectSeq.Kill();

        flyTween.Kill();
        flyRotateTween.Kill();
        flyScaleTween.Kill();

        transform.localScale = originScale;
    }


    public void Bind(CardInstance card)
    {
        owner = card;
        rt = GetComponent<RectTransform>();
        originScale = transform.localScale;
        targetPos = rt.anchoredPosition;
    }


    private void Update()
    {
        Tick(Time.unscaledDeltaTime);
    }

    public void SetTarget(Vector2 pos, float angleZ)
    {
        targetPos = pos;
        targetAngleZ = angleZ;
    }



    public void Tick(float dt)
    {
        if (owner.cardInstanceType != CardInstanceType.Hand) return;
        if (owner.cardState != CardState.InHand) return;

        Vector2 pos = rt.anchoredPosition;

        float k = followFreq * followFreq;
        float c = 2f * followDamp * followFreq;

        Vector2 accel = k * (targetPos - pos) - c * velocity;
        velocity += accel * dt;
        pos += velocity * dt;

        if ((pos - targetPos).sqrMagnitude < snapDist * snapDist)
        {
            pos = targetPos;
            velocity = Vector2.zero;
        }

        rt.anchoredPosition = pos;

        float currentZ = rt.localEulerAngles.z;
        float z = Mathf.LerpAngle(currentZ, targetAngleZ, 1f - Mathf.Exp(-rotateLerp * dt));
        rt.localRotation = Quaternion.Euler(0, 0, z);
    }


    // 못 쓸때.
    public void PlayReject()
    {
        rejectSeq?.Kill();

        Vector3 baseScale = transform.localScale;
        float baseZ = transform.localEulerAngles.z;

        float t1 = rejectTotal * 0.45f;
        float t2 = rejectTotal * 0.55f;

        float a1 = rejectAngle;
        float a2 = -rejectAngle * 0.60f;
        float a3 = rejectAngle * 0.25f;

        rejectSeq = DOTween.Sequence()
            .SetUpdate(true) 
                             
            .Join(transform.DOScale(baseScale * rejectScale, t1).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(baseScale, t2).SetEase(Ease.OutQuad))

            .Insert(0f, transform.DOLocalRotate(new Vector3(0, 0, baseZ + a1), rejectTotal * 0.25f).SetEase(Ease.OutQuad))
            .Insert(rejectTotal * 0.25f, transform.DOLocalRotate(new Vector3(0, 0, baseZ + a2), rejectTotal * 0.25f).SetEase(Ease.InOutQuad))
            .Insert(rejectTotal * 0.50f, transform.DOLocalRotate(new Vector3(0, 0, baseZ + a3), rejectTotal * 0.20f).SetEase(Ease.InOutQuad))
            .Insert(rejectTotal * 0.70f, transform.DOLocalRotate(new Vector3(0, 0, baseZ), rejectTotal * 0.30f).SetEase(Ease.OutQuad));
    }


    // Hover
    public void HoverOn()
    {
        if (owner.cardState != CardState.InHand) return;

        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale * hoverScale, hoverDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void HoverOff()
    {
        if (owner.cardState != CardState.InHand) return;

        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale, hoverDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void KillHoverOnly()
    {
        hoverTween?.Kill();
        hoverTween = null;
    }


    // Preview
    public void StartPreview(Vector2 centerPos)
    {
        KillHoverOnly();

        previewMoveTween?.Kill();
        previewScaleTween?.Kill();
        previewRotateTween?.Kill();
        previewEndScaleTween?.Kill();

        previewMoveTween = rt.DOAnchorPos(centerPos, previewMoveDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        previewScaleTween = transform.DOScale(originScale * previewScale, previewScaleDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        // 센터는 정면
        targetAngleZ = 0f;
        previewRotateTween = rt.DOLocalRotate(Vector3.zero, previewMoveDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    public void EndPreview()
    {
        previewMoveTween?.Kill();
        previewScaleTween?.Kill();
        previewRotateTween?.Kill();

        // hoverTween도 멈춰두는 게 안전
        KillHoverOnly();

        velocity = Vector2.zero;

        previewEndScaleTween?.Kill();
        previewEndScaleTween = transform.DOScale(originScale, Mathf.Max(0.01f, previewEndScaleDur))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }


    // 
    public void FlyToGrave(Vector3 graveAnchoredPos, System.Action onComplete = null)
    {
        KillHoverOnly();
        previewMoveTween?.Kill();
        previewScaleTween?.Kill();
        previewRotateTween?.Kill();
        previewEndScaleTween?.Kill();
        rejectSeq?.Kill();

        flyTween?.Kill();
        flyRotateTween?.Kill();
        flyScaleTween?.Kill();

        velocity = Vector2.zero;

        flyTween = rt.DOAnchorPos((Vector2)graveAnchoredPos, graveDuration)
            .SetEase(Ease.InSine)
            .SetUpdate(true);

        flyRotateTween = rt.DOLocalRotate(new Vector3(0f, 0f, graveTiltZ), graveDuration)
            .SetEase(Ease.InSine)
            .SetUpdate(true);

        flyScaleTween = transform.DOScale(originScale * graveScale, graveDuration)
            .SetEase(Ease.InSine)
            .SetUpdate(true);

        flyTween.OnComplete(() => onComplete?.Invoke());
    }
}