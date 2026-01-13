using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TextMotion : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private float motionDuration = 1f;
    [SerializeField] private Ease motionEase = Ease.Linear;

    public void OnHit(float _prev, float _current)
    {
        if (null == mainText)
            return;

        DOVirtual.Float(_prev, _current, motionDuration, (value) =>
        {
            mainText.text = Mathf.RoundToInt(value).ToString();
        }).SetEase(motionEase).SetUpdate(false);
    }
    
    
    public void Init<T>(T _value) where T : struct
    {
        if (mainText == null || (typeof(T) != typeof(int) && typeof(T) != typeof(float)))
            return;

        float convertedValue = Convert.ToSingle(_value);
        Debug.Log(convertedValue);
        mainText.text = Mathf.RoundToInt(convertedValue).ToString();
    }
}
