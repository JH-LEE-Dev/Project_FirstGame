using UnityEngine;

public interface ICardSystemActionCommand
{
    public void Execute(ICardSystemActionCommandHandler cardEffectCommandHandler) { }
}
