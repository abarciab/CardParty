using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using MyBox;

public class CardGameManager : GameManager
{
    public new static CardGameManager i;

    [Header("References")]
    [SerializeField] private TabletopController _tableTop;

    [Space()]
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private Transform _adventurerContainer;
    [SerializeField] private GameObject _selectedCreatureHighlight;
    [SerializeField] private GameObject _combatSlotPrefab;

    [HideInInspector] public List<Creature> SelectedCreatures = new List<Creature>();
    [HideInInspector] public CardObject CurrPlayedCard;
    [HideInInspector] public UnityEvent OnStartCombat;
    [HideInInspector] public UnityEvent OnStartEnemyTurn;
    [HideInInspector] public UnityEvent OnStartPlayerTurn;

    [HideInInspector] public CombatState CurrCombatState { get; private set; }
    [HideInInspector]public int Actions { get; private set; }
    [SerializeField] private int _maxActions = 3;
    [HideInInspector] public int MaxActions => _maxActions;

    private List<System.Type> _currValidSelectTargets = new List<System.Type>();
    private System.Random _r = new System.Random();
    //private List<Adventurer> _adventurers = new List<Adventurer>();
    //private List<Enemy> _enemies = new List<Enemy>();
    //private List<CombatSlot> _adventurerCombatSlots = new List<CombatSlot>();
    //private List<CombatSlot> _enemyCombatSlots = new List<CombatSlot>();
    //private List<CombatSlot> _blockCombatSlots = new List<CombatSlot>();

    private const int TURN_WAIT_TIME = 1000;
    private const float CREATURE_SPACING = 10f;

    private CardGameUIManager ui => CardGameUIManager.i;

    protected override void Awake() {
        base.Awake();
        i = this;
    }

    protected override void Update() {
        base.Update();
        TryToSelectTargets();
    }

    private void TryToSelectTargets()
    {
        if (!CurrPlayedCard || !Input.GetMouseButtonDown(0)) return;
        RaycastHit[] hits = Physics.RaycastAll(Camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), 50);

