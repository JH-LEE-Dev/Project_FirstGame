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
    public RectTransform wealthyRect = null;
    public RectTransform cardBackRect = null;
    private RectTransform topRect = null;
    private UIView_CardSystem cardSystem = null;

    [Header("Wealthy Settings")]
    [SerializeField] private float wealthyDuration = 1f;
    [SerializeField] private float wealthyAngle = 5f;
    [SerializeField] private Ease wealthyEase = Ease.Linear;

    [Header("Draw Effect Settings")]
    [SerializeField] private float drawDuration = 1f;
    [SerializeField] private float drawFirstPointDist = 2f;
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
    [SerializeField] private Vector3 upEventPunchPower = Vector3.zero;
    [SerializeField] private Ease upEventEase = Ease.OutExpo;

    private bool bPlayingUpEvent = false;
    private bool bHoveringEvent = false;

    private Sequence wealthySeq = null;
    private Sequence activeSeq = null;

    private ObjectPool<GameObject> drawEffectParticle;

    private Vector3 originScale = Vector3.one;
    private Quaternion originQuat = Quaternion.identity;

    protected void Awake()
    {
        topRect = GetComponent<RectTransform>();

        originScale = topRect.localScale;
        originQuat = topRect.localRotation;
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

    private void CardDrawEffect()
    {
        if (null == cardSystem)
        {
            Debug.Log("오류: 카드 시스템 바인딩 안 되어있음.");
            return;
        }

        RectTransform midPoint = cardSystem.DrawPathPoints[Random.Range(0, cardSystem.DrawPathPoints.Count - 1)];
        RectTransform endPoint = cardSystem.DrawEndPoint;

        // 첫 번째 경로를 자연스럽게 하기 위한 인위적인 경로
        Vector3 firstPointPos = transform.position + transform.up * drawFirstPointDist;
        Vector3 midPointPos = midPoint.position * Random.Range(-1f, 1f) * drawMidPointPower;
        Vector3 endPointPos = endPoint.position;
        endPointPos.x += Random.Range(-1f, 1f) * 3f;

        Vector3[] pathPoints = { firstPointPos, midPointPos, endPointPos };

        // 파티클 풀링한 거 여기서 차례대로 꺼내서 위치 이동 시키기
    }

    private void EnterEvent()
    {
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale * enterEventSizeMulti, enterEventDuration)
            .SetUpdate(false)
            .SetEase(enterEventEase));

        bHoveringEvent = true;
    }

    private void ExitEvent()
    {
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale, exitEventDuration)
            .SetUpdate(false)
            .SetEase(exitEventEase));

        bHoveringEvent = false;
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
        CancelPrevMotion(activeSeq);

        activeSeq = DOTween.Sequence();

        activeSeq.Append(topRect.DOScale(originScale * enterEventSizeMulti, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase));

        activeSeq.Join(topRect.DOPunchRotation(upEventPunchPower, upEventDuration)
            .SetUpdate(false)
            .SetEase(upEventEase)
            .OnComplete(() =>
            {
                bPlayingUpEvent = false;
                topRect.localRotation = originQuat;

                if (!bHoveringEvent)
                    ExitEvent();
            }));

        bPlayingUpEvent = true;
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        DownEvent();
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
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
