using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseDeckSystem : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    [Header("Main Binding")]
    public RectTransform wealthyRect = null;
    public RectTransform cardBackRect = null;

    protected RectTransform topRect = null;

    [Header("Wealthy Settings")]
    [SerializeField] protected float wealthySpeed = 1f;
    [SerializeField] protected float wealthyHeight = 2f;
    [SerializeField] protected float wealthyAngle = 3.5f;

    [Header("Draw Effect Settings")]
    [SerializeField] protected float drawDelay = 0.15f;
    [SerializeField] protected float drawDuration = 1f;
    [SerializeField] protected float drawDragPower = 250f;
    [SerializeField] protected Ease drawEase = Ease.OutQuad;

    [Header("Enter Event Settings")]
    [SerializeField] protected float enterEventDuration = 0.4f;
    [SerializeField] protected float enterEventSizeMulti = 1.15f;
    [SerializeField] protected Ease enterEventEase = Ease.OutExpo;

    [Header("Exit Event Settings")]
    [SerializeField] protected float exitEventDuration = 0.4f;
    [SerializeField] protected Ease exitEventEase = Ease.OutExpo;

    [Header("Down Event Settings")]
    [SerializeField] protected float downEventDuration = 0.4f;
    [SerializeField] protected Ease downEventEase = Ease.OutExpo;

    [Header("Up Event Settings")]
    [SerializeField] protected float upEventDuration = 0.4f;
    [SerializeField] protected Vector3 upEventStartRot = Vector3.zero;
    [SerializeField] protected Ease upEventEase = Ease.OutExpo;

    protected Sequence activeSeq = null;
    protected Sequence cardbackSeq = null;

    protected Vector3 originScale = Vector3.one;
    protected Quaternion originQuat = Quaternion.identity;

    protected Vector3 cardbackOriginScale = Vector3.zero;

    protected bool bClickedEvent = false;
    protected float originalY;

    protected virtual void Awake()
    {
        topRect = GetComponent<RectTransform>();

        originScale = topRect.localScale;
        originQuat = topRect.localRotation;

        if (null != cardBackRect)
        {
            cardbackOriginScale = cardBackRect.localScale;
        }
    }

    protected virtual void Start()
    {
        if (wealthyRect != null)
            originalY = wealthyRect.localPosition.y;
    }

    protected virtual void Update()
    {
        WealthyMotion();
    }

    protected virtual void OnDisable()
    {
        transform.DOKill();
        wealthyRect.DOKill();
        cardBackRect.DOKill();
    }

    private void WealthyMotion()
    {
        if (wealthyRect == null) return;

        float zRotation = Mathf.Cos(Time.time * wealthySpeed) * wealthyAngle;
        wealthyRect.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        float yOffset = Mathf.Sin(Time.time * wealthySpeed * 0.5f) * wealthyHeight;
        Vector3 pos = wealthyRect.localPosition;
        pos.y = originalY + yOffset;
        wealthyRect.localPosition = pos;
    }
    private void EnterEvent()
    {
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale * enterEventSizeMulti, enterEventDuration)
            .SetUpdate(false)
            .SetEase(enterEventEase));
    }

    private void ExitEvent()
    {
        if (bClickedEvent)
            return;

        topRect.localRotation = originQuat;

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale, exitEventDuration)
            .SetUpdate(false)
            .SetEase(exitEventEase));
    }

    private void DownEvent()
    {
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale, downEventDuration)
            .SetUpdate(false)
            .SetEase(downEventEase));
    }

    private void UpEvent()
    {
        bClickedEvent = true;

        topRect.localEulerAngles = upEventStartRot;

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase));

        activeSeq.Join(topRect.DORotate(Vector3.zero, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase)
            .OnComplete(UpEventCompleteEvent));
    }

    public virtual void OnPointerDown(PointerEventData _eventData)
    {
        DownEvent();
    }

    public virtual void OnPointerUp(PointerEventData _eventData)
    {
        UpEvent();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }

    public virtual void OnPointerEnter(PointerEventData _eventData)
    {
        EnterEvent();
    }

    public virtual void OnPointerExit(PointerEventData _eventData)
    {
        ExitEvent();
    }

    protected virtual void CancelPrevMotion(Sequence _activeSeq)
    {
        if (null != _activeSeq && _activeSeq.IsActive())
            _activeSeq.Kill();
    }

    protected virtual void CardBackDrawedEffectCompleteEvent()
    {
        cardBackRect.localEulerAngles = Vector3.zero;
    }

    protected virtual void UpEventCompleteEvent()
    {
        bClickedEvent = false;
    }
}
