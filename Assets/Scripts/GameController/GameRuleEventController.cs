public class GameRuleEventController
{
    //Character의존 DIP 적용 해야 함.
    public void Bind_Character(Character character, IGameFlowProvider gameFlowProvider,ICardSystemEvents cardSystemEvent,
        ICardSystemFlowActions cardSystemFlowActions)
    {
        GS_EnemyTurnState enemyTurnState = gameFlowProvider.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
            enemyTurnState.EnemyTurnStartEvent += character.ResetbCanAction;
        }

        cardSystemEvent.CardUsingTurnFinishedEvent -= character.SetbCanAction;
        cardSystemEvent.CardUsingTurnFinishedEvent += character.SetbCanAction;
        cardSystemEvent.CardDrawedEvent -= character.PlayerTurnStarted;
        cardSystemEvent.CardDrawedEvent += character.PlayerTurnStarted;

        character.PlayerAttackFinishedEvent -= cardSystemFlowActions.PlayerTurnFinished;
        character.PlayerAttackFinishedEvent += cardSystemFlowActions.PlayerTurnFinished;
    }

    public void Release_Character(Character character, IGameFlowProvider gameFlowProvider, ICardSystemEvents cardSystemEvent
        , ICardSystemFlowActions cardSystemFlowActions)
    {
        GS_EnemyTurnState enemyTurnState = gameFlowProvider.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
        }

        cardSystemEvent.CardDrawedEvent -= character.PlayerTurnStarted;
        cardSystemEvent.CardUsingTurnFinishedEvent -= character.SetbCanAction;

        character.PlayerAttackFinishedEvent -= cardSystemFlowActions.PlayerTurnFinished;
    }

    public void Bind_Enemy(Enemy enemy, IWaveSystemEvents waveSystemEvents, IWaveSystemActions waveSystemActions)
    {
        enemy.UnitIsDeadEvent -= waveSystemActions.EnemyIsDead;
        enemy.UnitIsDeadEvent += waveSystemActions.EnemyIsDead;
        waveSystemEvents.StartMoveEvent -= enemy.OnMove;
        waveSystemEvents.StartMoveEvent += enemy.OnMove;
    }

    public void Release_Enemy(Enemy enemy, IWaveSystemEvents waveSystemEvents, IWaveSystemActions waveSystemActions)
    {
        enemy.UnitIsDeadEvent -= waveSystemActions.EnemyIsDead;
        waveSystemEvents.StartMoveEvent -= enemy.OnMove;
    }
}