using DG.Tweening;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

using Sequence = DG.Tweening.Sequence;

public class TextMotion : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private float motionDuration = 1f;
    [SerializeField] private Ease motionEase = Ease.Linear;
    [SerializeField] private bool bColorChange = false;
    [SerializeField] private bool bShaking = false;

    [Header("Color Change Settings")]
    [ShowIf("bColorChange"), SerializeField] private Color defaultColor = Color.white;
    [ShowIf("bColorChange"), SerializeField] private Color callColor = Color.softRed;

    [Header("Shaking Settings")]
    [ShowIf("bShaking"), SerializeField] private RectTransform visualRect = null;
    [ShowIf("bShaking"), SerializeField] private float shakeDuration = 1f;
    [ShowIf("bShaking"), SerializeField] private float shakePower = 5f;
    [ShowIf("bShaking"), SerializeField] private Ease shakeEase = Ease.Linear;

    private Sequence colorSeq = null;
    private Sequence shakeSeq = null;

    private Vector2 originalAnchoredPos = Vector2.zero;

    private void Awake()
    {
        if(null != visualRect)
            originalAnchoredPos = visualRect.anchoredPosition;
    }

    public void OnHit(float _prev, float _current, float _motionDuration = -1f)
    {
        if (null == mainText)
            return;

        DOVirtual.Float(_prev, _current, motionDuration, (value) =>
        {
            mainText.text = Mathf.RoundToInt(value).ToString();
        }).SetEase(motionEase).SetUpdate(false);

        OnColorChange();
        OnShake(_motionDuration);
    }
    
    
    public void Init<T>(T _value) where T : struct
    {
        if (mainText == null || (typeof(T) != typeof(int) && typeof(T) != typeof(float)))
            return;

        float convertedValue = Convert.ToSingle(_value);
        Debug.Log(convertedValue);
        mainText.text = Mathf.RoundToInt(convertedValue).ToString();
    }

    private void OnColorChange()
    {
        if (!bColorChange)
            return;

        colorSeq = CancelPrevMotion(colorSeq);

        colorSeq.AppendCallback(() =>
        {
            mainText.color = callColor;
        });

        colorSeq.Append(mainText.DOColor(defaultColor, motionDuration)
            .SetEase(motionEase)
            .SetUpdate(false));
    }

    private void OnShake(float _motionDuration)
    {
        if (!bShaking || null == visualRect)
            return;

        float finalDuration = _motionDuration < 0f ? shakeDuration : _motionDuration;

        shakeSeq = CancelPrevMotion(shakeSeq);

        shakeSeq.AppendCallback(() =>
        {
            visualRect.anchoredPosition = originalAnchoredPos;
        });

        shakeSeq.Append(visualRect.DOShakeAnchorPos(finalDuration, shakePower)
            .SetEase(shakeEase)
            .SetUpdate(false));
    }

    private Sequence CancelPrevMotion(Sequence target)
    {
        if (null == target)
            return null;

        if (target.IsActive())
            target.Kill();

        return DOTween.Sequence();
    }
}
