using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GraveyardSystem : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform visualRect = null;
    private UIView_CardSystem cardSystem = null;

    private Sequence activeSeq = null;

    [Header("Effect Location Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();

    [Header("toDeck Effect Settings")]
    [SerializeField] private float toDeckDelay = 0.02f;
    [SerializeField] private float toDeckDuration = 1f;
    [SerializeField] private float toDeckFirstPointDist = 2f;
    [Space]
    [SerializeField] private bool bMidPointRandom = false;
    [SerializeField] private float toDeckMidPointPower = 2f;
    [SerializeField] private Ease toDeckEase = Ease.OutQuad;

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

        cardSystem?.CallPannel(CurrentPannel.Grave);
    }

    public void CardMoveToDeckEffect(int spawningCount)
    {
        if (null == cardSystem)
            return;

        // 묘지 > 덱 타이밍에 패널이 묘지 타입으로 열려 있다면 강제로 끔
        cardSystem.ForceDeActivatePannelSelf(CurrentPannel.Grave);

        RectTransform midPoint = drawPathPoints[Random.Range(0, drawPathPoints.Count - 1)];
        for (int i = 0; i < spawningCount; i++)
        {
            GameObject performer = cardSystem.GetStarPerformerFromPool();
            StarEffect script = performer?.GetComponent<StarEffect>();
            if (null == script)
                continue;

            script.AttachTo(this.transform);

            Vector3 midPointPos = midPoint.position;
            Vector3 endPointPos = cardSystem.GetDeckWorldPos();

            // mid
            if (bMidPointRandom)
                midPointPos.y += Random.Range(-0.35f, 0.35f) * toDeckMidPointPower;

            // first
            Vector3 firstPointPos = midPointPos;
            firstPointPos.x -= toDeckFirstPointDist;

            Vector3[] pathPoints = { endPointPos, firstPointPos, midPointPos };
            script.PlayingEventforWormHole(i, toDeckDelay, toDeckDuration, toDeckEase, pathPoints);
        }
    }

    private void CancelPrevMotion(Sequence _activeSeq)
    {
        if (null != _activeSeq && _activeSeq.IsActive())
            _activeSeq.Kill();
    }
}
