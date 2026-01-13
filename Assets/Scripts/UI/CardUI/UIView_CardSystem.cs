using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIView_CardSystem : UIView
{
    public event Action UICommandCompleteEvent;

    //외부 의존성
    private ICardSystemProvider cardSystemProvider;

    //사용 승인을 받은 카드
    private CardInstance verificationWaitCard;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [Space]
    [SerializeField] private TMP_Text deckCntText;
    [SerializeField] private TMP_Text graveCntText;
    [SerializeField] private TMP_Text handCntText;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button turnFinishedButton;
    ////////////

    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private ClickCatchSystem clickCatchSystem;

    [SerializeField] private HandSystem handSystem;
    public HandSystem HandSystem => handSystem;
    [SerializeField] private DeckSystem deckSystem;
    public DeckSystem DeckSystem => deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 묘지
    [Header("Graveyard Settings")]
    [SerializeField] private GraveyardSystem graveSystem = null;

    // 소멸
    [Header("Extinction Settings")]
    [SerializeField] private ExtinctionSystem extinctionSystem = null;

    // 덱, 묘지, 소멸 공용
    [Header("Pannel")]
    [SerializeField] private CardPannel cardPannel = null;
    [SerializeField] private GameObject pannelContent = null;
    public GameObject PannelContent { get { return pannelContent; } }

    // 드로우 중 작업 중지
    private bool bWorkingBlock = false;
    public bool WorkingBlock { get { return bWorkingBlock; } set { bWorkingBlock = value; } }

    public MeshRenderer testImpact = null;
    private Material mat = null;
    private ParticleSystem particle = null;

    public void DependencyInjection(ICardSystemProvider _cardSystemProvider)
    {
        cardSystemProvider = _cardSystemProvider;
    }

    public override void OnDestroy()
    {
        UICommandCompleteEvent = null;
    }

    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this);
        handSystem?.Init(this);
        deckSystem?.Init(this);
        graveSystem?.Init(this);
        extinctionSystem?.Init(this);
        clickCatchSystem?.Init(this);

        Test();
    }

    public void Test()
    {
        mat = testImpact?.material;
        particle = testImpact?.gameObject.GetComponentInChildren<ParticleSystem>();

        DG.Tweening.Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            if (particle != null)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Play();
            }
            mat.SetFloat("_Ratio", 0f);
        });

        seq.Append(mat.DOFloat(1f, "_Ratio", 2f)
            .SetEase(Ease.OutQuad));

        seq.SetLoops(-1);
    }

    // For PoolingSystem
    public CardInstance RentHandCard()
    {
        return poolingSystem?.RentHandCard();
    }
    public void ReturnHandCard(CardInstance card)
    {
        poolingSystem?.ReturnHandCard(card);
    }
    /////////////////


    // For HandSystem
    public void TryUseCard(CardInstance _card)
    {
        //카드 사용 승인 대기 카드
        verificationWaitCard = _card;

        cardSystemProvider.CardUsed(_card.CardData);
    }

    public void CardUsingApproved(bool boolean) // true이면 verificationWaitCard -> 사용 승인.
    {
        if (boolean)
        {
            // 우클릭을 했을 때 이쪽으로 온다. (즉시 사용)
            handSystem?.UseCard(verificationWaitCard);


        }
        else
        {
            //카드 사용 실패.
            Debug.Log("이 카드를 사용할 수 없습니다.");

            verificationWaitCard.Motion.PlayReject();

        }
    }

    public void OnCardLeftClick(CardInstance _card)
    {
        // 좌클릭을 했을 때 이쪽으로 온다. (프리뷰)
        handSystem?.OnCardLeftClick(_card);
    }

    public void OnCardHoverEnter(CardInstance _card)
    {
        // 호버 ON (벌어지는 연출위함)
        handSystem?.OnCardHoverEnter(_card);
    }
    public void OnCardHoverExit(CardInstance _card)
    {
        // 호버 OFF (축소되는 연출 위함)
        handSystem?.OnCardHoverExit(_card);
    }

    public void CancelPreview()
    {
        handSystem?.CancelPreview();
    }

    // state에 맞는 카드들이 묘지로 빨려들어가는 기능
    public void AllCardReturnToPool(CardState state)
    {
        handSystem?.AllCardReturnToPool(state);
    }
    /////////////////

    public Vector3 GetGraveAnchoredPos()
    {
        if (graveSystem == null) return Vector3.zero;
        return graveSystem.GetComponent<RectTransform>().anchoredPosition;
    }

    private void ActivatePannel(IReadOnlyList<CardDataInstance> _inCards)
    {
        if (null == poolingSystem || null == pannelContent)
            return;

        var pool = poolingSystem.OtherCardPool;

        int inCount = _inCards.Count;
        int poolCount = pool.Count;

        if (0 >= poolCount || inCount > poolCount)
            return;

        for (int i = 0; i < poolCount; ++i)
        {
            if(i < inCount)
            {
                pool[i].ApplyData(_inCards[i]);
                pool[i].transform.SetParent(pannelContent.transform);
                pool[i].gameObject.SetActive(true);
            }
            else
                pool[i].gameObject.SetActive(false);
        }
    }

    // 현재 패 개수 + 지금 들어오는 패에 몇 번째로 들어오는 애인지
    // 

    public Vector2 GetHandTargetEndPos(int currentDrawIdx)
    {
        if (null == handSystem)
            return Vector2.zero;

        Camera cam = Camera.main;

        int currHandCnt = handSystem.GetCurrentHandCardCount();
        Vector2 NextEndPos = handSystem.PredictRightmostPosForCount(currHandCnt + (currentDrawIdx + 1));

        return NextEndPos;
    }

    public void CallPannel(CurrentPannel _setType)
    {
        if (null == cardPannel)
            return;

        cardPannel.CurrPannelType = _setType;
        cardPannel.gameObject.SetActive(true);

        switch(_setType)
        {
            case CurrentPannel.Deck: 
                ActivatePannel(cardSystemProvider.deckCards); 
                break;

            case CurrentPannel.Grave:
                ActivatePannel(cardSystemProvider.graveCards);
                break;

            case CurrentPannel.Extinction:
                //ActivatePannel(cardSystemProvider.cards);
                break;
        }
    }

    public void ForceDeActivatePannelSelf(CurrentPannel callType)
    {
        if (null == cardPannel || callType != cardPannel.CurrPannelType)
            return;

        cardPannel.gameObject.SetActive(false);
    }

    public void CallOneCardDrawed(int currIdx, int _lastIdx, Vector3 _endPos, CardDataInstance _data, GameObject _performer)
    {
        if (currIdx == _lastIdx)
            WorkingBlock = false;

        handSystem?.ProcessDraw(_endPos, _data);
        poolingSystem?.StarEffects?.Release(_performer);
    }

    public void CallGraveToDeckFinished(int currIdx, GameObject _performer)
    {
        // 풀링한테 지워달라고 요청
        poolingSystem?.StarEffects?.Release(_performer);
        // 덱 받은 모션 재생
        deckSystem?.InDeckFromGraveMotion();

        // 현재 받은 인덱스가 마지막 주자 인덱스랑 같으면 마무리 모션
        graveSystem?.MoveToDeckFinishMotion(currIdx);
    }

    public void PlayDrawedEffect() => deckSystem?.CardBackDrawedEffect();
    public void PlayMoveToDeckMotion() => graveSystem?.CardMoveToDeckMotion();

    public Vector3 GetDeckWorldPos()
    {
        if (null == deckSystem)
            return Vector3.zero;

        return deckSystem.transform.position;
    }

    public GameObject GetStarPerformerFromPool(Transform target)
    {
        GameObject getObj = poolingSystem?.StarEffects.Get();

        if (null != getObj)
            getObj.transform.position = target.position;

        return getObj;
    }
    /////////////////////////////////////////////////

    private void SetText()
    {
        deckCntText.text = "Deck : " + cardSystemProvider.deckCards.Count.ToString();
        graveCntText.text = "Grave : " + cardSystemProvider.graveCards.Count.ToString();
        handCntText.text = "Hand : " + cardSystemProvider.handCards.Count.ToString();
    }

    protected override void OnShow()
    {
        base.OnShow();

        SetText();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void RenderUI()
    {

    }

    public void CardUsingFinished()
    {
        handSystem?.CancelPreview();

        turnFinishedButton.gameObject.SetActive(false);
        cardSystemProvider.CardUsingFinished();
        SetText();
    }

    public void CardDrawFinished()
    {
        turnFinishedButton.gameObject.SetActive(true);
    }

    public void EnemyTurnStarted()
    {
        //handRoot.gameObject.SetActive(false);
    }

    public void PlayerTurnStarted(int waveIdx)
    {
        //handRoot.gameObject.SetActive(true);
    }

    public async void RecieveUIJob(List<Job_CardSystemUI> _jobQueue)
    {
        var currentBatchList = _jobQueue;

        float turnWaitSecond = 0.5f;

        int size = currentBatchList.Count;
        for (int i = 0; i < size; ++i)
        {
            Job_CardSystemUI currentJob = currentBatchList[i];

            JobType_CardSystemUI currenType = currentJob.jobType;

            switch(currenType)
            {
                case JobType_CardSystemUI.Draw:

                    DrawingCards(currentJob.cards);

                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;

                case JobType_CardSystemUI.GraveToDeck:

                    graveSystem?.CardMoveToDeckEffect(currentBatchList[i].cards.Count);

                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;

                case JobType_CardSystemUI.AdditionalDraw:

                    DrawingCards(currentJob.cards);

                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;
                case JobType_CardSystemUI.HandToGrave:

                    AllCardReturnToPool(CardState.InHand);
                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;

                default: break;
            }
        }

        SetText();

        UICommandCompleteEvent?.Invoke();
    }

    void DrawingCards(List<CardDataInstance> _datas)
    {
        if (null == deckSystem)
            return;

        bWorkingBlock = true;
        deckSystem.CardDrawEffect(_datas);
    }
}
