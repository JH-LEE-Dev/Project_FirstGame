using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIView_CardSystem : UIView
{
    // 신경 쓰지 말기
    public event Action TurnFinishedEvent;
    public event Action<CardDataInstance> CardUsedEvent;

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

    public Action<Vector3, CardDataInstance> DrawEvent;

    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private ClickCatchSystem clickCatchSystem;

    [SerializeField] private HandSystem handSystem;
    public HandSystem HandSystem => handSystem;
    [SerializeField] private DeckSystem deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 덱
    [Header("Deck Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();
    [SerializeField] private RectTransform drawEndPoint = null;
    public List<RectTransform> DrawPathPoints { get { return drawPathPoints; } }
    public RectTransform DrawEndPoint { get { return drawEndPoint; } }

    // 덱, 묘지, 소멸 공용
    [Header("Pannel")]
    [SerializeField] private GameObject cardPannel = null;
    [SerializeField] private GameObject pannelContent = null;
    public GameObject PannelContent {  get { return pannelContent; } }

    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this);
        handSystem?.Init(this);
        deckSystem?.Init(this);
        clickCatchSystem?.Init(this);

        BindingFunction();
    }

    private void BindingFunction()
    {
        if(null != handSystem)
        {
            DrawEvent += handSystem.ProcessDraw;
        }
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
        CardUsedEvent?.Invoke(_card.CardData);

        // 우클릭을 했을 때 이쪽으로 온다. (즉시 사용)
        handSystem?.TryUseCard(_card);
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
    /////////////////




    public void GetDeckCards()
    {
        List<CardDataInstance> temp;

        // 상우 자아끼리 얘기 중
        // 추후 구현
        // 덱에다가 넘길 필요가 있나? 하나의 패널로 다 표현이 가능하면 여기서 하는 게 맞는 것 같은데

        // 리스트를 받으면 이 리스트를 가지고 풀링 데이터를 덮어씌우고 그 데이터를 가지고 여기서
        // pannelContent로 부모 객체를 옮겨가서 자동으로 Scroll View에 채워지게 슬더스 처럼 만들면 됨.

        // 만약 카드마다 연출이 필요하면? 카드한테 모션 cs 달아서 그걸 호출하는 게 맞을 듯? 여기저기서 이동을 다 만들지 말고
        // 그러면 영우형처럼 카드 렌트? ㄴㄴ 어차피 덱,묘,소 세개 중에 하나만 볼 수 있기 때문에 굳이 컨테이너 옮겨 다닐 필요 없음
        // 풀링 리스트 데이터에 바로 쓰고, 나가기 버튼 누르면 데이터 일괄 초기화 혹은 냅둬도 됨 문제 없음, 어차피 다음 이용에 덮어 씌워질 거임
        // 0 ~ N 까지 다 덮어 씌우고 N까지만 부모 객체를 옮길 거니까

        // 그럼 하나의 함수를 만들어서 각 Get 에서 그 함수 호출만 시키면 되게끔 ㄱㄱ
        // 그리고 덱,묘,소 모두 카드 등장 애니메이션이 달라야 하면 카드 프리팹에 모션 cs 넣고 거기서 여러 개 만들어 놓으면 될듯
        // 1. 리스트 받고 > 2. 부모 객체, 시블링 인덱스 보관 후 콘텐트로 옮겨가고 > 3. 연출 또는 액티브 켜고 > 4. 나가기 버튼 누르면 복구
    }

    public void GetWormholeCards()
    {
        List<CardDataInstance> temp;

        // 추후 구현
    }

    public void GetExtinctionCards()
    {
        List<CardDataInstance> temp;

        // 추후 구현
    }

    public void CardDrawed(List<CardDataInstance> cardDataPile)
    {
        if (null == deckSystem)
            return;

        deckSystem.CardDrawEffect(cardDataPile);
        SetText();
    }

    public void CallCardPannel(bool _activate)
    {
        cardPannel?.SetActive(_activate);
        Debug.Log("호출");
    }

    /////////////////////////////////////////////////



    private void SetText()
    {
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
        turnFinishedButton.gameObject.SetActive(false);
        TurnFinishedEvent?.Invoke();

        SetText();

        ClearAllCards();
    }

    public void ClearAllCards()
    {
        //for(int i = 0; i < cards.Count;++i)
        //{
        //    RectTransform card = cards[i].GetComponent<RectTransform>();

        //    card.localPosition = new Vector3(-1000,-1000,card.localPosition.z);
        //}

        //cards.Clear();

        //computeArc();
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
}
