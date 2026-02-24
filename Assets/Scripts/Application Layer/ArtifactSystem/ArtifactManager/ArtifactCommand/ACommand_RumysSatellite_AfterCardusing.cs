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

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        if (handler.cardSlotSystem.IsInherenceCardEquipped() == true)
            return;

        handler.cardSlotSystem.SetInherenceCard(prismBolt);

        DebuffElementData debuffElementData = new DebuffElementData(DebuffElementEffectType.Default,0);
        AdditionalAttackStat additionalAttackStat = new AdditionalAttackStat(2, 0.2f, 1, debuffElementData);

        if (bUpgraded == false)
        {
            handler.statusSystem.SetBulletType(BulletType.PrismBolt, false);
            handler.statusSystem.ApplyAdditionalAttackStat(additionalAttackStat);

            handler.statusSystem.ApplyAttackModifier(10);
            handler.statusSystem.ApplyAdditionalAttackValueModifier(1);
        }
        else
        {
            handler.statusSystem.SetBulletType(BulletType.PrismBolt, false);
            handler.statusSystem.ApplyAdditionalAttackStat(additionalAttackStat);

            handler.statusSystem.ApplyAttackModifier(20);
            handler.statusSystem.ApplyAdditionalAttackValueModifier(2);
        }

        handler.statusSystem.SetCharacterCanAttackState(true);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _handler)
    {
        if (_handler.cardSlotSystem.IsInherenceCardEquipped() == true)
            return;

        DebuffElementData debuffElementData = new DebuffElementData(DebuffElementEffectType.Default, 0);
        AdditionalAttackStat additionalAttackStat = new AdditionalAttackStat(0, 0f, 0, debuffElementData);

        _handler.statusSystem.ApplyAdditionalAttackStat(additionalAttackStat);
        _handler.statusSystem.ResetBulletType();

        _handler.statusSystem.SetCharacterCanAttackState(false);
    }
}