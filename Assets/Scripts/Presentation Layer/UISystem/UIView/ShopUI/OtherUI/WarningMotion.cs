using System;
using DG.Tweening;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class WarningMotion : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float waitToFinish = .5f;
    [SerializeField] private Ease ease = Ease.OutExpo;
    [SerializeField] private float power = 5f;

    private RectTransform ownerRt;
    private Image mainImage;
    private TMP_Text mainText;

    private Sequence seq;

    private Vector3 originRot;

    private Action callback;

    private void Awake()
    {
        ownerRt = GetComponent<RectTransform>();
        mainImage = GetComponentInChildren<Image>();
        mainText = GetComponentInChildren<TMP_Text>();

        originRot = ownerRt.eulerAngles;
    }

    public void PlayMotion(Action _callback)
    {
        if (null == ownerRt || null == mainText || null == mainImage)
            return;

        if (null != seq && seq.IsActive())
            seq.Kill();

        callback = _callback;
        ownerRt.eulerAngles = originRot;

        seq = DOTween.Sequence();

        seq.Append(mainImage.DOFade(1f, duration).SetEase(ease));
        seq.Join(mainText.DOFade(1f, duration).SetEase(ease));
        seq.Join(ownerRt.DOScale(1f, duration).SetEase(ease));
        seq.Join(ownerRt.DOPunchRotation(new Vector3(0f, 0f, power), duration + waitToFinish));

        seq.AppendInterval(waitToFinish);

        seq.Append(mainImage.DOFade(0f, duration).SetEase(ease));
        seq.Join(mainText.DOFade(0f, duration).SetEase(ease));

        seq.OnComplete(OnCompleted);
        seq.SetUpdate(false);
    }

    private void OnCompleted()
    {
        callback?.Invoke();
        ownerRt.eulerAngles = originRot;
    }

    private void OnDisable()
    {
        seq.Kill();
    }
}
