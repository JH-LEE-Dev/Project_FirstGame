using System;

public interface ICardDataControlActionCommandHandler : ICommandHandler
{
    void SetCardSystemContext(GameSystemActionContextType cardSystemContextType);
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant);
    void RevertCardsUpgrade(ReadOnlySpan<CardDataInstance> cards, bool bPermenant);
    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
    public void UndoValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
}
