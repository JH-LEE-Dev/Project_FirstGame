
namespace UnitSpawnSystemSignals
{
    public struct PlayerSpawnedEvent
    {
        public IPlayerData playerData;

        public PlayerSpawnedEvent(IPlayerData _playerData)
        {
            playerData = _playerData;
        }
    }
    public struct EnemySpawnedEvent { }
    public struct CharacterSpawnedEvent
    {
        public ICharacterData characterData;

        public CharacterSpawnedEvent(ICharacterData _characterData)
        {
            characterData = _characterData;
        }
    }
}
