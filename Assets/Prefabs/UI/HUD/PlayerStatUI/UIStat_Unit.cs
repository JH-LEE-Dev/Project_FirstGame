using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStat_Unit : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TMP_Text mainText;

    private string titleStr;
    private Image iconImage;

    private float prevValue;
    private float targetValue;

    private Sequence animSeq;

    public void Setup(Sprite _icon, string title, float value)
    {
        iconImage = GetComponent<Image>();

        if (null == iconImage || null == mainText)
            return;

        iconImage.sprite = _icon;

        titleStr = title;
        prevValue = value;

        mainText.text = title + value.ToString("0.#");
    }

    public void ValueChange(float _current, float duration = 0.5f)
    {
        CancelPrevMotion(animSeq);

        animSeq = DOTween.Sequence();
        animSeq.SetUpdate(false);

        targetValue = _current;
        animSeq.Append(DOVirtual.Float(prevValue, _current, duration, UpdateValueChange));

        if (prevValue < targetValue)
            animSeq.Join(mainText.DOColor(Color.green, duration));
        else
            animSeq.Join(mainText.DOColor(Color.white, duration));

        animSeq.OnComplete(CompleteCallback);
    }

    private void CompleteCallback()
    { 
        prevValue = targetValue;
    }

    private void UpdateValueChange(float _current)
    {
        if (null == mainText)
            return;

        mainText.text = titleStr + _current.ToString("0.#");
    }

    private void CancelPrevMotion(Sequence seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        seq = null;
    }

    private void OnDisable()
    {
        CancelPrevMotion(animSeq);
    }
}
