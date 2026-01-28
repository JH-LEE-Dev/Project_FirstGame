using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class CardMotion : MonoBehaviour
{
    private MainCardInstance owner;
    private RectTransform rt;

    [Header("MainMoving")]
    [SerializeField] private float followFreq = 22f;
    [SerializeField, Range(0f, 1.2f)] private float followDamp = 0.66f;
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
    private float previewScale = 2f;
    private float previewMoveDuration = 0.3f;
    private float previewScaleDuration = 0.3f;
    private float previewEndScaleDur = 0.3f;
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
    private float graveDuration = 0.2f;
    private float graveTiltZ = 80f;     // 왼쪽으로 기울기(도)
    private float graveScale = 0.3f;    // 줄어드는 비율
    private Tween flyTween;
    private Tween flyRotateTween;
    private Tween flyScaleTween;


    [Header("Bullet Socket Motion")]
    const float insertDur = 0.2f;
    const float insertPreviewDur = 0.15f;
    Ease insertEasePos = Ease.InSine;  
    Ease insertEaseRot = Ease.InSine; 
    Ease insertEaseScale = Ease.InSine;
    private Tween BulletSocketTween;
    private Tween BulletSocketRotateTween;
    private Tween BulletSocketScaleTween;


    [Header("SelectMode Motion")]
    const float SelectDur = 0.4f;
    Ease SelectEaseScale = Ease.OutCubic;
    private Tween SelectTween;
    private Vector3 SelectScale;

    [Header("Using Motion")]
    private Tween consumeScaleTween;
    private bool extinctionActive;
    private float extinctionTime;
    private float extinctionDur;
    private float extinctionPhase;
    private float extinctionBaseZ;
    private float extinctionAmplitudeStart = 1f;
    private float extinctionIntensityMul = 1f; 
    private float extinctionAngle = 3.5f;       
    private float extinctionFreqStart = 5f;    
    private float extinctionFreqEnd = 15f;     
    private Tween extinctionScaleTween;


    public int socketIndex { get; private set; }

    public void AllKillTweens(bool bRestoreScale = true)
    {
        hoverTween?.Kill();

        previewMoveTween?.Kill();
        previewScaleTween?.Kill();
        previewRotateTween?.Kill();
        previewEndScaleTween?.Kill();

        rejectSeq?.Kill();

        flyTween?.Kill();
        flyRotateTween?.Kill();
        flyScaleTween?.Kill();

        BulletSocketTween?.Kill();
        BulletSocketRotateTween?.Kill();
        BulletSocketScaleTween?.Kill();

        SelectTween?.Kill();

        consumeScaleTween?.Kill();
        extinctionScaleTween?.Kill();
        extinctionActive = false;

        if (bRestoreScale) transform.localScale = originScale;
    }


    public void Bind(MainCardInstance card)
    {
        owner = card;
        rt = GetComponent<RectTransform>();
        originScale = transform.localScale;
        SelectScale = originScale * 1.3f;
        targetPos = rt.anchoredPosition;

        socketIndex = -1;
    }


    private void Update()
    {
        ToTargetPos(Time.unscaledDeltaTime);
        TickExtinctionShake(Time.unscaledDeltaTime);
    }

    public void SetSocketIndex(int index)
    {
        socketIndex = index;
    }

    public void SetTarget(Vector2 pos, float angleZ)
    {
        targetPos = pos;
        targetAngleZ = angleZ;
    }


    public void ToTargetPos(float dt)
    {
        if (owner.cardInstanceType != CardInstanceType.Hand) return;
        if ((owner.cardState == CardState.InHand || owner.cardState == CardState.Selecting) == false) return;

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
        // 다른 연출들 정리
        AllKillTweens(false);

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

    public void SelectHoverOn()
    {
        if (owner.cardState != CardState.InHand) return;

        hoverTween?.Kill();
        hoverTween = transform.DOScale(SelectScale * hoverScale, hoverDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void SelectHoverOff()
    {
        if (owner.cardState != CardState.InHand) return;

        hoverTween?.Kill();
        hoverTween = transform.DOScale(SelectScale, hoverDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    // SelectMode
    public void StartSelectMode()
    {
        // 다른 연출들 정리
        AllKillTweens(false);

        SelectTween?.Kill();
        SelectTween = transform.DOScale(SelectScale, SelectDur)
            .SetEase(SelectEaseScale)
            .SetUpdate(true);
    }
    public void EndSelectMode()
    {
        // 다른 연출들 정리
        AllKillTweens(false);

        SelectTween?.Kill();
        SelectTween = transform.DOScale(originScale, SelectDur)
            .SetEase(SelectEaseScale)
            .SetUpdate(true);

    }

    // Preview
    public void StartPreview(Vector2 centerPos, System.Action onArrive = null)
    {
        // 다른 연출들 정리
        AllKillTweens(false);

        previewMoveTween = rt.DOAnchorPos(centerPos, previewMoveDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        previewScaleTween = transform.DOScale(originScale * previewScale, previewScaleDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        targetAngleZ = 0f;

        previewRotateTween = rt.DOLocalRotate(Vector3.zero, previewMoveDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        previewMoveTween.OnComplete(() => onArrive?.Invoke());
    }
    public void EndPreview()
    {
        // 다른 연출들 정리
        AllKillTweens(false);

        velocity = Vector2.zero;

        previewEndScaleTween?.Kill();
        previewEndScaleTween = transform.DOScale(originScale, Mathf.Max(0.01f, previewEndScaleDur))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    // 
    public void FlyToGrave(Vector3 graveAnchoredPos, System.Action onComplete = null)
    {
        // 다른 연출들 정리
        AllKillTweens(false);

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

    public void FlyToBulletSocket(bool bIsHand,Transform socketTransform, System.Action onComplete = null)
    {
        // 다른 연출들 정리
        AllKillTweens(false);

        velocity = Vector2.zero;

        Vector2 targetPos = socketTransform.position;
        Quaternion targetRot = socketTransform.rotation;
        Vector3 targetScale = socketTransform.localScale;

        switch (bIsHand)
        {
            case true:
                {
                    BulletSocketTween = transform.DOMove(targetPos, insertDur)
                        .SetEase(insertEasePos);

                    BulletSocketRotateTween = transform.DORotateQuaternion(targetRot, insertDur)
                        .SetEase(insertEaseRot);

                    BulletSocketScaleTween = transform.DOScale(targetScale, insertDur)
                        .SetEase(insertEaseScale);

                    BulletSocketTween.OnComplete(() => onComplete?.Invoke());
                    break;
                }

            case false:
                {
                    float recoilDur = 0.1f;

                    Vector3 recoilDir = Vector3.right;
                    float recoilDist = 2f;         
                    float recoilAngle = -25f;

                    Vector3 recoilPos = transform.position + recoilDir * recoilDist;

                    Quaternion recoilRot = transform.rotation * Quaternion.Euler(0f, 0f, recoilAngle);

                    Sequence seq = DOTween.Sequence();

                    seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

                    seq.Append(transform.DOMove(recoilPos, recoilDur).SetEase(Ease.OutCubic));
                    seq.Join(transform.DORotateQuaternion(recoilRot, recoilDur).SetEase(Ease.OutCubic));

                    seq.Append(transform.DOMove(targetPos, insertPreviewDur).SetEase(insertEasePos));
                    seq.Join(transform.DORotateQuaternion(targetRot, insertPreviewDur).SetEase(insertEaseRot));
                    seq.Join(transform.DOScale(targetScale, insertPreviewDur).SetEase(insertEaseScale));

                    seq.OnComplete(() => onComplete?.Invoke());
                    break;
                }
        }
    }

    public void FlyToHand()
    {
        AllKillTweens(false);

        owner.Input.SetIgnoreHover(true);

        BulletSocketScaleTween = transform
                .DOScale(originScale, 0.3f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    owner.Input.SetIgnoreHover(false);
                });
    }

    public void PlayConsumeShrink(float duration = 0.6f, float endScaleMul = 0.03f)
    {
        AllKillTweens(false);

        duration = Mathf.Max(0.01f, duration);


        Vector3 baseScale = transform.localScale;
        Vector3 endScale = baseScale * endScaleMul;

        Quaternion baseRot = transform.localRotation;
        Quaternion endRot = baseRot * Quaternion.Euler(0f, 0f, -30f); // 왼쪽 30도


        consumeScaleTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)

                // 스케일
                .Join(
                    transform.DOScale(endScale, duration)
                             .SetEase(Ease.InCubic)
                )

                // 회전
                .Join(
                    transform.DOLocalRotateQuaternion(endRot, duration)
                             .SetEase(Ease.InCubic)
                );
    }

    private void TickExtinctionShake(float dt)
    {
        if (!extinctionActive) return;

        extinctionTime += dt;

        float u = extinctionTime / extinctionDur;
        if (u >= 1f)
        {
            extinctionActive = false;
            return;
        }

        float uu = u * u;

        float freq = Mathf.Lerp(extinctionFreqStart, extinctionFreqEnd, uu) * extinctionIntensityMul;
        float omega = freq * Mathf.PI * 2f;
        extinctionPhase += omega * dt;

        float amp = Mathf.Lerp(extinctionAmplitudeStart, extinctionAngle, uu) * extinctionIntensityMul;

        float z = extinctionBaseZ + Mathf.Sin(extinctionPhase) * amp;
        rt.localRotation = Quaternion.Euler(0f, 0f, z);
    }

    public void PlayExtinctionShake(
    float dur = 0.35f,
    float scaleMul = 0.7f)
    {
        // 기존 트윈/상태 정리 (필요한 것만)
        AllKillTweens(false);
        velocity = Vector2.zero;

        extinctionDur = Mathf.Max(0.01f, dur * 3f);
        extinctionTime = 0f;
        extinctionPhase = 0f;
        extinctionActive = true;

        // 현재 회전 기준으로 흔들기 (프리뷰/손패 어느 상태든 자연스럽게)
        extinctionBaseZ = rt.localEulerAngles.z;

        // 스케일은 dur 동안 줄이기 (콜백/람다 없음)
        Vector3 baseScale = transform.localScale;
        extinctionScaleTween?.Kill();
        extinctionScaleTween = transform.DOScale(baseScale * scaleMul, dur)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }


}