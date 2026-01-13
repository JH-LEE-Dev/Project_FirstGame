using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BarMotion : MonoBehaviour
{
    [Header("Bar Settings")]
    [SerializeField] private bool activeGhost = false;
    [SerializeField] private bool activeShaking = false;
    [SerializeField] private bool activeParticle = false;

    [Header("Main Settings")]
    [HideIf("activeGhost"),SerializeField] private Slider mainSlider;
    [HideIf("activeGhost"), SerializeField] private float mainDuration = 0.5f;
    [HideIf("activeGhost"), SerializeField] private Ease mainEase = Ease.Linear;

    [Header("Ghost Settings")]
    [ShowIf("activeGhost"), SerializeField] private Slider ghostSlider;
    [ShowIf("activeGhost"), SerializeField] private float ghostDelay = 0.5f;
    [ShowIf("activeGhost"), SerializeField] private float ghostDuration = 0.5f;
    [ShowIf("activeGhost"), SerializeField] private Ease ghostEase = Ease.Linear;

    [Header("Shake Settings")]
    [ShowIf("activeShaking"), SerializeField] private RectTransform visualRect;
    [ShowIf("activeShaking"), SerializeField] private float shakeDuration = 0.5f;
    [ShowIf("activeShaking"), SerializeField] private float shakePower = 100f;
    [ShowIf("activeShaking"), SerializeField] private Ease shakeEase = Ease.Linear;
    private Vector2 originAnchoredPos = Vector2.zero;

    [Header("Particle Settings")]
    [ShowIf("activeParticle"), SerializeField] private ParticleSystem particle;
    [ShowIf("activeParticle"), SerializeField] private int particleCnt = 10;
    [ShowIf("activeParticle"), SerializeField, Range(1f, 10f)] private float particlePower = 1f;

    private Sequence mainSeq = null;
    private Sequence ghostSeq = null;

    private void Awake()
    {
        Setup_Particle();

        if(null != visualRect)
            originAnchoredPos = visualRect.anchoredPosition;
    }

    public void OnHit(float _progressValue)
    {
        if (activeGhost)
            OnHitGhost(_progressValue);
        else 
            OnHitNotGhost(_progressValue);
    }

    private void OnHitNotGhost(float _progressValue)
    {
        if (null == mainSlider)
            return;

        CancelPrevMotion(mainSeq);

        mainSeq = DOTween.Sequence();

        mainSeq.Append(mainSlider.DOValue(_progressValue, mainDuration)
            .SetEase(mainEase)
            .SetUpdate(false));

        ShakeBar();
    }

    private void OnHitGhost(float _progressValue)
    {
        if (null == mainSlider || null == ghostSlider)
            return;

        CancelPrevMotion(mainSeq);
        CancelPrevMotion(ghostSeq);

        mainSlider.value = _progressValue;

        ghostSeq = DOTween.Sequence();

        ghostSeq.AppendInterval(ghostDelay);
        ghostSeq.Append(ghostSlider.DOValue(_progressValue, ghostDuration)
            .SetEase(ghostEase)
            .SetUpdate(false));

        ShakeBar();
    }

    private void ShakeBar()
    {
        if (!activeShaking || null == visualRect)
            return;

        visualRect.anchoredPosition = originAnchoredPos;

        visualRect.DOKill();

        visualRect.DOShakeAnchorPos(shakeDuration, shakePower)
            .SetEase(shakeEase)
            .SetUpdate(false)
            .OnComplete(() =>
            {
                visualRect.anchoredPosition = originAnchoredPos;
            });
    }

    private void Setup_Particle()
    {
        if (!activeParticle || null == particle)
            return;


    }

    private void CancelPrevMotion(Sequence target)
    {
        if (null != target && target.IsActive())
            target.Kill();
    }

    [Button]
    private void PlayMotionTest()
    {
        OnHit(0.5f);
    }

    [Button]
    private void ResetSliderData()
    {
        if(null != mainSlider)
        {
            mainSlider.value = 1f;
            CancelPrevMotion(mainSeq);
        }

        if(null != ghostSlider)
        {
            ghostSlider.value = 1f;
            CancelPrevMotion(ghostSeq);
        }
    }
}
