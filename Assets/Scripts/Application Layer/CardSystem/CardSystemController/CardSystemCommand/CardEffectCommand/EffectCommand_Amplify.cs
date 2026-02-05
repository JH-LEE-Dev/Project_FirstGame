using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Amplify")]
public class EffectCommand_Amplify : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int bonusValueModifier = 1;
    [SerializeField] int upgradedBonusValueModifier = 1;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var bulletCards = complexSystemActionCommandHandler.GetCurrentBulletCards();

        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (bUpgraded == false)
        {
            int modifiedCnt = 0;

            for (int i = 0; i < bulletCards.Count; ++i)
            {
                for (int j = 0; j < bulletCards[i].Count; ++j)
                {
                    if (bulletCards[i][j].GetCardData().usingType == UsingType.Nesting)
                    {
                        writeBuffer[modifiedCnt] = bulletCards[i][j];
                        ++modifiedCnt;
                    }
                }
            }

            if (modifiedCnt != 0)
                complexSystemActionCommandHandler.ApplyValueModifier(writeBuffer.Slice(0, modifiedCnt), bonusValueModifier, cardSystemContextType);
        }
        else
        {
            int modifiedCnt = 0;

            for (int i = 0; i < bulletCards.Count; ++i)
            {
                for (int j = 0; j < bulletCards[i].Count; ++j)
                {
                    if (bulletCards[i][j].GetCardData().usingType == UsingType.Nesting)
                    {
                        writeBuffer[modifiedCnt] = bulletCards[i][j];
                        ++modifiedCnt;
                    }
                }
            }

            if (modifiedCnt != 0)
                complexSystemActionCommandHandler.ApplyValueModifier(writeBuffer.Slice(0, modifiedCnt), upgradedBonusValueModifier, cardSystemContextType);
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var bulletCards = complexSystemActionCommandHandler.GetCurrentBulletCards();

        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (bUpgraded == false)
        {
            int modifiedCnt = 0;

            for (int i = 0; i < bulletCards.Count; ++i)
            {
                for (int j = 0; j < bulletCards[i].Count; ++j)
                {
                    if (bulletCards[i][j].GetCardData().usingType == UsingType.Nesting)
                    {
                        writeBuffer[modifiedCnt] = bulletCards[i][j];
                        ++modifiedCnt;
                    }
                }
            }

            if (modifiedCnt != 0)
                complexSystemActionCommandHandler.UndoValueModifier(writeBuffer.Slice(0, modifiedCnt), bonusValueModifier, cardSystemContextType);
        }
        else
        {
            int modifiedCnt = 0;

            for (int i = 0; i < bulletCards.Count; ++i)
            {
                for (int j = 0; j < bulletCards[i].Count; ++j)
                {
                    if (bulletCards[i][j].GetCardData().usingType == UsingType.Nesting)
                    {
                        writeBuffer[modifiedCnt] = bulletCards[i][j];
                        ++modifiedCnt;
                    }
                }
            }

            if (modifiedCnt != 0)
                complexSystemActionCommandHandler.UndoValueModifier(writeBuffer.Slice(0, modifiedCnt), upgradedBonusValueModifier, cardSystemContextType);
        }
    }
}
