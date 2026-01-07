using UnityEngine;

public interface ICardLogicSystem
{
    void StrategyForwarding(CardEffectStrategy effectStrategy);

    void DrawAgain(int drawAmount);
}
