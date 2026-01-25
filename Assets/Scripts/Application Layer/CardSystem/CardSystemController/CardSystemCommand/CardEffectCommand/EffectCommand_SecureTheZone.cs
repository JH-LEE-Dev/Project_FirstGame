using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/SecureTheZone")]
public class EffectCommand_SecureTheZone : CardEffectCommand<ICardSlotSystemActionCommandHandler>
{
    [SerializeField] int bonusSlotCnt = 1;

    protected override void Execute(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {
        cardSlotSystemActionCommandHandler.ApplySlotCntModifier(bonusSlotCnt);

        ResetCommandData();
    }
}
