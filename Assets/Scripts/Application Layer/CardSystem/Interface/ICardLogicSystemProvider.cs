using System.Collections.Generic;

public interface ICardLogicSystemProvider
{
    CardDataInstance CreateCard(int id);
    void ReleaseCard(CardDataInstance card);
    void AddCards_Temp(List<ICardDataInstanceProvider> _cards);
    void DeleteCards_Temp(List<ICardDataInstanceProvider> _cards);
    void UpgradeCards_Temp(List<ICardDataInstanceProvider> _cards);
}
