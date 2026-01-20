using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;

using Image = UnityEngine.UI.Image;

public class GraveyardSystem : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform topRect = null;
    public RectTransform visualRect = null;
    private Image visualImage = null;
    private UIView_CardSystem cardSystem = null;

    private Sequence activeSeq = null;
    private Sequence viualSeq = null;

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

    [Header("visualEvent Settings")]
    [SerializeField] private Vector2 visualPunchPos = Vector2.zero;
    [SerializeField] private Vector3 visualPunchScale = Vector3.zero;
    [SerializeField] private Ease visualEventEase = Ease.OutExpo;

    private Vector3 originScale = Vector3.one;
    private Quaternion originQuat = Quaternion.identity;

    private Vector3 originVisualPos = Vector3.zero;
    private Vector3 originVisualScale = Vector3.one;

    private bool bClickedEvent = false;
    private int currentMoveCnt = 0;

    private void Awake()
    {
        topRect = GetComponent<RectTransform>();

        originScale = transform.localScale;
        originQuat = transform.localRotation;

        if (visualRect)
        {
            visualImage = visualRect.GetComponent<Image>();
            originVisualPos = visualRect.localPosition;
            originVisualScale = visualRect.localScale;
        }
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

        topRect.localRotation = originQuat;

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase));

        activeSeq.Join(topRect.DOPunchRotation(upEventPunchPower, upEventDuration)
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

        MoveToDeckTopRect();

        // 묘지 > 덱 타이밍에 패널이 묘지 타입으로 열려 있다면 강제로 끔
        cardSystem.ForceDeActivatePannelSelf(CurrentPannel.Grave);
        currentMoveCnt = spawningCount;

        RectTransform midPoint = drawPathPoints[Random.Range(0, drawPathPoints.Count - 1)];
        for (int i = 0; i < spawningCount; i++)
        {
            GameObject performer = cardSystem.GetStarPerformerFromPool(this.transform);
            RectTransform rect = performer?.GetComponent<RectTransform>();
            VFX_CardStar script = performer?.GetComponent<VFX_CardStar>();
            if (null == script || null == rect)
                continue;

            Vector3 midPointPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(midPoint.position, rect);
            Vector3 endPointPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(cardSystem.GetDeckWorldPos(), rect);

            // mid
            if (bMidPointRandom)
                midPointPos.y += Random.Range(-1f, 1f) * toDeckMidPointPower;

            // first
            Vector3 firstPointPos = midPointPos;
            firstPointPos.x -= toDeckFirstPointDist;

            Vector3[] pathPoints = { endPointPos, firstPointPos, midPointPos };
            script.PlayingEventforWormHole(i, toDeckDelay, toDeckDuration, toDeckEase, pathPoints);
        }
    }

    public void CardMoveToDeckMotion()
    {
        if (null == visualRect)
            return;

        bool update = false;

        visualRect.anchoredPosition = originVisualPos;
        visualRect.localScale = originVisualScale;

        CancelPrevMotion(viualSeq);

        viualSeq = DOTween.Sequence();

        float randPosX = Random.Range(-1f, -0.1f) * visualPunchPos.x;
        float randPosY = Random.Range(-1f, -1f) * visualPunchPos.y;

        Vector3 randomPos = new Vector3(randPosX, randPosY);

        viualSeq.Append(visualRect.DOPunchAnchorPos(randomPos, toDeckDelay)
            .SetUpdate(update)
            .SetEase(visualEventEase)
            .OnComplete(() =>
            {

            }));

        viualSeq.Join(visualRect.DOPunchScale(visualPunchScale, toDeckDelay)
            .SetUpdate(update)
            .SetEase(visualEventEase));

        visualImage.DOComplete();

        Color red = visualImage.color;
        red.g -= 0.5f / currentMoveCnt;

        visualImage.DOColor(red, toDeckDelay)
            .SetUpdate(update)
            .SetEase(visualEventEase);
    }

    public void MoveToDeckFinishMotion(int idx)
    {
        if (currentMoveCnt - 1 != idx)
            return;

        visualImage.DOComplete();

        visualImage.DOColor(Color.white, 1.5f)
            .SetUpdate(false)
            .SetEase(Ease.Linear);

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale, 1.5f)
            .SetUpdate(false)
            .SetEase(Ease.OutCubic));
    }

    private void MoveToDeckTopRect()
    {
        float totalDuration = toDeckDelay * currentMoveCnt;

        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale * 1.2f, 0.1f)
            .SetUpdate(false)
            .SetEase(Ease.OutCubic));
    }

    private void CancelPrevMotion(Sequence _activeSeq)
    {
        if (null != _activeSeq && _activeSeq.IsActive())
            _activeSeq.Kill();
    }
}
