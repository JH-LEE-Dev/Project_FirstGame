using UnityEngine;

[CreateAssetMenu(menuName = "Command/ArtifactEffects/Rumy's Satellite AfterCardUsing")]
public class ACommand_RumysSatellite_AfterCardUsing : ArtifactCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private CardDataBase cardDataBase;

    private CardDataInstance prismBolt = null;

    public override void InitializeCommand(bool _bUpgraded, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_bUpgraded, _cardSystemContextType);

        if (prismBolt == null)
        {
            CardData cardData = cardDataBase.GetCardData((int)CardName.PrismBolt);
            prismBolt = new CardDataInstance();
            prismBolt.Initialize(cardData);
        }
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (complexSystemActionCommand.IsInherenceCardEquipped() == true)
            return;

        complexSystemActionCommand.SetInherenceCard(prismBolt);

        AdditionalAttackStat additionalAttackStat = new AdditionalAttackStat(2, 0.2f, 1, default);

        if (bUpgraded == false)
        {
            complexSystemActionCommand.SetBulletType(BulletType.PrismBolt, false);
            complexSystemActionCommand.ApplyAdditionalAttackStat(additionalAttackStat);

            complexSystemActionCommand.ApplyAttackModifier(10, GameSystemActionContextType.MAX);
            complexSystemActionCommand.ApplyAdditionalAttackValueModifier(1);
        }
        else
        {
            complexSystemActionCommand.SetBulletType(BulletType.PrismBolt, false);
            complexSystemActionCommand.ApplyAdditionalAttackStat(additionalAttackStat);

            complexSystemActionCommand.ApplyAttackModifier(20, GameSystemActionContextType.MAX);
            complexSystemActionCommand.ApplyAdditionalAttackValueModifier(2);
        }

        complexSystemActionCommand.SetCharacterCanAttackState(true);
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (complexSystemActionCommand.IsInherenceCardEquipped() == true)
            return;

        complexSystemActionCommand.ApplyAdditionalAttackStat(default);
        complexSystemActionCommand.ResetBulletType();

        complexSystemActionCommand.SetCharacterCanAttackState(false);
    }
}