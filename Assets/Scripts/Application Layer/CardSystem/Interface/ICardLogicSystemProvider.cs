using System.Collections.Generic;

public interface ICardLogicSystemProvider
{
    CardDataInstance CreateCard(int id);
    void ReleaseCard(CardDataInstance card);
    void AddCards_Temp(List<CardDataInstance> _cards);
    void DeleteCards_Temp(List<CardDataInstance> _cards);
    void UpgradeCards_Temp(List<CardDataInstance> _cards);
}
