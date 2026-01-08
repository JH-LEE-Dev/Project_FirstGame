using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

using Random = UnityEngine.Random;
using Range = UnityEngine.RangeAttribute;



public class CardInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardDataInstance cardData;
    public CardDataInstance CardData => cardData;

    private UIView_CardSystem cardSystem;

    RectTransform rt;

    [Header("MainMoving")]
    Vector2 targetPos;
    float targetAngleZ;
    [SerializeField] float followFreq = 18f;
    [SerializeField, Range(0f, 1.2f)] float followDamp = 0.85f;
    [SerializeField] float rotateLerp = 18f;   // 회전 추종 속도
    [SerializeField] float snapDist = 0.05f;   // 미세 떨림 제거용


    [Header("Hover")]
    [SerializeField] public float hoverScale = 1.3f;
    [SerializeField] public float duration = 0.15f;
    private Tween hoverTween;
    private Vector3 originScale;
    Vector2 velocity; // 스프링 속도(내부 상태)


    [Header("Preview")]
    [SerializeField] private bool ignoreHandLayout = false; // 프리뷰 중이면 true
    [SerializeField] private float previewScale = 3f;
    [SerializeField] private float previewMoveDuration = 0.3f;
    [SerializeField] private float previewScaleDuration = 0.3f;
    private Tween previewMoveTween;
    private Tween previewScaleTween;
    private Tween previewRotateTween;
    [SerializeField] private float previewEndScaleDur = 0.5f;
    private Tween previewEndScaleTween;

    [Header("Visual")]
    private Vector2 visualBaseLocalPos;
    private float seed;
    [SerializeField] private RectTransform visual;
    [SerializeField] private float handFloatPosAmp = 1f;
    [SerializeField] private float handFloatRotAmp = 0.3f;
    [SerializeField] private float handFloatFreq = 0.4f;
    [SerializeField] private float previewFloatPosAmp = 0.5f;
    [SerializeField] private float previewFloatRotAmp = 0.15f;
    [SerializeField] private float previewFloatFreq = 0.2f;

    // 카드 부모를 옮길 때 복구용으로 사용할 변수들임
    private Transform originParentTransform;
    public Transform OriginParentTrasnform 
    {  
        get { return originParentTransform; } 
        set { originParentTransform = value; } 
    }
    ///////////////////////////////////////////////
    
    public void RollbackParent()
    {
        transform.SetParent(originParentTransform);
    }

    public void EnterHand()
    {
        ignoreHandLayout = false;
        velocity = Vector2.zero;
    }

    public void ExitHand()
    {
        ignoreHandLayout = false;
        velocity = Vector2.zero;
        KillHover();
        transform.localScale = originScale; // 수정 필요.
    }

    /// ////////////////


    void Awake()
    {
        rt = GetComponent<RectTransform>();

        originScale = transform.localScale;
        targetPos = rt.anchoredPosition;

        if (visual != null)
            visualBaseLocalPos = visual.anchoredPosition;

        seed = Random.Range(0f, 1000f); // 카드마다 흔들림 타이밍이 다르게

        originParentTransform = transform.parent;
    }
    public void Initialize(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    void Update()
    {
        // 패에 있을 때, 연출
        InHand();

        Floating();
    }


    public void ApplyData(CardDataInstance _cardData)
    {
        cardData = _cardData;
    }

    public void Clear()
    {
        cardData = null;
    }

    //////////////////////// 연출

    public void InHand()
    {
        // 프리뷰 중일때.
        if (ignoreHandLayout) return;

        float dt = Time.unscaledDeltaTime;

        Vector2 pos = rt.anchoredPosition;

        float k = followFreq * followFreq;
        float c = 2f * followDamp * followFreq;

        Vector2 accel = k * (targetPos - pos) - c * velocity;
        velocity += accel * dt;
        pos += velocity * dt;

        if ((pos - targetPos).sqrMagnitude < snapDist * snapDist)
        {
            pos = targetPos;
            velocity = Vector2.zero;
        }

        rt.anchoredPosition = pos;

        float currentZ = rt.localEulerAngles.z;
        float z = Mathf.LerpAngle(
            currentZ,
            targetAngleZ,
            1f - Mathf.Exp(-rotateLerp * dt)
        );

        rt.localRotation = Quaternion.Euler(0, 0, z);
    }

    private void Floating()
    {
        if (visual == null) return;

        if (ignoreHandLayout) PreviewFloating();
        else HandFloating();
    }

    private void HandFloating()
    {
        float t = Time.unscaledTime + seed;

        float posAmp = handFloatPosAmp;
        float rotAmp = handFloatRotAmp;
        float w = handFloatFreq * Mathf.PI * 2f;

        // 좌, 상하
        float x = Mathf.Sin(t * w) * posAmp;
        float y = Mathf.Cos(t * w * 1.13f) * (posAmp * 0.8f);

        // 미세한 Z회전
        float rz = Mathf.Sin(t * w * 0.9f) * rotAmp;

        visual.anchoredPosition = visualBaseLocalPos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }

    private void PreviewFloating()
    {
        float t = Time.unscaledTime + seed;

        float posAmp = previewFloatPosAmp;
        float rotAmp = previewFloatRotAmp;
        float w = previewFloatFreq * Mathf.PI * 2f;

        float x = Mathf.Sin(t * w) * posAmp;
        float y = Mathf.Cos(t * w * 1.07f) * (posAmp * 0.8f);
        float rz = Mathf.Sin(t * w * 0.85f) * rotAmp;

        visual.anchoredPosition = visualBaseLocalPos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }

    public void UpdateTargetPos(Vector3 tp, float angleZ)
    {
        targetPos = (Vector2)tp;
        targetAngleZ = angleZ;
    }

    public void KillHover()
    {
        hoverTween?.Kill();
        transform.localScale = originScale;
    }

    // 프리뷰 시작
    public void StartPreview(Vector2 centerPos)
    {
        ignoreHandLayout = true;

        // 호버 트윈 파괴
        KillHover();

        previewMoveTween?.Kill();
        previewScaleTween?.Kill();
        previewRotateTween?.Kill();

        previewMoveTween = rt.DOAnchorPos(centerPos, previewMoveDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        previewScaleTween = transform.DOScale(originScale * previewScale, previewScaleDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        targetAngleZ = 0f;

        previewRotateTween = rt.DOLocalRotate(Vector3.zero, previewMoveDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    // 프리뷰 종료
    public void EndPreview()
    {
        ignoreHandLayout = false;

        previewMoveTween?.Kill();
        previewScaleTween?.Kill();
        previewRotateTween?.Kill();
        hoverTween?.Kill();

        velocity = Vector2.zero;

        previewEndScaleTween?.Kill();
        previewEndScaleTween = transform.DOScale(originScale, previewEndScaleDur)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

    }



    ///////////////////// 입력

    // 호버 ON
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ignoreHandLayout) return;


        cardSystem?.OnCardHoverEnter(this);


        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale * hoverScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    // 호버 OFF
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ignoreHandLayout) return;


        cardSystem?.OnCardHoverExit(this);


        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    // 클릭
    public void OnPointerClick(PointerEventData eventData)
    {
        // 상우 : 뽑는 연출 중에는 모든 카드를 사용할 수 없게 해줘

        // 마우스 우클릭
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            cardSystem?.TryUseCard(this);
            return;
        }

        // 마우스 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            cardSystem?.OnCardLeftClick(this);
            return;
        }
    }

}
