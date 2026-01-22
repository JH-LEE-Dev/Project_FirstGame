using UnityEngine;

public interface ICardStatusEffectCommand
{
    public void Execute(ICommandHandler handler);
}
