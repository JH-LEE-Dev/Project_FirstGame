using System;

public interface ICardDataControlSystemActionCommandHandler : ICommandHandler
{
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards);
    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
}
