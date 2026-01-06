using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

using Random = UnityEngine.Random;
using Range = UnityEngine.RangeAttribute;



public class CardInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CardData cardData;
    public List<CardEffectData> additionalEffectData;
    public event Action<CardInstance> CardUsedEvent;


    // YW

    public UIView_CardSystem cardSystem;

    // Hover
    [SerializeField] public float hoverScale = 1.5f;
    [SerializeField] public float duration = 0.15f;
    private Tween hoverTween;
    private Vector3 originScale;
    Vector2 velocity; // 스프링 속도(내부 상태)

    RectTransform rt;

    // position
    Vector2 targetPos;
    float targetAngleZ;

    [SerializeField] float followFreq = 18f;
    [SerializeField, Range(0f, 1.2f)] float followDamp = 0.85f;
    [SerializeField] float rotateLerp = 18f;   // 회전 추종 속도
    [SerializeField] float snapDist = 0.05f;   // 미세 떨림 제거용


    [Header("Float")]
    [SerializeField] float floatAmplitude = 6f;   // 픽셀 단위 (아주 작게!)
    [SerializeField] float floatSpeed = 1.2f;     // 느리게
    [SerializeField] float floatPhaseRandom = 1f; // 카드마다 위상 차이
    float floatPhase;

    public bool inHand = true;

    /// ////////////////


    void Awake()
    {
        rt = GetComponent<RectTransform>();


        // DoTween 움직임을 위한 변수들
        originScale = transform.localScale;
        targetPos = rt.anchoredPosition;
        floatPhase = Random.value * Mathf.PI * 2f * floatPhaseRandom;
    }

    void Update()
    {

        // 패에 있을 때, 연출
        InHand();

    }

    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;   
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void AddCardEffect(CardEffectData effect)
    {
        additionalEffectData.Add(effect);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardUsedEvent?.Invoke(this);
    }



    // UIView_CardSystem을 받아옴. (패 매니저)
    public void SetMaker(UIView_CardSystem cs)
    {
        cardSystem = cs;
    }

    // 호버 ON
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardSystem?.OnCardHoverEnter(this);


        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale * hoverScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    // 호버 OFF
    public void OnPointerExit(PointerEventData eventData)
    {
        cardSystem?.OnCardHoverExit(this);


        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }


    public void InHand()
    {
        if (inHand == false) return;


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

        ///
    }

    public void UpdateTargetPos(Vector3 tp, float angleZ)
    {
        targetPos = (Vector2)tp;
        targetAngleZ = angleZ;
    }
}
