using DG.Tweening;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class DeckSystem : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main Binding")]
    public GameObject drawEffectPrefab = null;
    public RectTransform wealthyRect = null;
    public RectTransform cardBackRect = null;
    private RectTransform topRect = null;
    private UIView_CardSystem cardSystem = null;

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
    [SerializeField] private Vector3 upEventPunchPower = Vector3.zero;
    [SerializeField] private Ease upEventEase = Ease.OutExpo;

    private Sequence wealthySeq = null;
    private Sequence activeSeq = null;

    private ObjectPool<GameObject> drawEffectParticle;

    private Vector3 originScale = Vector3.one;
    private Quaternion originQuat = Quaternion.identity;

    private int currentDrawCount = 0;

    private bool bClickedEvent = false;

    protected void Awake()
    {
        topRect = GetComponent<RectTransform>();

        originScale = topRect.localScale;
        originQuat = topRect.localRotation;

        drawEffectParticle = new ObjectPool<GameObject>(
            createFunc: CreateDrawEffect,
            actionOnGet: ActivateDrawEffect,
            actionOnRelease: DeActivateDrawEffect,
            actionOnDestroy: DestroyPoolObj,
            maxSize: 15);

        for (int i = 0; i < 15; ++i)
        {
            GameObject newObj = drawEffectParticle.Get();
            drawEffectParticle.Release(newObj);
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

        RectTransform midPoint = cardSystem.DrawPathPoints[Random.Range(0, cardSystem.DrawPathPoints.Count - 1)];

        currentDrawCount = dataList.Count;
        for (int i = 0; i < currentDrawCount; i++)
        {
            GameObject performer = drawEffectParticle.Get();
            DrawEffect script = performer?.GetComponent<DrawEffect>();
            if (null == script)
                continue;

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
            script.PlayingDrawEvent(i, drawDelay, drawDuration, drawEase, pathPoints);
        }
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
            .OnComplete(()=>
            {
                bClickedEvent = false;
            }));

        cardSystem?.CallPannel(CurrentPannel.Deck);
    }

    public void CallOneCardDrawCompleted(int _idx, Vector3 _endPos, CardDataInstance _data, GameObject _performer)
    {
        if (null == cardSystem)
            return;

        if (currentDrawCount - 1 == _idx)
            cardSystem.WorkingBlock = false;

        cardSystem.DrawEvent.Invoke(_endPos, _data);
        drawEffectParticle.Release(_performer);
    }

    private GameObject CreateDrawEffect()
    {
        GameObject newObj = Instantiate(drawEffectPrefab, topRect);
        DrawEffect script = newObj?.GetComponent<DrawEffect>();
        script?.Init(this);

        return newObj;
    }

    private void DestroyPoolObj(GameObject obj)
    {
        Destroy(obj);
    }

    private void ActivateDrawEffect(GameObject obj)
    {
        obj?.SetActive(true);
    }

    private void DeActivateDrawEffect(GameObject obj)
    {
        if (null == obj)
            return;

        obj.transform.position = topRect.position;
        obj.SetActive(false);
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
