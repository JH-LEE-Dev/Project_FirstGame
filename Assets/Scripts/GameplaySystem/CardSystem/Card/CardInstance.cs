using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class CardInstance : MonoBehaviour
{
    [Header("Refs: View")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Image cardFrame;
    [SerializeField] private Image cardIcon;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private Sprite rotationIcon;
    [SerializeField] private Sprite extinctionIcon;

    private static readonly Color BulletColor = new Color32(255, 210, 102, 255);
    private static readonly Color MagicColor = new Color32(102, 190, 255, 255);

    public CardInstanceType cardInstanceType { get; private set; }


    // 데이터
    private CardDataInstance cardData;
    public CardDataInstance CardData => cardData;


    // 시스템
    private UIView_CardSystem cardSystem;
    public UIView_CardSystem CardSystem => cardSystem;


    // 컴포넌트
    public CardMotion Motion { get; private set; }
    public CardVisualFloat VisualFloat { get; private set; }
    public CardInput Input { get; private set; }

    private void Awake()
    {
        Motion = GetComponent<CardMotion>();
        Input = GetComponent<CardInput>();
        VisualFloat = GetComponentInChildren<CardVisualFloat>(true);

        if (Motion) Motion.Bind(this);
        if (Input) Input.Bind(this);
        if (VisualFloat) VisualFloat.Bind(this);
    }

    public void Initialize(UIView_CardSystem system, CardInstanceType type)
    {
        cardSystem = system;
        cardInstanceType = type;
    }

    // 옷 입히기
    public void ApplyData(CardDataInstance dataInstance)
    {
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
        if (!cardFrame) return;

        switch (type)
        {
            case CardType.Bullet: cardFrame.color = BulletColor; break;
            case CardType.Magic: cardFrame.color = MagicColor; break;
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