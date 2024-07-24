using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardGameUIManager : UIManager
{
    public new static CardGameUIManager i;
    protected override void Awake() { base.Awake(); i = this; }

    [Header("Submenus")]
    [SerializeField] private Deck _deck;
    [SerializeField] private Hand _hand;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _instructionsText;
    [SerializeField] private GameObject _instructionsParent;
    [SerializeField] private TMP_Text _actionsPointsText;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;
    [SerializeField] private Transform _currentPlayedCardParent;
    [SerializeField] private GameObject _bottomBar;
    [SerializeField] private GameObject _cardInfoParent;
    [SerializeField] private PlayableCardDisplay _cardInfo;

    private CardGameManager gMan => CardGameManager.i;
    
    public void Draw(int count = 1) => _deck.Draw(count: count);
    public void AddToDiscardPile(CardInstance inst, int count = 1) => _deck.AddToDiscard(inst, count: count);
    public void AddToDeck(CardInstance inst, int count = 1, bool random = true) => _deck.AddToDeck(inst, count: count, random: random);
    public void HideInstructions() => _instructionsParent.SetActive(false);
    public void DisplayVictoryScreen() => _victoryScreen.SetActive(true);
    public void DisplayDefeatScreen() => _defeatScreen.SetActive(true);
    public void MoveCardFromDisplay(CardObject cardObj)
    {
        HideInstructions();
        cardObj.ReturnToHand();
    }

    private void Start()
    {
        gMan.OnStartCombat.AddListener(StartCombat);
        gMan.OnStartPlayerTurn.AddListener(StartPlayerTurn);
        gMan.OnStartEnemyTurn.AddListener(StartEnemyTurn);
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
        _cardInfo.Initialize(inst);
    }

    private void StartEnemyTurn()
    {
        SetInstructionsText("Enemy turn");
        _bottomBar.SetActive(false);
    }

    private void StartPlayerTurn()
    {
        _hand.DrawUntilFull();
        HideInstructions();
        _bottomBar.SetActive(true);
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
    }

    public void MoveToDisplay(CardObject cardObject)
    {
        cardObject.transform.SetParent(_currentPlayedCardParent);
        cardObject.transform.localPosition = Vector3.zero;
        cardObject.transform.localScale = Vector3.one;
    }
}
