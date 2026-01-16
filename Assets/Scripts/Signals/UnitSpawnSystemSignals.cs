
namespace UnitSpawnSystemSignals
{
    public struct PlayerSpawnedEvent : IPulicSignal
    {
        public IPlayerData playerData;

        public PlayerSpawnedEvent(IPlayerData _playerData)
        {
            playerData = _playerData;
        }
    }
    public struct EnemySpawnedEvent : IPulicSignal { }
    public struct CharacterSpawnedEvent : IPulicSignal
    {
        public ICharacterData characterData;

        public CharacterSpawnedEvent(ICharacterData _characterData)
        {
            characterData = _characterData;
        }
    }
}
