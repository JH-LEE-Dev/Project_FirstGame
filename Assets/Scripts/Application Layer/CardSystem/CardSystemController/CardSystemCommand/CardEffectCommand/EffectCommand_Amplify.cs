using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Amplify")]
public class EffectCommand_Amplify : CardEffectCommand<ICardSlotSystemActionCommandHandler>
{
    [SerializeField] int bonusValueModifier = 1;
    [SerializeField] int upgradedBonusValueModifier = 1;

    protected override void Execute(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
            cardSlotSystemActionCommandHandler.ApplyValueModifier(bonusValueModifier*valueModifier);

        if (upgradeNestingCnt != 0)
            cardSlotSystemActionCommandHandler.ApplyValueModifier(upgradedBonusValueModifier * valueModifier);

        ResetCommandData();
    }
}
