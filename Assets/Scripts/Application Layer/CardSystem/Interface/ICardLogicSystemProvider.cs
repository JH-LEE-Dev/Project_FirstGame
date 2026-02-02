using System.Collections.Generic;

public interface ICardLogicSystemProvider
{
    CardDataInstance CreateCard(int id);
    void ReleaseCard(CardDataInstance card);
}
