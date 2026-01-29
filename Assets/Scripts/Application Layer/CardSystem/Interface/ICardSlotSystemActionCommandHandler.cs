using System.Collections.Generic;

public interface ICardSlotSystemActionCommandHandler : ICommandHandler
{ 

    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCard();
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentBulletCards();

    void ApplySlotCntModifier(int cnt);
    int GetPrevUsedBulletCardCnt();
}
