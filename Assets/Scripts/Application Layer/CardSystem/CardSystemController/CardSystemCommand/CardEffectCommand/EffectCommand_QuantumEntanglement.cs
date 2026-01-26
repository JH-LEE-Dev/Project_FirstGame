using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/QuantumEntanglement")]
public class EffectCommand_QuantumEntanglement : CardEffectCommand<ICardSelectionSystemActionCommandHandler>
{
    [SerializeField] int duplicateAmount = 1;

    protected override void Execute(ICardSelectionSystemActionCommandHandler cardSelectionSystemActionCommandHandler)
    {
        Debug.Log("1");
        cardSelectionSystemActionCommandHandler.StartCardSelectionMode(CardSelectionMode.Duplicate, duplicateAmount);

        ResetCommandData();
    }
}