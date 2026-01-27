using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/QuantumEntanglement")]
public class EffectCommand_QuantumEntanglement : CardEffectCommand<ICardSelectionSystemActionCommandHandler>
{
    [SerializeField] int duplicateAmount = 1;
    [SerializeField] int upgradedDuplicateAmount = 1;

    protected override void Execute(ICardSelectionSystemActionCommandHandler cardSelectionSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
            cardSelectionSystemActionCommandHandler.StartCardSelectionMode(CardSelectionMode.DuplicateToHand, duplicateAmount * nestingCnt * valueModifier);

        if(upgradeNestingCnt != 0)
            cardSelectionSystemActionCommandHandler.StartCardSelectionMode(CardSelectionMode.DuplicateToHand, upgradedDuplicateAmount * upgradeNestingCnt * valueModifier);

        ResetCommandData();
    }
}