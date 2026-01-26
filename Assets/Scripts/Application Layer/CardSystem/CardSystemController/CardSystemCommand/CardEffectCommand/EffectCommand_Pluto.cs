using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Pluto")]
public class EffectCommand_Pluto : CardEffectCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.ExtinctionCardsToDeck;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.RandomExtinctionCardToDeck();

        ResetCommandData();
    }
}
