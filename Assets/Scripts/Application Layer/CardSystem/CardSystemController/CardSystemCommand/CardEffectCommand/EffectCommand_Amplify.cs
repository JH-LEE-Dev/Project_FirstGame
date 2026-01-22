using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Amplify")]
public class EffectCommand_Amplify : CardEffectCommand<ICardSlotSystemActionCommandHandler>
{
    [SerializeField] int bonusValueModifier = 1;

    protected override void Execute(ICardSlotSystemActionCommandHandler cardSlotSystemActionCommandHandler)
    {
        cardSlotSystemActionCommandHandler.ApplyValueModifier(bonusValueModifier);

        ResetCommandData();
    }
}
