using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class PickUpCancelPannal : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image dim;

    private bool canCancel = true;
    private Action onClicked;

    public void Init()
    {
        if (!dim) dim = GetComponentInChildren<Image>(true);

        SetAlpha(0f);
        dim.raycastTarget = false;
        canCancel = true;
    }
    public void Bind(Action onClick)
    {
        onClicked = onClick;
    }

    public void Show(bool canCancel)
    {
        this.canCancel = canCancel;
        gameObject.SetActive(true);

        dim.raycastTarget = true;
        SetAlpha(0.7f);
    }

    public void Hide()
    {
        dim.raycastTarget = false;
        SetAlpha(0f);
        gameObject.SetActive(false);
    }

    public void SetCanCancel(bool value)
    {
        canCancel = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!dim.raycastTarget) return;
        if (!canCancel) return;

        onClicked?.Invoke();
    }

    private void SetAlpha(float a)
    {
        if (!dim) return;
        var c = dim.color;
        c.a = a;
        dim.color = c;
    }
}
