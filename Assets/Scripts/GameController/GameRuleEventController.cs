public class GameRuleEventController
{
    private GameController gameController;

    public void Bind(Character character, GameController gameController,ICardEventSetter cardEventSetter)
    {
        GS_EnemyTurnState enemyTurnState = gameController.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
            enemyTurnState.EnemyTurnStartEvent += character.ResetbCanAction;
        }

        cardEventSetter.CardUsingFinishedEvent -= character.SetbCanAction;
        cardEventSetter.CardUsingFinishedEvent += character.SetbCanAction;

        character.PlayerAttackIsFinishedEvent -= OnPlayerAttackFinished;
        character.PlayerAttackIsFinishedEvent += OnPlayerAttackFinished;
        this.gameController = gameController;
    }

    public void Release(Character character, GameController gameController, ICardEventSetter cardEventSetter)
    {
        GS_EnemyTurnState enemyTurnState = gameController.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
        }

        cardEventSetter.CardUsingFinishedEvent -= character.SetbCanAction;

        character.PlayerAttackIsFinishedEvent -= OnPlayerAttackFinished;
    }

    private void OnPlayerAttackFinished()
    {
        gameController.PlayerTurnIsFinished();
    }
}