using DG.Tweening;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

using Image = UnityEngine.UI.Image;

public class BattleDeckSystem : BaseDeckSystem
{
    [Header("Main Binding")]
    public GameObject impactEffectPrefab = null;
    public CountUI deckCount = null;
    private UIView_CardSystem cardSystem = null;
    private ParticleSystem impactParticle = null;

    [Header("CardBack Event for Drawed")]
    [SerializeField] protected Ease drawedCardBackEase = Ease.OutExpo;

    protected override void Awake()
    {
        base.Awake();

        impactParticle = impactEffectPrefab?.GetComponentInChildren<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    public void CardDrawEffect(List<ICardDataInstanceProvider> dataList)
    {
        if (null == cardSystem)
            return;

        // 드로우 타이밍에 패널이 덱 타입으로 열려 있다면 강제로 끔
        cardSystem.ForceDeActivatePannelSelf(CardZone.Deck);

        int currentDrawCount = dataList.Count;
        for (int i = 0; i < currentDrawCount; i++)
        {
            GameObject performer = cardSystem.GetStarPerformerFromPool(this.transform);
            VFX_CardStar script = performer?.GetComponent<VFX_CardStar>();
            if (null == script)
                continue;

            Vector3[] pathPoints = cardSystem.PathSystem?.GetDragPath(performer,
                transform.position, cardSystem.GetHandTargetEndPos(i), drawDragPower, DragDir.UP);

            script.CardDataInstance = dataList[i];
            script.PlayingEventforDeck(i, currentDrawCount - 1, drawDelay, drawDuration, drawEase, pathPoints);
        }
    }

    public void CardBackDrawedEffect()
    {
        if (null == cardBackRect || null == impactParticle)
            return;

        cardBackRect.localEulerAngles = new Vector3(0f, 0f, Random.Range(-10f, 10f));
        cardBackRect.localScale = cardbackOriginScale * 0.85f;

        CancelPrevMotion(cardbackSeq);

        cardbackSeq = DOTween.Sequence();

        cardbackSeq.Append(cardBackRect.DOLocalRotate(Vector3.zero, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase)
            .OnComplete(() =>
            {
                cardBackRect.localEulerAngles = Vector3.zero;
            }));

        cardbackSeq.Join(cardBackRect.DOScale(cardbackOriginScale, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase));

        impactParticle.Stop();
        impactParticle.Play();
    }

    public void InDeckMotion()
    {
        if (null == cardBackRect || null == impactParticle)
            return;

        cardBackRect.localEulerAngles = new Vector3(0f, 0f, Random.Range(-10f, 10f));
        cardBackRect.localScale = cardbackOriginScale * 0.85f;

        CancelPrevMotion(cardbackSeq);

        cardbackSeq = DOTween.Sequence();

        cardbackSeq.Append(cardBackRect.DOLocalRotate(Vector3.zero, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase)
            .OnComplete(CardBackDrawedEffectCompleteEvent));

        cardbackSeq.Join(cardBackRect.DOScale(cardbackOriginScale, drawDelay)
            .SetUpdate(false)
            .SetEase(drawedCardBackEase));

        impactParticle.Stop();
        impactParticle.Play();
    }

    public void SetCount(int _count) => deckCount?.SetCount(_count);

    public void AddCount(int _count) => deckCount?.SetCount(deckCount.GetCount() + _count);

    public void SetupCount(CountUIType _type, int _count) => deckCount?.TypeSetting(_type, _count);

    public override void OnPointerDown(PointerEventData _eventData)
    {
        if (true == cardSystem?.WorkingBlock)
            return;

        base.OnPointerDown(_eventData);
    }

    public override void OnPointerUp(PointerEventData _eventData)
    {
        if (true == cardSystem?.WorkingBlock)
            return;

        base.OnPointerUp(_eventData);
        cardSystem?.CallPannel(CardZone.Deck);
    }

    public override void OnPointerEnter(PointerEventData _eventData)
    {
        base.OnPointerEnter(_eventData);
    }

    public override void OnPointerExit(PointerEventData _eventData)
    {
        base.OnPointerExit(_eventData);
    }
}
