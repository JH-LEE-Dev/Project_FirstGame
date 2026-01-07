using DG.Tweening;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class Deck : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main Binding")]
    public RectTransform wealthyRect = null;
    public RectTransform cardBackRect = null;
    private UIView_CardSystem cardSystem = null;

    [Header("Wealthy Settings")]
    [SerializeField] private float wealthyDuration = 1f;
    [SerializeField] private float wealthyAngle = 5f;
    [SerializeField] private Ease wealthyEase = Ease.Linear;

    [Header("DrawEffect Settings")]
    [SerializeField] private float drawDuration = 1f;
    [SerializeField] private float drawFirstPointDist = 2f;
    [SerializeField] private float drawMidPointPower = 2f;
    [SerializeField] private Ease drawEase = Ease.OutQuad;

    private Sequence wealthySeq = null;

    private ObjectPool<GameObject> drawEffectParticle;

    protected void Awake()
    {
        
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

    public void Initialize(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    private void WealthyMotion()
    {
        if (null == wealthyRect)
            return;

        wealthySeq = DOTween.Sequence();

        wealthySeq.Append(wealthyRect.DOLocalRotate(new Vector3(0f, 0f, wealthyAngle), wealthyDuration, RotateMode.Fast)
          .SetEase(Ease.InOutSine));

        wealthySeq.Append(wealthyRect.DOLocalRotate(new Vector3(0f, 0f, -wealthyAngle), wealthyDuration, RotateMode.Fast)
          .SetEase(Ease.InOutSine));

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

    public void OnPointerDown(PointerEventData _eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        
    }

    private void CancelPrevMotion(Sequence _activeSeq)
    {
        if (null != _activeSeq && _activeSeq.IsActive())
            _activeSeq.Kill();
    }
}
