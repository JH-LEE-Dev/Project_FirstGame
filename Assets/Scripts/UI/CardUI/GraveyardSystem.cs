using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GraveyardSystem : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform visualRect = null;
    private UIView_CardSystem cardSystem = null;

    private Sequence activeSeq = null;

    [Header("Enter Event Settings")]
    [SerializeField] private float enterEventDuration = 0.4f;
    [SerializeField] private float enterEventSizeMulti = 1.15f;
    [SerializeField] private Ease enterEventEase = Ease.OutExpo;

    [Header("Exit Event Settings")]
    [SerializeField] private float exitEventDuration = 0.4f;
    [SerializeField] private Ease exitEventEase = Ease.OutExpo;

    [Header("Down Event Settings")]
    [SerializeField] private float downEventDuration = 0.4f;
    [SerializeField] private Ease downEventEase = Ease.OutExpo;

    [Header("Up Event Settings")]
    [SerializeField] private float upEventDuration = 0.4f;
    [SerializeField] private Vector3 upEventPunchPower = Vector3.zero;
    [SerializeField] private Ease upEventEase = Ease.OutExpo;

    private Vector3 originScale = Vector3.one;
    private Quaternion originQuat = Quaternion.identity;

    private bool bClickedEvent = false;

    private void Awake()
    {
        visualRect = GetComponent<RectTransform>();

        originScale = transform.localScale;
        originQuat = transform.localRotation;
    }

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DownEvent();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UpEvent();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterEvent();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitEvent();
    }

    private void EnterEvent()
    {
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(visualRect.DOScale(originScale * enterEventSizeMulti, enterEventDuration)
            .SetUpdate(false)
            .SetEase(enterEventEase));
    }

    private void ExitEvent()
    {
        if (bClickedEvent)
            return;

        visualRect.localRotation = originQuat;

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(visualRect.DOScale(originScale, exitEventDuration)
            .SetUpdate(false)
            .SetEase(exitEventEase));
    }

    private void DownEvent()
    {
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(visualRect.DOScale(originScale, downEventDuration)
            .SetUpdate(false)
            .SetEase(downEventEase));
    }

    private void UpEvent()
    {
        bClickedEvent = true;

        visualRect.localRotation = originQuat;

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(visualRect.DOScale(originScale, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase));

        activeSeq.Join(visualRect.DOPunchRotation(upEventPunchPower, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase)
            .OnComplete(() =>
            {
                bClickedEvent = false;
            }));

        // ³ªÁß¿¡ µ¦ ¸»°í ¹¦Áö·Î ¹Ù²Ù¸é µÊ.
        cardSystem?.CallGravePannel();
    }

    private void CancelPrevMotion(Sequence _activeSeq)
    {
        if (null != _activeSeq && _activeSeq.IsActive())
            _activeSeq.Kill();
    }
}
