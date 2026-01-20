using System.Collections.Generic;

namespace UnitSpawnSystemSignals
{
    public struct PlayerSpawnedSignal 
    {
        public IPlayerData playerData;

        public PlayerSpawnedSignal(IPlayerData _playerData)
        {
            playerData = _playerData;
        }
    }
    public struct EnemySpawnedSignal  { }
    public struct CharacterSpawnedSignal
    {
        public ICharacterData characterData;

        public CharacterSpawnedSignal(ICharacterData _characterData)
        {
            characterData = _characterData;
        }
    }
}
