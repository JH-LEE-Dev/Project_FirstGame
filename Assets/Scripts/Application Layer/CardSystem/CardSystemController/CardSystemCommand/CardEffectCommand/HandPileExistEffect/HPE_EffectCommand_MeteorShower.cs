using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/HandPileExistCommand/Bullet/MeteorShower")]
public class HPE_EffectCommand_MeteorShower : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        var handPile = complexSystemActionCommand.GetHandPile();

        var bulletCardSlot = complexSystemActionCommand.GetCurrentCardSlot();

        bool bCondition_1 = false;
        for(int i = 0;i<bulletCardSlot.Count;++i)
        {
            if (bulletCardSlot[i].Count != 0)
            {
                bCondition_1 = true; break;
            }    
        }

        if (bCondition_1 == false)
            return;

        if (handPile.Count == 0)
            return;

        if (bUpgraded == false)
        {
            bool bCondition = true;

            for (int i = 0; i < 1; ++i)
            {
                if (handPile[i].GetCardData().id != (int)CardName.MeteorShower)
                {
                    bCondition = false;
                    break;
                }
            }

            if(bCondition && handPile.Count == 1)
            {
                complexSystemActionCommand.ApplyCardUsePhaseCntModifier(1, gameSystemActionContext);
            }
        }
        else
        {
            bool bCondition = true;

            for (int i = 0; i < handPile.Count; ++i)
            {
                if (handPile[i].GetCardData().id != (int)CardName.MeteorShower)
                {
                    bCondition = false;
                    break;
                }
            }

            if (bCondition)
            {
                complexSystemActionCommand.ApplyCardUsePhaseCntModifier(1, gameSystemActionContext);
            }
        }

        ResetCommandData();
    }
    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {

    }
}