using DamageNumbersPro;
using DG.Tweening;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

using Sequence = DG.Tweening.Sequence;

public class UIText_HP : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private float motionDuration = 1f;
    [SerializeField] private Ease motionEase = Ease.Linear;
    [SerializeField] private bool colorChange = false;
    [SerializeField] private bool shaking = false;
    [SerializeField] private bool damageNumber = false;
    private UIView_HUD hudSystem = null;

    [Header("Color Change Settings")]
    [ShowIf("colorChange"), SerializeField] private bool overrideColor = false;
    [ShowIf("colorChange"), SerializeField] private Color startColor = Color.softRed;
    [ShowIf("colorChange"), SerializeField] private Color finalColor = Color.white;
    

    [Header("Override FinalColor Settings")]
    [ShowIf("overrideColor"), SerializeField] private Color warningColor = Color.yellow;
    [ShowIf("overrideColor"), SerializeField] private Color dangerColor = Color.orangeRed;

    [Header("Shaking Settings")]
    [ShowIf("shaking"), SerializeField] private RectTransform visualRect = null;
    [ShowIf("shaking"), SerializeField] private float shakeDuration = 1f;
    [ShowIf("shaking"), SerializeField] private float shakePower = 5f;
    [ShowIf("shaking"), SerializeField] private Ease shakeEase = Ease.Linear;

    [Header("Damage Number")]
    [ShowIf("damageNumber"), SerializeField] private float damageWait = 0.5f;
    [ShowIf("damageNumber"), SerializeField] private RectTransform damageTargetPoint;
    private Camera mainCam;

    private Sequence damageSeq = null;
    private Sequence colorSeq = null;
    private Sequence shakeSeq = null;

    private Vector2 originalAnchoredPos = Vector2.zero;

    private void Awake()
    {
        if(null != visualRect)
            originalAnchoredPos = visualRect.anchoredPosition;
    }

    private void Start()
    {
        mainCam = Camera.main;
    }
    
    public void Init<T>(T _value, UIView_HUD _hudSystem) where T : struct
    {
        hudSystem = _hudSystem;

        if (mainText == null || (typeof(T) != typeof(int) && typeof(T) != typeof(float)))
            return;

        float convertedValue = Convert.ToSingle(_value);
        Debug.Log(convertedValue);
        mainText.text = Mathf.RoundToInt(convertedValue).ToString();
    }

    public void OnHit(float _prev, float _current, float _progress, float _damage, 
        float _motionDuration = -1f, GameObject _damagNum = null)
    {
        if (null == mainText)
            return;

        if (!damageNumber)
            OnDefaultHit(_prev, _current, _progress, _motionDuration);
        else
            OnDamageNumberHit(_prev, _current, _progress, _motionDuration, _damagNum);
    }


    private void OnColorChange(float _progress)
    {
        if (!colorChange)
            return;

        Color targetColor = finalColor;

        if (overrideColor)
        {
            if (0.25f >= _progress)
                targetColor = dangerColor;
            else if (0.5f >= _progress)
                targetColor = warningColor;
        }

        colorSeq = CancelPrevMotion(colorSeq);

        colorSeq.AppendCallback(() =>
        {
            mainText.color = startColor;
        });

        colorSeq.Append(mainText.DOColor(targetColor, motionDuration)
            .SetEase(motionEase)
            .SetUpdate(false));
    }

    private void OnShake(float _motionDuration)
    {
        if (!shaking || null == visualRect)
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

    private void OnDefaultHit(float _prev, float _current, float _progress, float _motionDuration)
    {
        DOVirtual.Float(_prev, _current, motionDuration, (value) =>
        {
            mainText.text = Mathf.RoundToInt(value).ToString();
        }).SetEase(motionEase).SetUpdate(false);

        OnColorChange(_progress);
        OnShake(_motionDuration);
    }

    private void OnDamageNumberHit(float _prev, float _current, float _progress, float _motionDuration, GameObject _damagNum)
    {
        _damagNum.transform.position = damageTargetPoint.position;

        // 여기서 넘버링 변경

        damageSeq = CancelPrevMotion(damageSeq);

        damageSeq.AppendInterval(damageWait);
        damageSeq.Append(_damagNum.transform.DOMove(transform.position, motionDuration)
            .SetEase(motionEase));
        damageSeq.Append(_damagNum.transform.DOScale(0f, motionDuration)
            .SetEase(motionEase));

        damageSeq.SetUpdate(false);
        damageSeq.OnComplete(() =>
        {
            hudSystem?.ReturnDamageText(_damagNum);
        });
    }

    private Sequence CancelPrevMotion(Sequence target)
    {
        if (target.IsActive())
            target.Kill();

        return DOTween.Sequence();
    }

    [Button]
    private void PlayMotionTest()
    {
        float prev = 50f;
        float next = 35f;
        float damage = prev - next;

        OnHit(prev, next, 1f, damage);
    }

    [Button]
    private void ResetData()
    {
        if (null != visualRect)
        {
            visualRect.anchoredPosition = originalAnchoredPos;
        }

        if (null != mainText)
        {
            mainText.color = finalColor;
            mainText.text = Mathf.RoundToInt(50f).ToString();
        }
    }
}
