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

    [SerializeField] private List<EquipmentData> _equipmentData;

    [HideInInspector] public List<CreatureObject> SelectedCreatures = new List<CreatureObject>();
    [HideInInspector] public CardObject CurrentPlayedCard;
    [HideInInspector] public UnityEvent OnStartCombat = new UnityEvent();
    [HideInInspector] public UnityEvent OnStartPlayerTurn = new UnityEvent();
    [HideInInspector] public UnityEvent OnEndPlayerTurn = new UnityEvent();
    [HideInInspector] public UnityEvent OnStartEnemyTurn = new UnityEvent();
    [HideInInspector] public UnityEvent OnEndEnemyTurn = new UnityEvent();

    [HideInInspector] public CombatState CurrCombatState { get; private set; }
    [HideInInspector] public int Actions { get; private set; }
    [HideInInspector] public int MaxActions => _maxActions;

    [SerializeField] private CardPlayData _currentCardPlayData;
    private CombatData _currCombatData;

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
    public void PlayIfValidTargets() => _tableTop.PlayIfValidTargets();

    protected override void Awake()
    {
        base.Awake();
        i = this;
    }

    public void ChangeActionNum(int actionDelta)
    {
        Actions = Mathf.Max(0, Actions + actionDelta);
        CardGameUIManager.i.UpdateActionDisplay();
    }

    public (EquipmentData, int) GetLoot(CombatData combat = null) {
        if (combat == null) combat = _currCombatData;

        (EquipmentData, int) res = (null, 0);
        float r = UnityEngine.Random.Range(0, 1);
        if (r < 0.1) {
            EquipmentRarity rarity = EquipmentRarity.Common;
            r = UnityEngine.Random.Range(0, 1);
            if (combat.Difficulty < 5) {
                if (r < 0.3) rarity = EquipmentRarity.Uncommon;
                else rarity = EquipmentRarity.Common;
            } else if (combat.Difficulty <= 9) {
                if (r < 0.1) rarity = EquipmentRarity.Rare;
                else if (r < 0.4) rarity = EquipmentRarity.Uncommon;
                else rarity = EquipmentRarity.Common;
            } else if (combat.Difficulty <= 12) {
                if (r < 0.2) rarity = EquipmentRarity.Rare;
                else if (r < 0.5) rarity = EquipmentRarity.Uncommon;
                else rarity = EquipmentRarity.Common;
            }
            foreach(EquipmentData equipment in _equipmentData.Shuffle()) {
                if (equipment.Rarity == rarity) {
                    res.Item1 = equipment;
                    break;
                }
            }
        } else {
            res.Item2 = 22 + UnityEngine.Random.Range(-5, 5);
        }
        return res;
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

        CardGameManager.i = null;
    }

    public async void StartCombat(CombatData combat)
    {
        _currCombatData = combat;

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

        _tableTop.RemoveEnemyBlock();

        StartEnemyTurn();
    }

    public async void StartEnemyTurn()
    {
        CurrCombatState = CombatState.EnemyTurn;

        OnStartEnemyTurn.Invoke();

        await Task.Delay(TURN_WAIT_TIME);

        await _tableTop.TakeEnemyActions(TURN_WAIT_TIME);

        _tableTop.RemoveAdventurerBlock();
        
        OnEndEnemyTurn.Invoke();

        CurrCombatState = CombatState.Idle;
        
        StartPlayerTurn();
    }

    public void UpdateCurrPlayedCardData(CardObject cardObject)
    {
        if (Actions == 1 && cardObject.CardInstance.CardData.Cost > 0) {
            ui.StopPlayingCards();
        }

        CurrentPlayedCard = cardObject;
        var data = cardObject.CardInstance;
        var playData = data.GetPlayData(GetOwnerAdventurer(cardObject));
        _currentCardPlayData = playData;
        //_tableTop.StopAllWiggles();

        //CardPlayFunction_Async(cardObject, playData);
    }

    public void PlayCard(CardObject cardObject) {
        DoCurrentCardFunction(new List<CreatureObject>());
    }

    public void StartSelectingTargets(CardPlayData playData)
    {
        if (playData.TargetTypes.Count > 0) _tableTop.StartSelectingTargets(playData.TargetTypes);
    }

    public async void DoCurrentCardFunction(List<CreatureObject> targets)
    {
        foreach (var f in _currentCardPlayData.CardFunctionData) await EvaluateFunction(f, targets);

        CardEndsPlay(CurrentPlayedCard);
    }

    private async Task EvaluateFunction(CardFunctionData function, List<CreatureObject> targets)
    {
        var playData = _currentCardPlayData;
        if (!playData.Owner) throw new Exception("card does not have an owner!");

        CreatureObject currTarget;
        if (function.TargetSelf) {
            currTarget = playData.Owner;
        } else if (targets.Count > 0) {
            currTarget = targets[0];
        } else currTarget = null;

        string ownerName = playData.Owner.GetName();
        string targetName = function.TargetSelf || currTarget == null ? "" : currTarget.GetName();

        float amount = function.Amount;
        Function funct = function.Function;
        bool doesAttack = function.Function == Function.ATTACK || function.Function == Function.THEVESSEL;

        int intAmount = (int)amount;
        int attackDamage = intAmount + playData.Owner.GetBonusDamage();
        int blockAmount = funct == Function.ARCHMAGEPROT ? 2 * CardGameUIManager.i.GetHandSize() : intAmount;


        if (doesAttack) {
            ui.LogMove(ownerName + " attacked " + targetName + " for " + attackDamage + " damage");
            await Utilities.LerpToAndBack(playData.Owner.gameObject, currTarget.transform.position);
            currTarget.TakeDamage(attackDamage);

            if (funct == Function.THEVESSEL && currTarget.IsDead) {
                foreach (AdventurerObject adventurer in GetAdventurers()) {
                    adventurer.AddStatusEffect(function.StatusEffectData);
                }
            }
        }
        if (funct == Function.BLOCK) {
            ui.LogMove(ownerName + " gained " + blockAmount + " block");
            playData.Owner.AddBlock(blockAmount);
        }
        if (funct == Function.DRAW) {
            ui.LogMove(ownerName + " drew " + intAmount + (intAmount > 1 ? " cards" : "card"));
            ui.Draw(intAmount);
        }
        if (funct == Function.HEAL) {
            ui.LogMove(ownerName + " healed " + (string.IsNullOrEmpty(targetName) ? "" : targetName) + " for " + intAmount + " health");
            currTarget.RestoreHealth(intAmount);
        }
        if (funct == Function.ADDCARDS) {
            ui.LogMove(ownerName + " added " + function.CardData.Name + " to the deck");
            CardInstance newInst = new CardInstance(function.CardData, GetAdventurerData(playData.Owner));
            ui.AddToDeck(newInst, count: (int)function.Amount);
        }
        if (funct == Function.STATUS) {
            ui.LogMove(ownerName + " inflicted " + function.StatusEffectData.Name + " on " + currTarget);
            currTarget.AddStatusEffect(function.StatusEffectData);
        }
        if (funct == Function.REMOVESTATUS) {
            ui.LogMove(ownerName + "removed all effects from " + targetName);
            targets[0].RemoveAllStatusEffects();
        }

        if (funct == Function.TRIGGEREDEFFECT) {
            AddTriggeredEffect(function.TriggeredEffectData);
        }
        if (funct == Function.WHEEL) {
            int handSize = CardGameUIManager.i.GetHandSize();
            CardGameUIManager.i.Discard(handSize);
            CardGameUIManager.i.Draw(handSize);
        }
    }

    public void CardEndsPlay(CardObject cardObject)
    {
        CurrentPlayedCard = null;

        ChangeActionNum(cardObject.CardInstance.CardData.Cost);

        ui.HideInstructions();
        CurrCombatState = CombatState.PlayerTurn;

        CardGameUIManager.i.AddToDiscardPile(cardObject.CardInstance);
        Destroy(cardObject.gameObject);
    }

    public void MoveCardFromDisplay()
    {
        ui.MoveCardFromDisplay(CurrentPlayedCard);
        CurrentPlayedCard = null;
    }

    public bool IsPlayable(CardObject card) {
        if (!GetOwnerAdventurer(card)) return false;
        return GetOwnerAdventurer(card).CanPlayCards();
    }
}

public enum CombatState {
    PlayerTurn,
    EnemyTurn,
    Idle, //in between states
}