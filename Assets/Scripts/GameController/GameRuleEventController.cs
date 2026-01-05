public class GameRuleEventController
{
    public void Bind(Character character, GameController gameController,IDeckProvider deckProvider)
    {
        GS_EnemyTurnState enemyTurnState = gameController.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= character.ResetbCanAction;
            enemyTurnState.EnemyTurnStartEvent += character.ResetbCanAction;
        }

        deckProvider.CardUsingFinishedEvent -= character.SetbCanAction;
        deckProvider.CardUsingFinishedEvent += character.SetbCanAction;

        character.PlayerAttackIsFinishedEvent += OnPlayerAttackFinished;
        this.gameController = gameController;
    }

    private GameController gameController;

    private void OnPlayerAttackFinished()
    {
        gameController.PlayerTurnIsFinished();
    }
}