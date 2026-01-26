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

    private static Color Hex(string hex, float alpha)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        c.a = alpha;
        return c;
    }

    private readonly Color bulletFrameColor = Hex("#DEAB48", 1f);
    private readonly Color bulletTextFrameColor = Hex("#FAE1AA", 1f);
    private readonly Color bulletGlowColor = new Color32(145, 181, 32, 72);
    private readonly Color bulletAOColor = Hex("#7B6D21", 0.3f);

    private readonly Color magicFrameColor = Hex("#696EC2", 1f);
    private readonly Color magicTextFrameColor = Hex("#DAD5ED", 1f);
    private readonly Color magicGlowColor = new Color32(5, 93, 176, 109);
    private readonly Color magicAOColor = Hex("#0025CD", 0.3f);

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

        CardImageChange(data.id); //
        CardFrameChange(data.cardType);
        CardIconChange(data.elementType);
        CardNameChange(data.id); //
        CardDescriptionChange(data.id); //
    }

    public void Clear()
    {
        cardData = null;
        ResetVisual();
    }

    public void TurnOffGlowFilter() => glowFilter?.gameObject.SetActive(false);

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
        if (!cardFrame || !cardTextFrame || !CardAO || !glowFilter) return;

        switch (type)
        {
            case CardType.Bullet:
                cardFrame.color = bulletFrameColor;
                cardTextFrame.color = bulletTextFrameColor;
                CardAO.color = bulletAOColor;
                glowFilter.Color = bulletGlowColor;
                break;
            case CardType.Magic:
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