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
using Unity.VisualScripting;

public class CardGameManager : GameManager
{
    public new static CardGameManager i;

    [Header("References")]
    [SerializeField] private TabletopController _tableTop;
    [SerializeField] private TriggeredEffectController _triggeredEffectController;
    [SerializeField] private CardGameCameraController _cameraController;

    [Header("Stats")]
    [SerializeField] private int _maxActions = 3; //move to playerInfo.Stats eventually

    [HideInInspector] public List<Creature> SelectedCreatures = new List<Creature>();
    [HideInInspector] public CardObject CurrentPlayedCard;
    [HideInInspector] public UnityEvent OnStartCombat = new UnityEvent();
    [HideInInspector] public UnityEvent OnStartPlayerTurn = new UnityEvent();
    [HideInInspector] public UnityEvent OnEndPlayerTurn = new UnityEvent();
    [HideInInspector] public UnityEvent OnStartEnemyTurn = new UnityEvent();
    [HideInInspector] public UnityEvent OnEndEnemyTurn = new UnityEvent();

    [HideInInspector] public CombatState CurrCombatState { get; private set; }
    [HideInInspector] public int Actions { get; private set; }
    [HideInInspector] public int MaxActions => _maxActions;

    private CardPlayData _currentCardPlayData;

    private const int TURN_WAIT_TIME = 1000;

    private CardGameUIManager ui => CardGameUIManager.i;
    public AdventurerObject GetOwnerAdventurer(CardObject cardObject) => GetOwnerAdventurer(cardObject.CardInstance);
    public AdventurerObject GetOwnerAdventurer(CardInstance inst) => _tableTop.GetAdventurerObject(inst.Owner);
    public AdventurerObject GetAdventurerObject(AdventurerData ownerData) => _tableTop.GetAdventurerObject(ownerData);
    public AdventurerData GetAdventurerData(AdventurerObject adventurerObject) => _tableTop.GetAdventurerData(adventurerObject);
    public List<AdventurerObject> GetAdventurers() => _tableTop.GetAdventurers();
    public List<EnemyObject> GetEnemies() => _tableTop.GetEnemies();
    public void AddTriggeredEffect(TriggeredEffectData triggeredEffect) => _triggeredEffectController.AddTriggeredEffect(triggeredEffect);
    public void ToggleCamera() => _cameraController.Toggle();
    public void StartWiggle(AdventurerData aData) => _tableTop.StartWiggle(aData);
    public void StopWiggle(AdventurerData aData) => _tableTop.StopWiggle(aData);

    protected override void Awake()
    {
        base.Awake();
        i = this;
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
        SceneManager.UnloadSceneAsync(2);

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

    public void StartPlayerTurn()
    {
        CurrCombatState = CombatState.PlayerTurn;
        Actions = _maxActions;
        OnStartPlayerTurn.Invoke();
    }

    public void EndPlayerTurn()
    {
        CurrCombatState = CombatState.Idle;

        OnEndPlayerTurn.Invoke();
        StartEnemyTurn();
    }

    public async void StartEnemyTurn()
    {
        CurrCombatState = CombatState.EnemyTurn;

        OnStartEnemyTurn.Invoke();

        await Task.Delay(TURN_WAIT_TIME);

        await _tableTop.TakeEnemyActions(TURN_WAIT_TIME);

        OnEndEnemyTurn.Invoke();

        CurrCombatState = CombatState.Idle;

        StartPlayerTurn();
    }

    public void PlayCard(CardObject cardObject)
    {

        CurrentPlayedCard = cardObject;
        var data = cardObject.CardInstance;
        var playData = data.GetPlayData(GetOwnerAdventurer(cardObject));
        _currentCardPlayData = playData;
        //_tableTop.StopAllWiggles();

        StartSelectingTargets(playData);

        //CardPlayFunction_Async(cardObject, playData);
    }

    private void StartSelectingTargets(CardPlayData playData)
    {
        if (playData.TargetTypes.Count > 0) _tableTop.StartSelectingTargets(playData.TargetTypes);
        else DoCurrentCardFunction(new List<Creature>());
    }

    public async void DoCurrentCardFunction(List<Creature> targets)
    {
        foreach (var f in _currentCardPlayData.CardFunctionData) await EvaluateFunction(f, targets);

        CardEndsPlay(CurrentPlayedCard);
    }

    private async Task EvaluateFunction(CardFunctionData function, List<Creature> targets)
    {
        var playData = _currentCardPlayData;
        if (!playData.Owner) throw new Exception("card does not have an owner!");

        if (function.Function == Function.ATTACK) {
            await Utilities.LerpToAndBack(playData.Owner.gameObject, targets[0].transform.position);
            targets[0].TakeDamage(function.Amount);
        }
        else if (function.Function == Function.BLOCK) {
            playData.Owner.AddBlock(function.Amount);
        }
        else if (function.Function == Function.DRAW) {
            ui.Draw((int)function.Amount);
        }
        else if (function.Function == Function.HEAL) {
            targets[0].RestoreHealth((int)function.Amount);
        }
        else if (function.Function == Function.ADDCARDS) {
            CardInstance newInst = new CardInstance(function.CardData, GetAdventurerData(playData.Owner));
            ui.AddToDeck(newInst, count: (int)function.Amount);
        }
        else if (function.Function == Function.STATUS) {
            targets[0].AddStatusEffect(function.StatusEffectData);
        }
        else if (function.Function == Function.TRIGGEREDEFFECT) {
            AddTriggeredEffect(function.TriggeredEffectData);
        }
    }

    public void CardEndsPlay(CardObject cardObject)
    {
        CurrentPlayedCard = null;

        DecrementActionPoints();

        ui.HideInstructions();

        if (Actions == 0) {
            EndPlayerTurn();
        }
        else {
            CurrCombatState = CombatState.PlayerTurn;
        }

        CardGameUIManager.i.AddToDiscardPile(cardObject.CardInstance);
        Destroy(cardObject.gameObject);
    }

    public void MoveCardFromDisplay()
    {
        ui.MoveCardFromDisplay(CurrentPlayedCard);

        CurrentPlayedCard = null;
    }
}

public enum CombatState {
    PlayerTurn,
    EnemyTurn,
    Idle, //in between states
}