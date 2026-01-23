using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Pluto")]
public class EffectCommand_Pluto : CardEffectCommand<ICardSystemActionCommandHandler>
{
    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.RandomExtinctionCardToDeck();

        ResetCommandData();
    }
}
