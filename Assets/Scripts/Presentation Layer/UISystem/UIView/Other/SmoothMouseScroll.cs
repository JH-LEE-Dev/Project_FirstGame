using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // ★ 이 네임스페이스가 꼭 필요합니다

public class SmoothMouseScroll : MonoBehaviour, IScrollHandler
{
    [Header("Main Settings")]
    [SerializeField] private ScrollRect targetScrollRect;
    [SerializeField] private float scrollSpeed = 0.03f;
    [SerializeField] private float smoothTime = 0.2f;

    // 내부 변수
    private float targetPosition = 1f;
    private float currentVelocity = 0f;
    private bool isScrolling = false;

    private void Awake()
    {
        if (null == targetScrollRect)
            targetScrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        if (null != targetScrollRect)
        {
            targetScrollRect.verticalNormalizedPosition = 1f;
            targetPosition = 1f;
            currentVelocity = 0f;
            isScrolling = false;
        }
    }

    public void OnScroll(PointerEventData data)
    {
        if (null == targetScrollRect) 
            return;

        if (!isScrolling) 
            targetPosition = targetScrollRect.verticalNormalizedPosition;

        targetPosition += data.scrollDelta.y * scrollSpeed;
        targetPosition = Mathf.Clamp01(targetPosition);

        isScrolling = true;
    }

    private void Update()
    {
        if (null == targetScrollRect) 
            return;

        bool isPressed = false;
        if (null != Pointer.current && Pointer.current.press.isPressed)
            isPressed = true;

        if (isPressed)
        {
            targetPosition = targetScrollRect.verticalNormalizedPosition;
            currentVelocity = 0f;
            isScrolling = false;
            return;
        }

        if (!isScrolling && Mathf.Abs(targetScrollRect.velocity.y) > 0.01f)
        {
            targetPosition = targetScrollRect.verticalNormalizedPosition;
            return;
        }

        if (isScrolling)
        {
            if (Mathf.Abs(targetScrollRect.verticalNormalizedPosition - targetPosition) < 0.0001f)
            {
                isScrolling = false;
                currentVelocity = 0f;
                return;
            }

            float nextPos = Mathf.SmoothDamp(
                targetScrollRect.verticalNormalizedPosition,
                targetPosition,
                ref currentVelocity,
                smoothTime
            );

            targetScrollRect.verticalNormalizedPosition = nextPos;
        }
    }
}