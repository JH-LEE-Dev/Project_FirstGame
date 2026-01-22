using System.Collections.Generic;

public interface ICardSlotSystemActionCommandHandler : ICommandHandler
{
    void ApplyValueModifier(int valueModifier);

    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedRotationBulletCard();
}