        foreach (RaycastHit hit in hits) {
            Creature creature = hit.collider.gameObject.GetComponent<Creature>();
            if (!creature) continue;

            if (SelectedCreatures.Contains(creature)) creature.Deselect();
            else creature.Select();
        }
    }

    private void DecrementActionPoints() => ChangeActionNum(-1);

    public void ChangeActionNum(int actionDelta)
    {
        Actions = Mathf.Max(0, Actions + actionDelta);
        CardGameUIManager.i.UpdateActionDisplay();
    }

    public void LoadOverworld()
    {
        Resume();
        FadeThenShowOverworld();
    }

    private async void FadeThenShowOverworld()
    {
        float fadeTime = UIManager.i.GetFadeTime();
        Music.FadeOutCurrent(fadeTime);
        await Task.Delay((int)(1000 * fadeTime));
        Camera.GetComponent<AudioListener>().enabled = false;
        var unloadingTask = SceneManager.UnloadSceneAsync(2);

        if (OverworldManager.i) OverworldManager.i.ShowOverworldObjects();
        else SceneManager.LoadScene(1);
    }
    
    public async void StartCombat(Combat combat)
    {
        _tableTop.SpawnCombatants(combat);

        OnStartCombat.Invoke();
        await Task.Delay(Mathf.RoundToInt(TURN_WAIT_TIME / 2));

        StartPlayerTurn();
    }

    public void StartPlayerTurn() {
        CurrCombatState = CombatState.PlayerTurn;
        Actions = _maxActions;
        OnStartPlayerTurn.Invoke();

        
    }

    public void EndPlayerTurn() {
        CurrCombatState = CombatState.Idle;

        StartEnemyTurn();
    }

    public async void StartEnemyTurn() {
        CurrCombatState = CombatState.EnemyTurn;

        OnStartEnemyTurn.Invoke();

        await Task.Delay(TURN_WAIT_TIME);

        await _tableTop.TakeEnemyActions(TURN_WAIT_TIME);

        CurrCombatState = CombatState.Idle;

        StartPlayerTurn();
    }

    public void PlayCard(CardObject cardObject)
    {
        CurrPlayedCard = cardObject;
        var data = cardObject.CardData;
        var playData = data.GetPlayData(GetOwnerAdventurer(cardObject));

        CardPlayFunction_Async(cardObject, playData);
    }

    public async void CardPlayFunction_Async(CardObject cardObject, CardPlayData cardPlayData) {
        foreach (CardFunctionData cardFunctionData in cardPlayData.CardFunctionData) {

            List<System.Type> requiredTargets = cardFunctionData.Targets;
            List<Creature> selectedTargets = await SelectTargets(requiredTargets);

            if (cardFunctionData.Function == Function.ATTACK) {
                await Utilities.LerpToAndBack(cardPlayData.Owner.gameObject, selectedTargets[0].transform.position);
                selectedTargets[0].TakeDamage(cardFunctionData.Amount);
            }
            else if (cardFunctionData.Function == Function.BLOCK) {
                cardPlayData.Owner.AddBlock(cardFunctionData.Amount);
            }

            //DoSpecial();
        }

        CardEndsPlay(cardObject);
    }

    public void CardEndsPlay(CardObject cardObject) {
        CurrPlayedCard = null;
        
        DeselectAllCreatures();
        DecrementActionPoints();

        ui.HideInstructions();

        if (Actions == 0) {
            EndPlayerTurn();
        } else {
            CurrCombatState = CombatState.PlayerTurn;
        }

        CardGameUIManager.i.AddToDiscardPile(cardObject.CardData);
        Destroy(cardObject.gameObject);
    }

    public void MoveCardFromDisplay() {
        ui.MoveCardFromDisplay(CurrPlayedCard);

        CurrPlayedCard = null;
    }

    public async Task<List<Creature>> SelectTargets(List<System.Type> requiredTargets) {
        ui.SetInstructionsText("Select Targets");

        DeselectAllCreatures();
        _currValidSelectTargets = requiredTargets;
        while(!CompareListsByType<Creature>(requiredTargets, SelectedCreatures)) await Task.Delay(1);
        
        return SelectedCreatures;
    }

    bool CompareListsByType<T>(List<System.Type> listA, List<T> listB) {
        foreach (System.Type type in listA.Distinct().ToList()) {
            if (listA.Count(x => x == type) != listB.Count(x => x.GetType() == type)) return false;
        }
        return true;
    }

    public void SelectCreature(Creature creature) {
        if (_currValidSelectTargets.Count == 0) return;

        // if you've already selected enough creatures of this type, deselect the oldest
        if (SelectedCreatures.Count > 0 && SelectedCreatures.Count(x => x.GetType() == creature.GetType()) >= _currValidSelectTargets.Count(x => x.GetType() == creature.GetType())) {
            DeselectCreature(SelectedCreatures.FirstOrDefault(x => x.GetType() == creature.GetType()));
        }

        SelectedCreatures.Add(creature);
        
        creature.SelectedCreatureHighlight = Instantiate(_selectedCreatureHighlight, creature.Canvas.gameObject.transform);
    }
    
    public void DeselectCreature(Creature creature) {
        SelectedCreatures.Remove(creature);

        Destroy(creature.SelectedCreatureHighlight);
    }

    public void DeselectAllCreatures() {
        while (SelectedCreatures.Count > 0) {
            DeselectCreature(SelectedCreatures[0]);
        }
    }

    public Adventurer GetOwnerAdventurer(CardObject cardObject) => GetOwnerAdventurer(cardObject.CardData);
    public Adventurer GetOwnerAdventurer(CardData cardData) => _tableTop.GetAdventurerObject(cardData.Owner);
    public Adventurer GetAdventurerObject(AdventurerData ownerData) => _tableTop.GetAdventurerObject(ownerData);


    public void UpdateAttackArrow(CombatSlot blockSlot) {       
        if (blockSlot.Creature) {
            blockSlot.AttackArrow.SetArrow(blockSlot.AttackArrow.Owner.transform.position, blockSlot.transform.position);
        } else {
            blockSlot.AttackArrow.SetArrow(blockSlot.AttackArrow.Owner.transform.position, blockSlot.AttackArrow.Owner.GetTarget().transform.position);
        }
    }
}

public enum CombatState {
    PlayerTurn,
    EnemyTurn,
    Idle, //in between states
}