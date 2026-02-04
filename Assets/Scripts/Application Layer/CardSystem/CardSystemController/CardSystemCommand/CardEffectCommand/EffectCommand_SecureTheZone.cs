using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/SecureTheZone")]
public class EffectCommand_SecureTheZone : CardEffectCommand<ICardSlotSystemActionCommandHandler>
{
    [SerializeField] int bonusSlotCnt = 1;
    [SerializeField] int upgradedBonusSlotCnt = 1;

    protected override void Execute(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
            cardSlotSystemActionCommandHandler.ApplySlotCntModifier(bonusSlotCnt * nestingCnt * valueModifier);

        if (upgradeNestingCnt != 0)
            cardSlotSystemActionCommandHandler.ApplySlotCntModifier(upgradedBonusSlotCnt * nestingCnt * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {

    }
}
