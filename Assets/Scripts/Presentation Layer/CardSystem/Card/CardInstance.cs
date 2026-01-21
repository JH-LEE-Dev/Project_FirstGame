using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChocDino.UIFX;


public class CardInstance : MonoBehaviour
{
    [Header("Refs: View")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Image cardFrame;
    [SerializeField] private Image cardTextFrame;
    [SerializeField] private GlowFilter glowFilter;
    [SerializeField] private Image CardAO;

    [SerializeField] private Image cardIcon;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private Sprite rotationIcon;
    [SerializeField] private Sprite extinctionIcon;

    private readonly Color bulletFrameColor = new Color32(183, 66, 81, 255);
    private readonly Color bulletTextFrameColor = new Color32(233, 180, 166, 255);
    private readonly Color bulletGlowColor = new Color32(121, 13, 22, 255);
    private readonly Color bulletAOColor = new Color32(41, 31, 22, 38);

    private readonly Color magicFrameColor = new Color32(80, 70, 214, 255);
    private readonly Color magicTextFrameColor = new Color32(198, 214, 255, 255);
    private readonly Color magicGlowColor = new Color32(5, 39, 176, 255);
    private readonly Color magicAOColor = new Color32(33, 38, 61, 38);

    // 데이터
    private CardDataInstance cardData;
    public CardDataInstance CardData => cardData;

    private void Awake()
    {

    }

    // 옷 입히기
    public void ApplyData(CardDataInstance dataInstance)
    {
        if (dataInstance == null) return;

        ResetVisual();

        cardData = dataInstance;
        CardData data = cardData.GetCardData();

        CardImageChange(data.id);
        CardFrameChange(data.cardType);
        CardIconChange(data.elementType);
        CardNameChange(data.id);
        CardDescriptionChange(data.id);
    }

    public void Clear()
    {
        cardData = null;
        ResetVisual();
    }

    private void ResetVisual()
    {
        if (cardFrame) cardFrame.color = Color.white;
        if (cardImage) cardImage.sprite = null;
        if (cardIcon) cardIcon.sprite = null;

        if (cardName) cardName.text = string.Empty;
        if (cardDescription) cardDescription.text = string.Empty;
    }

    private void CardImageChange(int id)
    {
        // TODO: sprite 적용
        // cardImage.sprite = ...
    }

    private void CardFrameChange(CardType type)
    {
        //if (!cardFrame || !cardTextFrame || !CardAO || !glowFilter) return;


        //switch (type)
        //{
        //    case CardType.Bullet: 
        //        cardFrame.color = bulletFrameColor;
        //        cardTextFrame.color = bulletTextFrameColor;
        //        CardAO.color = bulletAOColor;
        //        glowFilter.Color = bulletGlowColor;
        //        break;
        //    case CardType.Magic: 
        //        cardFrame.color = magicFrameColor;
        //        cardTextFrame.color = magicTextFrameColor;
        //        CardAO.color = magicAOColor;
        //        glowFilter.Color = magicGlowColor;
        //        break;
        //}

        if (!cardFrame || !cardTextFrame || !CardAO || !glowFilter) return;

        int i = Random.Range(0, 2);

        switch (i)
        {
            case 0:
                cardFrame.color = bulletFrameColor;
                cardTextFrame.color = bulletTextFrameColor;
                CardAO.color = bulletAOColor;
                glowFilter.Color = bulletGlowColor;
                break;
            case 1:
                cardFrame.color = magicFrameColor;
                cardTextFrame.color = magicTextFrameColor;
                CardAO.color = magicAOColor;
                glowFilter.Color = magicGlowColor;
                break;
        }

    }
    private void CardIconChange(ElementType type)
    {
        if (!cardIcon) return;

        switch (type)
        {
            case ElementType.Rotation: cardIcon.sprite = rotationIcon; break;
            case ElementType.Extinction: cardIcon.sprite = extinctionIcon; break;
        }
    }

    private void CardNameChange(int id)
    {
        if (cardName) cardName.SetText("Name Test");
    }

    private void CardDescriptionChange(int id)
    {
        if (cardDescription) cardDescription.SetText("Description Change OK");
    }

}