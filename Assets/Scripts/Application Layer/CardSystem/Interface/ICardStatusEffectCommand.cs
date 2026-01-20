using UnityEngine;

public interface ICardStatusEffectCommand
{
    public void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler) { }
}
