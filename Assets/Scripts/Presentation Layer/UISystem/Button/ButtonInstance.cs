using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class ButtonInstance : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum VisualState
    {
        Hidden,             // 아예 안보이는 모드
        VisibleDisabled,    // 보이지만 비활성모드 (클릭안됨)
        VisibleEnabled      // 보이고 활성모드
    }

    [SerializeField] protected Image button;

    [Header("Alpha")]
    [SerializeField, Range(0f, 1f)] protected float hiddenAlpha = 0f;
    [SerializeField, Range(0f, 1f)] protected float disabledAlpha = 0.3f;
    [SerializeField, Range(0f, 1f)] protected float enabledAlpha = 1f;

    protected bool bIsActive;
    protected bool bCanClick;
    protected VisualState state = VisualState.Hidden;



    protected virtual void Awake()
    {
        if (!button) button = GetComponentInChildren<Image>(true);
    }

    public virtual void Init()
    {
        bIsActive = false;
        bCanClick = false;
        ApplyState(VisualState.Hidden);
    }


    // 이 버튼 자체를 보이게 할 것인지, 안보이게 할 것인지.
    public void SetActiveVisible(bool active)
    {
        bIsActive = active;
        Recompute();
    }

    // 버튼을 클릭 가능하게 할 것인지, 클릭 불가능하게 할 것인지.
    public void SetCanClick(bool canClick)
    {
        bCanClick = canClick;
        Recompute();
    }

    // 위 두개 말고, VisualState으로 버튼의 상태를 컨트롤한다.
    public void SetState(VisualState newState)
    {
        state = newState;
        bIsActive = (state != VisualState.Hidden);
        bCanClick = (state == VisualState.VisibleEnabled);
        ApplyState(state);
    }




    // 신경 ㄴㄴ 알아서 비활 활성에 따른 알파변화 및 raycast on off 중
    private void Recompute()
    {
        if (!bIsActive) ApplyState(VisualState.Hidden);
        else if (!bCanClick) ApplyState(VisualState.VisibleDisabled);
        else ApplyState(VisualState.VisibleEnabled);
    }

    private void ApplyState(VisualState newState)
    {
        var old = state;
        state = newState;

        if (button) button.raycastTarget = (state == VisualState.VisibleEnabled);

        float targetA = state switch
        {
            VisualState.Hidden => hiddenAlpha,
            VisualState.VisibleDisabled => disabledAlpha,
            VisualState.VisibleEnabled => enabledAlpha,
            _ => enabledAlpha
        };

        SetAlpha(targetA);

        if (old == newState) return;
        OnStateChanged(old, state);
    }

    private void SetAlpha(float alpha)
    {
        if (!button) return;

        var c = button.color; 
        c.a = alpha; 
        button.color = c;
    }


    // For EventSystem
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state != VisualState.VisibleEnabled) return;
        OnHoverEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (state != VisualState.VisibleEnabled) return;
        OnHoverExit(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (state != VisualState.VisibleEnabled) return;
        if (!bCanClick) return;
        OnClick(eventData);
    }



    ////// 자식 구현부 //////
    
    // State가 변할 때, 모션을 알아서 구현하소
    protected virtual void OnStateChanged(VisualState oldState, VisualState newState) { }

    // 호버 ON 구현
    protected virtual void OnHoverEnter(PointerEventData eventData) { }

    // 호버 OFF 구현
    protected virtual void OnHoverExit(PointerEventData eventData) { }

    // 클릭할 때 구현
    protected abstract void OnClick(PointerEventData eventData);
}
