using UnityEngine;

public interface ICardSystemActionCommand
{
    public void Execute(ICommandHandler handler);
    public CardSystemContextType GetCardSystemContext();
}
