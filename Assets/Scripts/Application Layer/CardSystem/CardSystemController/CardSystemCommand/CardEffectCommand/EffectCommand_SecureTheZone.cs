using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/SecureTheZone")]
public class EffectCommand_SecureTheZone : CardEffectCommand<ICardSlotSystemActionCommandHandler>
{
    [SerializeField] int bonusSlotCnt = 1;
    [SerializeField] int upgradedBonusSlotCnt = 1;

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

    protected override void Execute(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {
        if (bUpgraded == false)
            cardSlotSystemActionCommandHandler.ApplySlotCntModifier(bonusSlotCnt  * (int)valueModifier);
        else
            cardSlotSystemActionCommandHandler.ApplySlotCntModifier(upgradedBonusSlotCnt  * (int)valueModifier);
    }
    protected override void Undo(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {

    }
}
