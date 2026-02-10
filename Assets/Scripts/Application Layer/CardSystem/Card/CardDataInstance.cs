using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[Serializable]
public class CardDataInstance : ICardDataInstanceProvider
{
    //카드 인스턴스마다 불변인 데이터는 cardData로 캡슐화.
    private CardData cardData;

    //카드 인스턴스마다 가변인 데이터는 CardDataInstance에 노출.
    public bool bUpgrade = false;
    public bool bPermanentUpgrade = false;
    public int valueModifier = 1;
    public bool bPermanent = false;

    public Dictionary<BulletElementType, BulletElementData> elementTypes = new Dictionary<BulletElementType, BulletElementData>(SYSTEM_VAR.maxDebuffElementCount);
    public Dictionary<DebuffElementEffectType, DebuffElementData> debuffTypes = new Dictionary<DebuffElementEffectType, DebuffElementData>(SYSTEM_VAR.maxDebuffElementCount);

    public Dictionary<BulletElementType, BulletElementData> initialElementTypes = new Dictionary<BulletElementType, BulletElementData>(SYSTEM_VAR.maxDebuffElementCount);
    public Dictionary<DebuffElementEffectType, DebuffElementData> initialDebuffTypes = new Dictionary<DebuffElementEffectType, DebuffElementData>(SYSTEM_VAR.maxDebuffElementCount);

    private List<CardEffectCommand> cardLogicSystemEffects = new List<CardEffectCommand>(3);
    private List<CardEffectCommand> cardDataControlSystemEffects = new List<CardEffectCommand>(3);
    private List<CardEffectCommand> cardStatusEffects = new List<CardEffectCommand>(3);
    private List<CardEffectCommand> cardSlotSystemEffects = new List<CardEffectCommand>(3);
    private List<CardEffectCommand> complexSystemEffects = new List<CardEffectCommand>(3);
    private List<CardEffectCommand> selectionSystemEffects = new List<CardEffectCommand>(3);
    private CardEffectCommand HandPileExistEffect;

    public List<CardEffectCommand> GetcardLogicSystemEffects() { return cardLogicSystemEffects; }
    public List<CardEffectCommand> GetcardDataControlSystemEffects() { return cardDataControlSystemEffects; }
    public List<CardEffectCommand> GetcardStatusEffects() { return cardStatusEffects; }
    public List<CardEffectCommand> GetcardSlotSystemEffects() { return cardSlotSystemEffects; }
    public List<CardEffectCommand> GetcomplexSystemEffects() { return complexSystemEffects; }
    public List<CardEffectCommand> GetselectionSystemEffects() { return selectionSystemEffects; }
    public CardEffectCommand GetHandPileExistEffect() { return HandPileExistEffect; }

    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;
        ResetState();

        ReadyEffects();
    }

    private void ReadyEffects()
    {
        for (int i = 0; i < cardData.defaultElementTypes.Count; ++i)
        {
            elementTypes[cardData.defaultElementTypes[i].bulletElementType] = new BulletElementData(cardData.defaultElementTypes[i].bulletElementType, cardData.defaultElementTypes[i].nestingCnt);
            initialElementTypes[cardData.defaultElementTypes[i].bulletElementType] = new BulletElementData(cardData.defaultElementTypes[i].bulletElementType, cardData.defaultElementTypes[i].nestingCnt);
        }

        for (int i = 0; i < cardData.defaultdebuffTypes.Count; ++i)
        {
            debuffTypes[cardData.defaultdebuffTypes[i].debuffElementType] = new DebuffElementData(cardData.defaultdebuffTypes[i].debuffElementType, cardData.defaultdebuffTypes[i].turnCnt);
            initialDebuffTypes[cardData.defaultdebuffTypes[i].debuffElementType] = new DebuffElementData(cardData.defaultdebuffTypes[i].debuffElementType, cardData.defaultdebuffTypes[i].turnCnt);
        }

        for (int i = 0; i < cardData.cardLogicSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(cardData.cardLogicSystemEffects_Prefab[i]);

            cardLogicSystemEffects.Add(commands);
        }

        for (int i = 0; i < cardData.cardStatusEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(cardData.cardStatusEffects_Prefab[i]);

            cardStatusEffects.Add(commands);
        }

        for (int i = 0; i < cardData.cardDataControlSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(cardData.cardDataControlSystemEffects_Prefab[i]);

            cardDataControlSystemEffects.Add(commands);
        }

        for (int i = 0; i < cardData.cardSlotSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(cardData.cardSlotSystemEffects_Prefab[i]);

            cardSlotSystemEffects.Add(commands);
        }

        for (int i = 0; i < cardData.complexSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(cardData.complexSystemEffects_Prefab[i]);

            complexSystemEffects.Add(commands);
        }

        for (int i = 0; i < cardData.selectionSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(cardData.selectionSystemEffects_Prefab[i]);

            selectionSystemEffects.Add(commands);
        }

        if (cardData.HandPileExistEffect_Prefab != null)
            HandPileExistEffect = UnityEngine.Object.Instantiate(cardData.HandPileExistEffect_Prefab);
    }

    public void ResetElement_Debuff()
    {
        elementTypes.Clear();
        debuffTypes.Clear();

        for (int i = 0; i < cardData.defaultElementTypes.Count; ++i)
        {
            if (elementTypes.ContainsKey(cardData.defaultElementTypes[i].bulletElementType))
                elementTypes[cardData.defaultElementTypes[i].bulletElementType] = new BulletElementData(cardData.defaultElementTypes[i].bulletElementType, cardData.defaultElementTypes[i].nestingCnt);
        }
        for (int i = 0; i < cardData.defaultdebuffTypes.Count; ++i)
        {
            if (debuffTypes.ContainsKey(cardData.defaultdebuffTypes[i].debuffElementType))
                debuffTypes[cardData.defaultdebuffTypes[i].debuffElementType] = new DebuffElementData(cardData.defaultdebuffTypes[i].debuffElementType, cardData.defaultdebuffTypes[i].turnCnt);
        }
    }

    public void ResetState()
    {
        ResetElement_Debuff();
    }

    public ICardDataProvider GetCardDataProvider()
    {
        return cardData;
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void ResetCardData()
    {
        valueModifier = 1;
    }

    public void SetUpgrade(bool boolean)
    {
        bUpgrade = boolean;
    }

    public void SetPermanentlyUpgrade(bool boolean)
    {
        bPermanentUpgrade = boolean;
    }

    public bool IsUpgraded()
    {
        return bUpgrade || bPermanentUpgrade;
    }
}
