using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CardGameUIManager : UIManager
{
    public new static CardGameUIManager i;

    [Header("Submenus")]
    [SerializeField] private Deck _deck;
    [SerializeField] private Hand _hand;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _instructionsText;
    [SerializeField] private GameObject _instructionsParent;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;
    [SerializeField] private Transform _currentPlayedCardParent;
    [SerializeField] private GameObject _bottomBar;
    [SerializeField] private GameObject _cardInfoParent;
    [SerializeField] private PlayableCardDisplay _cardInfo;
    [SerializeField] private DeckDisplayController _deckDisplayController;

    [Header("backings")]
    [SerializeField] private List<SelectableItem> _bottomBarBackings = new List<SelectableItem>();

    [Header("AP meter")]
    [SerializeField] private TMP_Text _actionsPointsText;
    [SerializeField] private Slider _actionPointsSlider;
    [SerializeField] private TMP_Text _victoryScreenTempText;
    [SerializeField] private GameObject _ApSliderParent;

    [Header("Combat log")]
    [SerializeField] private TextMeshProUGUI _combatLogPreviewText;
    [SerializeField] private TextMeshProUGUI _combatLogMainText;

    [Header("Card Targeting")]
    [SerializeField] private GameObject _targetingArrowPrefab;
    [SerializeField] private GameObject _currTargetingArrow;

    private CardGameManager gMan => CardGameManager.i;
    
    public void Draw(int count = 1) => _deck.Draw(count: count);
    public void Discard(int count = 1) => _hand.Discard(count: count);
    public void AddToDiscardPile(CardInstance inst, int count = 1) => _deck.AddToDiscard(inst, count: count);
    public void AddToDeck(CardInstance inst, int count = 1, bool random = true) => _deck.AddToDeck(inst, count: count, random: random);
    public void HideInstructions() => _instructionsParent.SetActive(false);
    public void DisplayDefeatScreen() => _defeatScreen.SetActive(true);
    public void ToggleCameraPerspective() => gMan.ToggleCamera();
    public void StopPlayingCards() => _hand.StopPlayingCards();
    public void EndTurn() => gMan.EndPlayerTurn();
    public int GetHandSize() => _hand.GetHandSize();
    public List<CardInstance> GetDrawPile() => _deck.GetDrawPile();
    public List<CardInstance> GetDiscardPile() => _deck.GetDiscardPile();
    protected override void Awake()
    {
        base.Awake();
        i = this;

        gMan.OnStartCombat.AddListener(StartCombat);
        gMan.OnStartPlayerTurn.AddListener(StartPlayerTurn);
        gMan.OnEndPlayerTurn.AddListener(EndPlayerTurn);
        gMan.OnStartEnemyTurn.AddListener(StartEnemyTurn);

        _combatLogMainText.text = _combatLogPreviewText.text = "";
    }

    public void LogMove(string move)
    {
        _combatLogPreviewText.text = move;
        _combatLogMainText.text += "\n" + move;
    }

    public void MoveCardFromDisplay(CardObject cardObj)
    {
        HideInstructions();
        cardObj.ReturnToHand();
    }


    private void StartCombat()
    {
        _deck.Initialize();
        HideInstructions();
        _bottomBar.SetActive(true);
    }

    public void DisplayCardInfo(CardInstance inst)
    {
        _cardInfoParent.SetActive(true);
        _cardInfo.Initialize(inst, inst.Owner.Name);
    }

    private void StartEnemyTurn()
    {
        _ApSliderParent.SetActive(false);
        foreach (var b in _bottomBarBackings) b.SetEnabled(false);
        //SetInstructionsText("Enemy turn");
        //_bottomBar.SetActive(false);
        
    }

    private void StartPlayerTurn()
    {
        foreach (var b in _bottomBarBackings) b.SetEnabled(true);
        _ApSliderParent.SetActive(true);
        _hand.DrawUntilFull();
        HideInstructions();
        _bottomBar.SetActive(true);
    }

    private void EndPlayerTurn() {
        _hand.Discard(-1);
    }

    public void SetInstructionsText(string text)
    {
        _instructionsText.text = text;
        _instructionsParent.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_instructionsParent.GetComponent<RectTransform>());
    }

    public void UpdateActionDisplay()
    {
        _actionsPointsText.text = gMan.Actions + "/" + gMan.MaxActions;
        _actionPointsSlider.value = gMan.Actions / (float) gMan.MaxActions;
    }

    public async void MoveToDisplay(CardObject cardObject)
    {
        cardObject.transform.SetParent(_currentPlayedCardParent);
        await Task.Delay(1);
        if (cardObject == null) return;

        cardObject.transform.localPosition = Vector3.zero;
        cardObject.transform.localScale = Vector3.one;
        cardObject.transform.localEulerAngles = Vector3.zero;
    }

    public void DisplayVictoryScreen() {
        Loot loot = CardGameManager.i.GetLoot();

        if (loot.Equipment) PlayerInfo.Inventory.AddEquipment(loot.Equipment);
        PlayerInfo.Stats.Money += loot.Gold;

        string text = "You Obtained:\n";
        if (loot.Equipment) text += loot.Equipment.Name + "\n";
        if (loot.Gold != 0) text += loot.Gold + " Gold\n";
        
        _victoryScreenTempText.text = text;

        _victoryScreen.SetActive(true);
    }

    public void UpdateTargetingArrow(CardObject cardObject, Vector3 newPos) {
        if (!_currTargetingArrow) _currTargetingArrow = GameObject.Instantiate(_targetingArrowPrefab, transform);

        _currTargetingArrow.transform.position = newPos;
    }

    public void DestroyTargetingArrow() {
        if (!_currTargetingArrow) return;
        
        Destroy(_currTargetingArrow);
    }

    public void OpenDeck(DeckDisplayTypeEnum type) {
        OpenMenus += 1;
        _deckDisplayController.Open(type);
    }

    public void CloseDeck() {
        OpenMenus -= 1;
        _deckDisplayController.Close();
    }
}