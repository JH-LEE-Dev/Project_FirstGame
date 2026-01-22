using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BarMotion : MonoBehaviour
{
    [Header("Bar Settings")]
    [SerializeField] private bool activeGhost = false;
    [SerializeField] private bool activeShield = false;
    [SerializeField] private bool activeShaking = false;
    [SerializeField] private bool activeJelly = false;

    private bool activateVisual => activeShaking || activeJelly;

    [Header("Main Settings")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private float mainDelay = 0f;
    [SerializeField] private float mainDuration = 0.5f;
    [SerializeField] private Ease mainEase = Ease.Linear;
    [ShowIf("activateVisual"), SerializeField] private RectTransform visualRect;

    [Header("Ghost Settings")]
    [ShowIf("activeGhost"), SerializeField] private Slider ghostSlider;
    [ShowIf("activeGhost"), SerializeField] private float ghostDelay = 0.5f;
    [ShowIf("activeGhost"), SerializeField] private float ghostDuration = 0.5f;
    [ShowIf("activeGhost"), SerializeField] private Ease ghostEase = Ease.Linear;

    [Header("Shield Settings")]
    [ShowIf("activeShield"), SerializeField] private Slider shieldSlider;
    [ShowIf("activeShield"), SerializeField] private float shieldDelay = 0.5f;
    [ShowIf("activeShield"), SerializeField] private float shieldDuration = 0.5f;
    [ShowIf("activeShield"), SerializeField] private Ease shieldEase = Ease.Linear;

    [Header("Shake Settings")]
    [ShowIf("activeShaking"), SerializeField] private float shakeDuration = 0.5f;
    [ShowIf("activeShaking"), SerializeField] private float shakePower = 100f;
    [ShowIf("activeShaking"), SerializeField] private Ease shakeEase = Ease.Linear;

    [Header("Jelly Settings")]
    [ShowIf("activeJelly"), SerializeField] private float jellyDuration = 0.5f;
    [ShowIf("activeJelly"), SerializeField] private float jellyPower = 100f;
    [ShowIf("activeJelly"), SerializeField] private Ease jellyEase = Ease.Linear;

    private Vector2 originAnchoredPos = Vector2.zero;
    private Vector3 originlocalScale = Vector3.zero;

    private Sequence mainSeq = null;
    private Sequence ghostSeq = null;
    private Sequence shieldSeq = null;

    private RectTransform mainRect;

    private int maxValue = 0;

    public void Init(float _progressValue, int _maxValue = 0)
    {
        if (mainSlider)
            mainSlider.value = _progressValue;

        if (ghostSlider)
            ghostSlider.value = _progressValue;

        if (shieldSlider)
            shieldSlider.value = 0f;

        maxValue = _maxValue;
    }

    public void OnHit(float _progressValue)
    {
        if (activeGhost)
            OnHitGhostSlider(_progressValue);
        else 
            OnNotGhostSlider(_progressValue);
    }

    public void OnFill(float _progressValue)
    {
        if (activeGhost)
            OnFillGhostSlider(_progressValue);
        else
            OnNotGhostSlider(_progressValue);
    }

    public void OnShieldHit(float _progressValue, Action _callback = null)
    {
        if (!activeShield)
            return;

        CalcShield(_progressValue, _callback);
    }

    public void CalcShield(float _progressValue, Action callback = null)
    {
        if (null == shieldSlider)
            return;

        CancelPrevMotion(shieldSeq);

        shieldSeq = DOTween.Sequence();

        shieldSeq.AppendInterval(shieldDelay);
        shieldSeq.Append(shieldSlider.DOValue(_progressValue, shieldDuration)
            .SetEase(shieldEase)
            .SetUpdate(false));

        shieldSeq.OnStart(() =>
        {
            callback?.Invoke();
        });

        shieldSeq.OnComplete(() =>
        {
            shieldSlider.value = _progressValue;
        });
    }

    public void DirectShieldSet(float _progress) => shieldSlider.value = _progress;

    public void CalcMain(float _progressValue, Action callback = null)
    {
        if (null == mainSlider)
            return;

        CancelPrevMotion(mainSeq);

        mainSeq = DOTween.Sequence();

        mainSeq.AppendInterval(shieldDelay);
        mainSeq.Append(mainSlider.DOValue(_progressValue, shieldDuration)
            .SetEase(shieldEase)
            .SetUpdate(false));

        mainSeq.OnComplete(() =>
        {
            mainSlider.value = _progressValue;
            callback?.Invoke();
        });
    }

    public Vector2 GetAnchoredPos() => mainRect.anchoredPosition;

    private void Awake()
    {
        if(null != visualRect)
        {
            originAnchoredPos = visualRect.anchoredPosition;
            originlocalScale = visualRect.localScale;
        }

        mainRect = GetComponent<RectTransform>(); 
    }

    private void OnNotGhostSlider(float _progressValue)
    {
        if (null == mainSlider)
            return;

        CancelPrevMotion(mainSeq);

        mainSeq = DOTween.Sequence();

        mainSeq.AppendInterval(mainDelay);

        mainSeq.Append(mainSlider.DOValue(_progressValue, mainDuration)
            .SetEase(mainEase)
            .SetUpdate(false));

        ShakeBar();
        JellyBar();
    }

    private void OnHitGhostSlider(float _progressValue)
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
        JellyBar();
    }

    private void OnFillGhostSlider(float _progressValue)
    {
        if (null == mainSlider || null == ghostSlider)
            return;

        CancelPrevMotion(mainSeq);
        mainSeq = DOTween.Sequence();
        mainSeq.AppendInterval(mainDelay);
        mainSeq.Append(mainSlider.DOValue(_progressValue, mainDuration)
            .SetEase(mainEase)
            .SetUpdate(false));

        CancelPrevMotion(ghostSeq);
        ghostSeq = DOTween.Sequence();
        ghostSeq.AppendInterval(ghostDelay);
        ghostSeq.Append(ghostSlider.DOValue(_progressValue, ghostDuration)
            .SetEase(ghostEase)
            .SetUpdate(false));

        ShakeBar();
        JellyBar();
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

    private void JellyBar()
    {
        if (false == activeJelly || null == visualRect)
            return;

        visualRect.DOKill();

        visualRect.DOShakeScale(jellyDuration, jellyPower)
            .SetEase(jellyEase)
            .SetUpdate(false)
            .OnComplete(() =>
            {
                visualRect.localScale = originlocalScale;
            });
    }

    private void CancelPrevMotion(Sequence target)
    {
        if (null != target && target.IsActive())
            target.Kill();
    }

    [Button]
    private void PlaHitTest()
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

    [ShowIf("activeJelly"), Button]
    private void PlayJellyMotionTest()
    {
        JellyBar();
    }
}
