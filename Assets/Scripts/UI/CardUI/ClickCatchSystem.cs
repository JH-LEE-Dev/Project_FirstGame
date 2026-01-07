using UnityEngine;
using UnityEngine.EventSystems;
public class ClickCatchSystem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UIView_CardSystem cardSystem;

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left ||
            eventData.button == PointerEventData.InputButton.Right)
            cardSystem?.CancelPreview();
    }
}
