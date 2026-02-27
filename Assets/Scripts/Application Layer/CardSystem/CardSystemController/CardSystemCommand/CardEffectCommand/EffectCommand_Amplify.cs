using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Amplify")]
public class EffectCommand_Amplify : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int bonusValueModifier = 1;
    [SerializeField] int upgradedBonusValueModifier = 1;

    public override bool EffectConditionCheck()
    {
        int newCondition = 0;

        if (newCondition != condition)
        {
            CheckApplyCondition();
            condition = newCondition;
        }
        return true;
    }

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        EffectConditionCheck();

        var bulletCards = handler.cardSlotSystem.GetCurrentCardSlot();

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
            {
                handler.cardDataSystem.SetCardSystemContext(gameSystemActionContext);
                handler.cardDataSystem.ApplyValueModifier(writeBuffer.Slice(0, modifiedCnt), bonusValueModifier);
            }
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
            {
                handler.cardDataSystem.SetCardSystemContext(gameSystemActionContext);
                handler.cardDataSystem.ApplyValueModifier(writeBuffer.Slice(0, modifiedCnt), upgradedBonusValueModifier);
            }
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        var bulletCards = handler.cardSlotSystem.GetCurrentCardSlot();

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
            {
                handler.cardDataSystem.SetCardSystemContext(gameSystemActionContext);
                handler.cardDataSystem.UndoValueModifier(writeBuffer.Slice(0, modifiedCnt), bonusValueModifier);
            }
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
            {
                handler.cardDataSystem.SetCardSystemContext(gameSystemActionContext);
                handler.cardDataSystem.UndoValueModifier(writeBuffer.Slice(0, modifiedCnt), upgradedBonusValueModifier);
            }
        }
    }
}
