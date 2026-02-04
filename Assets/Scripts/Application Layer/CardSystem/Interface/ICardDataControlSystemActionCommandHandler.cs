using System;

public interface ICardDataControlActionCommandHandler : ICommandHandler
{
    void SetCardSystemContext(CardSystemContextType cardSystemContextType);
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant);
    void RevertCardsUpgrade(ReadOnlySpan<CardDataInstance> cards, bool bPermenant);
    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
}
