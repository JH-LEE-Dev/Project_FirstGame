using UnityEngine.EventSystems;
using UnityEngine;

public class ShopCardInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ShopCardInstance owner;
    public void Bind(ShopCardInstance card) => owner = card;

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
