using System;

public interface ICardDataControlSystemActionCommandHandler : ICommandHandler
{
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant);
    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
}
