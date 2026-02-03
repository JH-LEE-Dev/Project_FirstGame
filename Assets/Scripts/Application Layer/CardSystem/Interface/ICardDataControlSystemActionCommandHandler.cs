using System;

public interface ICardDataControlActionCommandHandler : ICommandHandler
{
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant);
    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
}
