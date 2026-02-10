using System.Collections.Generic;

public interface ICardSlotSystemActionCommandHandler : ICommandHandler
{ 

    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCard();
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentCardSlot();

    void ApplySlotCntModifier(int cnt);
    int GetPrevUsedBulletCardCnt();
    bool IsInherenceCardEquipped();
    CardDataInstance GetCurrentInherenceCard();
}
