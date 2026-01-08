using UnityEngine;

public interface ICardStrategyHandler
{
    void StrategyForwarding(CardEffectStrategy effectStrategy);

    void DrawAgain(int drawAmount);

    void AttackAgain();
}
