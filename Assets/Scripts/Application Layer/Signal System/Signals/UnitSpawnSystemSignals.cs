
namespace UnitSpawnSystemSignals
{
    public struct CharacterCreatedSignal
    {
        public ICharacterData characterData;
        public CharacterCreatedSignal(ICharacterData _characterData)
        {
            characterData = _characterData;
        }
    }
}
