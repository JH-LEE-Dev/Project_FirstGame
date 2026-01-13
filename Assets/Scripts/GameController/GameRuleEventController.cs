public class GameRuleEventController
{
    //Character의존 DIP 적용 해야 함.
    public void Bind(Character character, IGameFlowProvider gameFlowProvider,ICardSystemEvents cardSystemEvent,
        ICardSystemActions cardSystemActions)
    {
        GS_EnemyTurnState enemyTurnState = gameFlowProvider.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
            enemyTurnState.EnemyTurnStartEvent += character.ResetbCanAction;
        }

        cardSystemEvent.CardUsingTurnFinishedEvent -= character.SetbCanAction;
        cardSystemEvent.CardUsingTurnFinishedEvent += character.SetbCanAction;

        character.PlayerAttackFinishedEvent -= cardSystemActions.PlayerTurnFinished;
        character.PlayerAttackFinishedEvent += cardSystemActions.PlayerTurnFinished;
    }

    public void Release(Character character, IGameFlowProvider gameFlowProvider, ICardSystemEvents cardEventSetter
        ,ICardSystemActions cardSystemActions)
    {
        GS_EnemyTurnState enemyTurnState = gameFlowProvider.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
        }

        cardEventSetter.CardUsingTurnFinishedEvent -= character.SetbCanAction;

        character.PlayerAttackFinishedEvent -= cardSystemActions.PlayerTurnFinished;
    }
}