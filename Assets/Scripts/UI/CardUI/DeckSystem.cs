using DG.Tweening;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

public class DeckSystem : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main Binding")]
    public GameObject drawEffectPrefab = null;
    public RectTransform wealthyRect = null;
    public RectTransform cardBackRect = null;
    private RectTransform topRect = null;
    private UIView_CardSystem cardSystem = null;

    [Header("Effect Location Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();

    [Header("Wealthy Settings")]
    [SerializeField] private float wealthyDuration = 1f;
    [SerializeField] private float wealthyAngle = 5f;
    [SerializeField] private Ease wealthyEase = Ease.Linear;

    [Header("Draw Effect Settings")]
    [SerializeField] private float drawDelay = 0.15f;
    [SerializeField] private float drawDuration = 1f;
    [SerializeField] private float drawFirstPointDist = 2f;
    [Space]
    [SerializeField] private bool bMidPointRandom = false;
    [SerializeField] private float drawMidPointPower = 2f;
    [Space]
    [SerializeField] private bool bEndPointRandom = false;
    [SerializeField] private float drawEndPointPower = 2f;
    [SerializeField] private Ease drawEase = Ease.OutQuad;

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
    [SerializeField] private Vector3 upEventStartRot = Vector3.zero;
    [SerializeField] private Ease upEventEase = Ease.OutExpo;

    [Header("CardBack Event for Drawed")]
    [SerializeField] private Vector2 drawedCardBackPunchPosMulti = Vector3.zero;
    [SerializeField] private Vector3 drawedCardBackPunchScale = Vector3.zero;
    [SerializeField] private Vector3 drawedCardBackPunchRot = Vector3.zero;
    [SerializeField] private Ease drawedCardBackEase = Ease.OutExpo;

    private Sequence wealthySeq = null;
    private Sequence activeSeq = null;
    private Sequence cardbackSeq = null;

    private Vector3 originScale = Vector3.one;
    private Quaternion originQuat = Quaternion.identity;

    private Vector3 cardbackOriginPos = Vector3.zero;
    private Vector3 cardbackOriginScale = Vector3.zero;
    private Quaternion cardbackOriginRot = Quaternion.identity;

    private bool bClickedEvent = false;

    protected void Awake()
    {
        topRect = GetComponent<RectTransform>();

        originScale = topRect.localScale;
        originQuat = topRect.localRotation;

        if (null != cardBackRect)
        {
            cardbackOriginPos = cardBackRect.anchoredPosition;
            cardbackOriginScale = cardBackRect.localScale;
            cardbackOriginRot = cardBackRect.rotation;
        }
    }

    private void Start()
    {
        WealthyMotion();
    }

    private void OnDisable()
    {
        transform.DOKill();
        wealthyRect.DOKill();
        cardBackRect.DOKill();
    }

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    private void WealthyMotion()
    {
        if (null == wealthyRect)
            return;

        wealthyRect.localRotation = Quaternion.Euler(0f, 0f, wealthyAngle);

        wealthySeq = DOTween.Sequence();

        wealthySeq.Append(wealthyRect.DOLocalRotate(new Vector3(0f, 0f, -wealthyAngle), wealthyDuration, RotateMode.Fast)
            .SetUpdate(false)
            .SetEase(wealthyEase));

        wealthySeq.SetLoops(-1, LoopType.Yoyo);
    }

    public void CardDrawEffect(List<CardDataInstance> dataList)
    {
        if (null == cardSystem)
            return;

        // 드로우 타이밍에 패널이 덱 타입으로 열려 있다면 강제로 끔
        cardSystem.ForceDeActivatePannelSelf(CurrentPannel.Deck);

        RectTransform midPoint = drawPathPoints[Random.Range(0, drawPathPoints.Count - 1)];

        int currentDrawCount = dataList.Count;
        for (int i = 0; i < currentDrawCount; i++)
        {
            GameObject performer = cardSystem.GetStarPerformerFromPool();
            StarEffect script = performer?.GetComponent<StarEffect>();
            if (null == script)
                continue;

            script.AttachTo(this.transform);

            Vector3 midPointPos = midPoint.position;
            Vector3 endPointPos = cardSystem.GetHandTargetEndPos(i);

            // mid
            if (bMidPointRandom)
            {
                midPointPos.x += Random.Range(-1f, 1f) * drawMidPointPower;
                midPointPos.y += Random.Range(-0.25f, 0.25f) * drawMidPointPower;
            }

            // first
            Vector3 firstPointPos = midPointPos;
            firstPointPos.x += drawFirstPointDist;

            // end
            if (bEndPointRandom)
                endPointPos.x += Random.Range(-1f, 1f) * drawEndPointPower;

            Vector3[] pathPoints = { endPointPos, firstPointPos, midPointPos  };

            script.CardDataInstance = dataList[i];
            script.PlayingEventforDeck(i, dataList.Count - 1, drawDelay, drawDuration, drawEase, pathPoints);
        }
    }

    public void CardBackDrawedEffect()
    {
        if (null == cardBackRect)
            return;

        cardBackRect.anchoredPosition = cardbackOriginPos;
        cardBackRect.localScale = cardbackOriginScale;

        CancelPrevMotion(cardbackSeq);

        cardbackSeq = DOTween.Sequence();

        float randPosX = Random.Range(0.1f, 1f) * drawedCardBackPunchPosMulti.x;
        float randPosY = Random.Range(0.1f, 1f) * drawedCardBackPunchPosMulti.y;

        Vector3 randomPos = new Vector3(randPosX, randPosY);

        cardbackSeq.Append(cardBackRect.DOPunchAnchorPos(randomPos, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase)
            .OnComplete(() =>
            {
                
            }));

        cardbackSeq.Join(cardBackRect.DOPunchScale(drawedCardBackPunchScale, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase));
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
            .OnComplete(() =>
            {
                bClickedEvent = false;
            }));

        cardSystem?.CallPannel(CurrentPannel.Deck);
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        if (true == cardSystem?.WorkingBlock)
            return;

        DownEvent();
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        if (true == cardSystem?.WorkingBlock)
            return;

        UpEvent();
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        EnterEvent();
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        ExitEvent();
    }

    private void CancelPrevMotion(Sequence _activeSeq)
    {
        if (null != _activeSeq && _activeSeq.IsActive())
            _activeSeq.Kill();
    }
}
