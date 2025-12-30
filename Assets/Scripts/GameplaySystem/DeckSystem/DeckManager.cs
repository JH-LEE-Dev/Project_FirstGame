using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private Dictionary<string, Queue<CardInstance>> cardPool;
    private Stack<CardInstance> drawPile;
    private List<CardInstance> hand;
    private List<CardInstance> discardPile;
    private CardDataBase cardDataBase;

    public void Awake()
    {
        for (int i = 0; i < cardDataBase.cardData.Count; ++i)
        {
            CardData cardData = cardDataBase.GetCardData(i);
            cardPool.Add(cardData.id, new Queue<CardInstance>());

            for (int j = 0; j < 5; ++j)
            {
                CardInstance instance = new CardInstance();
                instance.Initialize(cardData);

                cardPool[cardData.id].Enqueue(instance);
            }
        }
    }

    public void Draw(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawOne();
        }
    }

    void DrawOne()
    {
        // if (drawPile.Count == 0)
        //Reshuffle();

        var card = drawPile.Pop();
        hand.Add(card);

        //EventBus.Publish(new CardDrawnEvent(card));
    }
}
