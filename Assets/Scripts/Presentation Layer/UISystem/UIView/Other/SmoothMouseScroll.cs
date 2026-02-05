using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SmoothMouseScroll : MonoBehaviour, IScrollHandler
{
    [Header("Main Settings")]
    [SerializeField] private ScrollRect targetScrollRect;
    [SerializeField] private float scrollSpeed = 0.2f;
    [SerializeField] private float smoothTime = 0.15f;

    private float targetPosition = 1f;
    private float currentVelocity = 0f;
    private bool isScrolling = false;

    private RectTransform _contentRect;
    private RectTransform _viewportRect;

    private void Awake()
    {
        if (null == targetScrollRect)
            targetScrollRect = GetComponent<ScrollRect>();

        if (targetScrollRect != null)
        {
            _contentRect = targetScrollRect.content;
            _viewportRect = targetScrollRect.viewport;

            if (_viewportRect == null)
                _viewportRect = targetScrollRect.GetComponent<RectTransform>();
        }
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
        if (null == targetScrollRect || _contentRect == null || _viewportRect == null)
            return;

        if (!isScrolling)
            targetPosition = targetScrollRect.verticalNormalizedPosition;

        float contentHeight = _contentRect.rect.height;
        float viewportHeight = _viewportRect.rect.height;
        float scrollableHeight = contentHeight - viewportHeight;

        if (scrollableHeight <= 0) 
            return;

        float step = (data.scrollDelta.y * scrollSpeed * viewportHeight) / scrollableHeight;

        targetPosition += step;
        targetPosition = Mathf.Clamp01(targetPosition);

        isScrolling = true;
    }

    private void Update()
    {
        if (null == targetScrollRect)
            return;

        // 클릭/드래그 체크 (New Input System)
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

        // 관성 체크
        if (!isScrolling && Mathf.Abs(targetScrollRect.velocity.y) > 0.01f)
        {
            targetPosition = targetScrollRect.verticalNormalizedPosition;
            return;
        }

        // 부드러운 이동
        if (isScrolling)
        {
            if (Mathf.Abs(targetScrollRect.verticalNormalizedPosition - targetPosition) < 0.0001f)
            {
                isScrolling = false;
                currentVelocity = 0f;
                targetScrollRect.verticalNormalizedPosition = targetPosition;
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