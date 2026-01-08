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
    [SerializeField] private float previewScale = 3f;
    [SerializeField] private float previewMoveDuration = 0.3f;
    [SerializeField] private float previewScaleDuration = 0.3f;
    [SerializeField] private float previewEndScaleDur = 0.5f;

    private Tween previewMoveTween;
    private Tween previewScaleTween;
    private Tween previewRotateTween;
    private Tween previewEndScaleTween;



    // 이거 키면 패쪽으로 안빨려감.
    public bool IgnoreHandLayout = true;


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



    public void EnterHand()
    {
        IgnoreHandLayout = false;
        velocity = Vector2.zero;
    }



    public void ExitHand()
    {
        IgnoreHandLayout = false;
        velocity = Vector2.zero;
        KillHoverOnly();
        transform.localScale = originScale;
    }



    public void SetTarget(Vector2 pos, float angleZ)
    {
        targetPos = pos;
        targetAngleZ = angleZ;
    }



    public void Tick(float dt)
    {
        if (IgnoreHandLayout) return;

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




    // Hover
    public void HoverOn()
    {
        if (IgnoreHandLayout) return;

        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale * hoverScale, hoverDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void HoverOff()
    {
        if (IgnoreHandLayout) return;

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
        IgnoreHandLayout = true;

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
        IgnoreHandLayout = false;

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
}