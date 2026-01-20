using UnityEngine;
using UnityEngine.EventSystems;
public class ClickCatchSystem : MonoBehaviour, IPointerClickHandler
{
    private UIView_Unit uIView_Unit;
    private WorldCanvasEnabler canvasEnabler;

    public void Init(UIView_Unit _uIView_Unit)
    {
        uIView_Unit = _uIView_Unit;
        canvasEnabler = GetComponent<WorldCanvasEnabler>();
        canvasEnabler.Initialize();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left ||
            eventData.button == PointerEventData.InputButton.Right)
            uIView_Unit?.CancelPreview();
    }
}
