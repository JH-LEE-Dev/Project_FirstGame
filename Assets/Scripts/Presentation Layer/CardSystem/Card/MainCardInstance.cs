using NaughtyAttributes;
using UnityEngine;

public class MainCardInstance : CardInstance
{
    // 이 카드의 풀링 전용이 HandSystem인가 아니면 Other인가에 대한 열거
    public CardInstanceType cardInstanceType { get; private set; }

    // HandSystem전용. 상태를 나타냄.
    public CardState cardState { get; private set; } = CardState.Hidden;

    // 시스템
    private UIView_CardSystem cardSystem;
    public UIView_CardSystem CardSystem => cardSystem;




    // 컴포넌트
    public CardMotion Motion { get; private set; }
    public CardOtherMotion OtherMotion { get; private set; }
    public CardVisualFloat VisualFloat { get; private set; }
    public CardInput Input { get; private set; }

    private void Awake()
    {
        Motion = GetComponent<CardMotion>();
        OtherMotion = GetComponent<CardOtherMotion>();
        Input = GetComponent<CardInput>();
        VisualFloat = GetComponentInChildren<CardVisualFloat>(true);

        if (Motion) Motion.Bind(this);
        if (OtherMotion) OtherMotion.Bind(this);
        if (Input) Input.Bind(this);
        if (VisualFloat) VisualFloat.Bind(this);
    }

    public void Initialize(UIView_CardSystem system, CardInstanceType type)
    {
        cardSystem = system;
        cardInstanceType = type;
    }

    public void SetUIState(CardState state)
    {
        cardState = state;
    }

    public void SetVisible(bool visible)
    {
        VisualFloat?.SetVisible(visible);
    }
}
