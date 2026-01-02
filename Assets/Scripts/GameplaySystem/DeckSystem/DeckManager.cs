using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour, IDeckProvider
{
    private Dictionary<string, List<CardInstance>> cardPool = new Dictionary<string, List<CardInstance>>();
    private Stack<CardInstance> drawPile = new Stack<CardInstance>();
    private List<CardInstance> handPile = new List<CardInstance>();
    private List<CardInstance> gravePile = new List<CardInstance>();

    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int drawCardCnt = 5;
    [SerializeField] private float cardDrawRate = 1f;
    [SerializeField] private int initialCost = 3;

    private int curDrawedCardCnt = 0;

    public IReadOnlyList<CardData> HandCards => throw new NotImplementedException();

    public int deckCnt { get; private set; }

    public int graveCnt { get; private set; }

    public event Action HandChangedEvent;
    public event Action<CardInstance> CardDrawedEvent;
    public event Action CardDrawFinishedEvent;
    public event Action CardUsingFinishedEvent;

    public int curCost { get; private set; }


    public void Awake()
    {
        curCost = initialCost;

        for (int i = 0; i < cardDataBase.cardData.Count; ++i)
        {
            CardData cardData = cardDataBase.GetCardData(i);
            cardPool.Add(cardData.id, new List<CardInstance>());

            for (int j = 0; j < 5; ++j)
            {
                GameObject newInstance = Instantiate(cardData.cardObject);
                CardInstance newCardInstance = newInstance.GetComponent<CardInstance>();
                newCardInstance.Initialize(cardData);
                cardPool[cardData.id].Add(newCardInstance);
            }
        }
    }

    public void Start()
    {
        CardData cardData = cardDataBase.GetCardData(0);

        if (cardData == null)
        {
            return;
        }

        for (int i = 0; i < drawCardCnt; ++i)
        {
            drawPile.Push(cardPool[cardData.id][i]);
            ++deckCnt;
        }

        StartCoroutine(CardDrawCoroutine());
    }

    public void Draw(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawOne();
        }
    }

    private void DrawOne()
    {
        // if (drawPile.Count == 0)
        //Reshuffle();

        var card = drawPile.Pop();
        handPile.Add(card);
        card.gameObject.SetActive(true);

        --deckCnt;
        CardDrawedEvent?.Invoke(card);
    }

    private IEnumerator CardDrawCoroutine()
    {
        while (curDrawedCardCnt < 5)
        {
            yield return new WaitForSeconds(cardDrawRate);
            DrawOne();
            ++curDrawedCardCnt;
        }

        CardDrawFinishedEvent?.Invoke();
        curDrawedCardCnt = 0;
    }

    public bool CardUsed(CardInstance usedCard)
    {
        int cardCost = usedCard.GetCardData().cost;

        if (curCost - cardCost < 0)
        {
            Debug.Log("Not Enough Cost");
            return false;
        }
        else
        {
            handPile.Remove(usedCard);
            ++graveCnt;
            gravePile.Add(usedCard);

            curCost -= cardCost;
        }

        return true;
    }

    public void CardUsingFinished()
    {
        CardUsingFinishedEvent?.Invoke();
    }
}
