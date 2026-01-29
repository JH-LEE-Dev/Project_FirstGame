using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


public class CardPack : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image packImage;

    private bool canClick = false;
    private Action onClicked;

    public void Init()
    {
        if (!packImage) packImage = GetComponentInChildren<Image>(true);

        SetAlpha(0f);
        packImage.raycastTarget = false;
        canClick = false;
    }

    public void Bind(Action onClick)
    {
        onClicked = onClick;
    }

    public void Show(bool canClick)
    {
        this.canClick = canClick;
        gameObject.SetActive(true);

        packImage.raycastTarget = canClick;
        SetAlpha(1f);
    }

    public void Hide()
    {
        packImage.raycastTarget = false;
        SetAlpha(0f);
        gameObject.SetActive(false);
    }

    public void SetCanClick(bool value)
    {
        canClick = value;
        packImage.raycastTarget = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick) return;
        onClicked?.Invoke();
    }

    public void PlayOpenAnim()
    {



    }

    private void SetAlpha(float a)
    {
        if (!packImage) return;
        var c = packImage.color;
        c.a = a;
        packImage.color = c;
    }
}
