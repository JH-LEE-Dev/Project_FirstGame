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

    private HandSystem handSystem;



    // Hover
    [SerializeField] public float hoverScale = 1.3f;
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

    private bool inHand;
    public bool bInHand => inHand;
    public void EnterHand()
    {
        if (inHand) return;

        inHand = true;
        velocity = Vector2.zero;
    }

    public void ExitHand()
    {
        if (!inHand) return;

        inHand = false;
        velocity = Vector2.zero;
        KillHover();
        transform.localScale = originScale; // 수정 필요.
    }

    /// ////////////////


    void Awake()
    {
        rt = GetComponent<RectTransform>();


        // DoTween 움직임을 위한 변수들
        originScale = transform.localScale;
        targetPos = rt.anchoredPosition;
    }
    public void Initialize(HandSystem _handSystem)
    {
        handSystem = _handSystem;
        inHand = false;
    }

    void Update()
    {

        // 패에 있을 때, 연출
        InHand();

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

    public void KillHover()
    {
        hoverTween?.Kill();
        transform.localScale = originScale;
    }



    // 입력

    // 호버 ON
    public void OnPointerEnter(PointerEventData eventData)
    {
        handSystem?.OnCardHoverEnter(this);


        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale * hoverScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    // 호버 OFF
    public void OnPointerExit(PointerEventData eventData)
    {
        handSystem?.OnCardHoverExit(this);


        hoverTween?.Kill();
        hoverTween = transform.DOScale(originScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    // 클릭
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!handSystem.IscanAction) return;

        // 마우스 우클릭
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            handSystem.UseCard(this);
        }

        // 마우스 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 임시
            handSystem.UseCard(this);

            // 확대 및 옵션 연출 해야함.
        }
    }
}
