using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Amplify")]
public class EffectCommand_Amplify : CardEffectCommand
{
    [SerializeField] int bonusValueModifier = 1;

    public override void Execute(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {
        cardSlotSystemActionCommandHandler.ApplyValueModifier(bonusValueModifier);

        ResetCommandData();
    }
}
