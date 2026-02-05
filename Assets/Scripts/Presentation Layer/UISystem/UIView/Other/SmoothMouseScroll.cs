using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // ★ 이 네임스페이스가 꼭 필요합니다

public class SmoothMouseScroll : MonoBehaviour, IScrollHandler
{
    [Header("Main Settings")]
    [SerializeField] ScrollRect targetScrollRect;
    [SerializeField] float scrollSpeed = 0.3f;
    [SerializeField] float smoothTime = 0.15f;

    // 내부 변수
    private float _targetPosition = 1f;
    private float _currentVelocity = 0f;
    private bool _isScrolling = false;

    private void Awake()
    {
        if (targetScrollRect == null)
            targetScrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        if (targetScrollRect != null)
        {
            targetScrollRect.verticalNormalizedPosition = 1f;
            _targetPosition = 1f;
            _currentVelocity = 0f;
            _isScrolling = false;
        }
    }

    public void OnScroll(PointerEventData data)
    {
        if (targetScrollRect == null) 
            return;

        if (!_isScrolling) 
            _targetPosition = targetScrollRect.verticalNormalizedPosition;

        _targetPosition += data.scrollDelta.y * scrollSpeed;
        _targetPosition = Mathf.Clamp01(_targetPosition);

        _isScrolling = true;
    }

    private void Update()
    {
        if (targetScrollRect == null) 
            return;

        bool isPressed = false;
        if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            isPressed = true;
        }

        if (isPressed)
        {
            _targetPosition = targetScrollRect.verticalNormalizedPosition;
            _currentVelocity = 0f;
            _isScrolling = false;
            return;
        }

        if (!_isScrolling && Mathf.Abs(targetScrollRect.velocity.y) > 0.01f)
        {
            _targetPosition = targetScrollRect.verticalNormalizedPosition;
            return;
        }

        if (_isScrolling)
        {
            if (Mathf.Abs(targetScrollRect.verticalNormalizedPosition - _targetPosition) < 0.0001f)
            {
                _isScrolling = false;
                _currentVelocity = 0f;
                return;
            }

            float nextPos = Mathf.SmoothDamp(
                targetScrollRect.verticalNormalizedPosition,
                _targetPosition,
                ref _currentVelocity,
                smoothTime
            );

            targetScrollRect.verticalNormalizedPosition = nextPos;
        }
    }
}