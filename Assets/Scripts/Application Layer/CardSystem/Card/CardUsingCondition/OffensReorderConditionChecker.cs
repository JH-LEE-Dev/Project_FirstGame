using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardUsingConditionCheck/Offense Reorder")]
public class OffensReorderConditionChecker : CardUsingCondition
{
    public override void CheckUsingCondition(IComplexSystemActionCommandHandler _handler)
    {
        bResult = false;

        var handPile = _handler.cardLogicSystem.GetHandPile();

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].GetCardData().cardType == CardType.Inherence)
            {
                bResult = true;
                break;
            }
        }
    }
}
