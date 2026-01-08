public class GameRuleEventController
{
    public void Bind(Character character, IGameFlowProvider gameFlowProvider,ICardSystemEvent cardSystemEvent,
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

        character.PlayerAttackIsFinishedEvent -= cardSystemActions.PlayerTurnFinished;
        character.PlayerAttackIsFinishedEvent += cardSystemActions.PlayerTurnFinished;
    }

    public void Release(Character character, GameController gameController, ICardSystemEvent cardEventSetter
        ,ICardSystemActions cardSystemActions)
    {
        GS_EnemyTurnState enemyTurnState = gameController.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
        }

        cardEventSetter.CardUsingTurnFinishedEvent -= character.SetbCanAction;

        character.PlayerAttackIsFinishedEvent -= cardSystemActions.PlayerTurnFinished;
    }
}