using DG.Tweening;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

using Image = UnityEngine.UI.Image;

public class DeckSystem : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main Binding")]
    public GameObject impactEffectPrefab = null;
    [Space]
    public RectTransform wealthyRect = null;
    public RectTransform cardBackRect = null;

    private RectTransform topRect = null;
    private UIView_CardSystem cardSystem = null;
    private ParticleSystem impactParticle = null;

    [Header("Effect Location Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();

    [Header("Wealthy Settings")]
    [SerializeField] private float wealthySpeed = 1f;
    [SerializeField] private float wealthyHeight = 2f;
    [SerializeField] private float wealthyAngle = 3.5f;

    [Header("Draw Effect Settings")]
    [SerializeField] private float drawDelay = 0.15f;
    [SerializeField] private float drawDuration = 1f;
    [SerializeField] private float drawFirstPointDist = 2f;
    [Space]
    [SerializeField] private bool bMidPointRandom = false;
    [SerializeField] private float drawMidPointPower = 2f;
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
    [SerializeField] private Ease drawedCardBackEase = Ease.OutExpo;

    private Sequence activeSeq = null;
    private Sequence cardbackSeq = null;

    private Vector3 originScale = Vector3.one;
    private Quaternion originQuat = Quaternion.identity;

    private Vector3 cardbackOriginScale = Vector3.zero;

    private bool bClickedEvent = false;
    private float originalY;

    protected void Awake()
    {
        topRect = GetComponent<RectTransform>();

        originScale = topRect.localScale;
        originQuat = topRect.localRotation;

        if (null != cardBackRect)
        {
            cardbackOriginScale = cardBackRect.localScale;
        }

        impactParticle = impactEffectPrefab?.GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        if (wealthyRect != null)
            originalY = wealthyRect.localPosition.y;
    }

    private void Update()
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
        if (wealthyRect == null) return;

        float zRotation = Mathf.Cos(Time.time * wealthySpeed) * wealthyAngle;
        wealthyRect.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        float yOffset = Mathf.Sin(Time.time * wealthySpeed * 0.5f) * wealthyHeight;
        Vector3 pos = wealthyRect.localPosition;
        pos.y = originalY + yOffset;
        wealthyRect.localPosition = pos;
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
            GameObject performer = cardSystem.GetStarPerformerFromPool(this.transform);
            RectTransform rect = performer?.GetComponent<RectTransform>();
            VFX_CardStar script = performer?.GetComponent<VFX_CardStar>();
            if (null == script || null == rect)
                continue;

            Vector3 midPointPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(midPoint.position, rect);
            Vector3 endPointPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(cardSystem.GetHandTargetEndPos(i), rect);

            // mid
            if (bMidPointRandom)
            {
                midPointPos.x += Random.Range(-1f, 1f) * drawMidPointPower;
                midPointPos.y += Random.Range(-0.25f, 0.25f) * drawMidPointPower;
            }

            // first
            Vector3 firstPointPos = midPointPos;
            firstPointPos.x += drawFirstPointDist;

            Vector3[] pathPoints = { endPointPos, firstPointPos, midPointPos  };
            script.CardDataInstance = dataList[i];
            script.PlayingEventforDeck(i, currentDrawCount - 1, drawDelay, drawDuration, drawEase, pathPoints);
        }
    }

    public void CardBackDrawedEffect()
    {
        if (null == cardBackRect || null == impactParticle)
            return;

        cardBackRect.localEulerAngles = new Vector3(0f, 0f, Random.Range(-10f, 10f));
        cardBackRect.localScale = cardbackOriginScale * 0.85f;

        CancelPrevMotion(cardbackSeq);

        cardbackSeq = DOTween.Sequence();

        cardbackSeq.Append(cardBackRect.DOLocalRotate(Vector3.zero, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase)
            .OnComplete(() =>
            {
                cardBackRect.localEulerAngles = Vector3.zero;
            }));

        cardbackSeq.Join(cardBackRect.DOScale(cardbackOriginScale, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase));

        impactParticle.Stop();
        impactParticle.Play();
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

    public void InDeckFromGraveMotion()
    {
        if (null == cardBackRect || null == impactParticle)
            return;

        cardBackRect.localEulerAngles = new Vector3(0f, 0f, Random.Range(-10f, 10f));
        cardBackRect.localScale = cardbackOriginScale * 0.85f;

        CancelPrevMotion(cardbackSeq);

        cardbackSeq = DOTween.Sequence();

        cardbackSeq.Append(cardBackRect.DOLocalRotate(Vector3.zero, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase)
            .OnComplete(() =>
            {
                cardBackRect.localEulerAngles = Vector3.zero;
            }));

        cardbackSeq.Join(cardBackRect.DOScale(cardbackOriginScale, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase));

        impactParticle.Stop();
        impactParticle.Play();
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
