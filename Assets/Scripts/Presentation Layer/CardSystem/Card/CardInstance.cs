using ChocDino.UIFX;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

    protected Material dissolveMatInstance;
    private static readonly int ProgressID = Shader.PropertyToID("_Progress");
    private float dissolve01 = 1f;
    private Tween consumeTween;
    private System.Action<CardInstance> consumeComplete;

    protected ICardLocalizationSystem cardLocalizationSystem;

    [SerializeField] private TMPTextFxController fxNameController;
    [SerializeField] private TMPTextFxController fxDescriptionController;


    private float GetDissolve01() => dissolve01;
    private void SetDissolve01(float v)
    {
        dissolve01 = v;
        SetDissolveProgress(v);
        ApplyNonDissolveFade(v); // 텍스트/AO/Glow 같이 페이드
    }

    private static Color Hex(string hex, float alpha)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        c.a = alpha;
        return c;
    }

    private readonly Color bulletFrameColor = Hex("#DEAB48", 1f);
    private readonly Color bulletTextFrameColor = Hex("#FAE1AA", 1f);
    private readonly Color bulletGlowColor = new Color32(145, 181, 32, 72);
    private readonly Color bulletAOColor = Hex("#7B6D21", 0.15f);

    private readonly Color magicFrameColor = Hex("#696EC2", 1f);
    private readonly Color magicTextFrameColor = Hex("#DAD5ED", 1f);
    private readonly Color magicGlowColor = new Color32(5, 93, 176, 109);
    private readonly Color magicAOColor = Hex("#0025CD", 0.15f);

    private float nameBaseAlpha = 1f;
    private float descBaseAlpha = 1f;
    private float aoBaseAlpha = 1f;
    private Color glowBaseColor;

    // 데이터
    private ICardDataInstanceProvider cardData;
    public ICardDataInstanceProvider CardData => cardData;


    private void Awake()
    {

    }

    public virtual void Initialize(Material template, ICardLocalizationSystem cls)
    {
        dissolveMatInstance = new Material(template);
        ApplyDissolveMaterialToVisuals();
        cardLocalizationSystem = cls;
    }

    protected void ApplyDissolveMaterialToVisuals()
    {
        if (dissolveMatInstance == null)
        {
            Debug.LogWarning("Dissolve material instance is null");
            return;
        }

        // Image들
        if (cardImage) cardImage.material = dissolveMatInstance;
        if (cardFrame) cardFrame.material = dissolveMatInstance;
        if (cardTextFrame) cardTextFrame.material = dissolveMatInstance;
        if (cardIcon) cardIcon.material = dissolveMatInstance;
        // 초기값
        dissolveMatInstance.SetFloat(ProgressID, 1f);
    }

    // 옷 입히기
    public void ApplyData(ICardDataInstanceProvider dataInstance)
    {
        if (dataInstance == null)
        {
            Debug.Log("NULLCard");
            return;
        }

        ResetVisual();

        cardData = dataInstance;
        ICardDataProvider data = cardData.GetCardDataProvider();

        CardImageChange(data.cardImage);
        CardFrameChange(data.cardType);
        CardIconChange(data.elementType);
        CardNameAndDescriptionChange(cardData);
    }

    public void Clear()
    {
        cardData = null;
        ResetVisual();
        ResetDissolve();
    }

    public void TurnOffGlowFilter() => glowFilter?.gameObject.SetActive(false);

    private void ResetVisual()
    {
        if (cardFrame) cardFrame.color = Color.white;
        if (cardImage) cardImage.sprite = null;
        if (cardIcon) cardIcon.sprite = null;

        if (cardName)
        {
            cardName.text = string.Empty;
            cardName.alpha = descBaseAlpha;
        }
        if (cardDescription) 
        {
            cardDescription.text = string.Empty;
            cardDescription.alpha = descBaseAlpha;
        }
    }

    private void CardImageChange(Sprite _cardImage)
    {
        cardImage.sprite = _cardImage;
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

    private void CardNameAndDescriptionChange(ICardDataInstanceProvider dataInstance)
    {
        if (!cardName) return;

        ICardDataProvider cardData = dataInstance.GetCardDataProvider();

        // 강화됨
        if (dataInstance.IsUpgraded() == true)
            cardLocalizationSystem.SetCardUIText(cardData.id, null, cardName, null, cardDescription);
        // 강화안됨
        else
            cardLocalizationSystem.SetCardUIText(cardData.id, cardName, null, cardDescription, null);


        if (fxNameController) fxNameController.SetTaggedText(cardName.text);
        if (fxDescriptionController) fxDescriptionController.SetTaggedText(cardDescription.text);
    }

    public void UpdateForEnforce()
    {
        // 연출은 나중에.
        CardNameAndDescriptionChange(cardData);
    }

    // For Dissolve
    public void SetDissolveProgress(float t)
    {
        dissolveMatInstance?.SetFloat(ProgressID, t);
    }

    public void ResetDissolve()
    {
        dissolveMatInstance?.SetFloat(ProgressID, 1f);
    }

    private void CacheBaseIfNeeded()
    {
        if (cardName) nameBaseAlpha = cardName.alpha;
        if (cardDescription) descBaseAlpha = cardDescription.alpha;
        if (CardAO) aoBaseAlpha = CardAO.color.a;
        if (glowFilter) glowBaseColor = glowFilter.Color;
    }

    public void PlayConsumeExtinction(
        float dissolveDur,
        System.Action<CardInstance> onComplete)
    {

        CacheBaseIfNeeded();

        consumeTween?.Kill();
        consumeTween = null;

        consumeComplete = onComplete;

        SetDissolve01(1f);

        consumeTween = DG.Tweening.DOTween.To(
                GetDissolve01,
                SetDissolve01,
                0f,
                dissolveDur
            )
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(OnConsumeTweenComplete);
    }

    private void OnConsumeTweenComplete()
    {
        consumeTween = null;
        var cb = consumeComplete;
        consumeComplete = null;
        cb?.Invoke(this);
    }

    private void ApplyNonDissolveFade(float t01)
    {
        float a = t01 * t01;

        if (cardName) cardName.alpha = nameBaseAlpha * a;
        if (cardDescription) cardDescription.alpha = descBaseAlpha * a;

        if (CardAO)
        {
            var c = CardAO.color;
            c.a = aoBaseAlpha * a;
            CardAO.color = c;
        }

        if (glowFilter)
        {
            var gc = glowBaseColor;
            gc.a = glowBaseColor.a * a;
            glowFilter.Color = gc;
        }
    }
}