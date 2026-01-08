using UnityEngine;

[DisallowMultipleComponent]
public class CardVisualFloat : MonoBehaviour
{
    private CardInstance owner;
    private RectTransform visual;

    private Vector2 basePos;
    private float seed;

    [Header("Hand Float")]
    [SerializeField] private float handFloatPosAmp = 0.5f;
    [SerializeField] private float handFloatRotAmp = 0.2f;
    [SerializeField] private float handFloatFreq = 0.2f;

    [Header("Preview Float")]
    [SerializeField] private float previewFloatPosAmp = 0.5f;
    [SerializeField] private float previewFloatRotAmp = 0.15f;
    [SerializeField] private float previewFloatFreq = 0.2f;

    public void Bind(CardInstance card)
    {
        owner = card;
        visual = GetComponent<RectTransform>();
        basePos = visual.anchoredPosition;
        seed = UnityEngine.Random.Range(0f, 1000f);
    }
    
    private void Update()
    {
        if (owner == null || owner.Motion == null) return;

        if (owner.Motion.IgnoreHandLayout) PreviewFloating();
        else HandFloating();
    }

    private void HandFloating()
    {
        float t = Time.unscaledTime + seed;
        float w = handFloatFreq * Mathf.PI * 2f;

        float x = Mathf.Sin(t * w) * handFloatPosAmp;
        float y = Mathf.Cos(t * w * 1.13f) * (handFloatPosAmp * 0.8f);
        float rz = Mathf.Sin(t * w * 0.9f) * handFloatRotAmp;

        visual.anchoredPosition = basePos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }

    private void PreviewFloating()
    {
        float t = Time.unscaledTime + seed;
        float w = previewFloatFreq * Mathf.PI * 2f;

        float x = Mathf.Sin(t * w) * previewFloatPosAmp;
        float y = Mathf.Cos(t * w * 1.07f) * (previewFloatPosAmp * 0.8f);
        float rz = Mathf.Sin(t * w * 0.85f) * previewFloatRotAmp;

        visual.anchoredPosition = basePos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }
}
