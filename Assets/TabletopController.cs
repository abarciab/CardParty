using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TabletopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _adventurerParent;
    [SerializeField] private Transform _enemyParent;

    [Header("Slot spacing")]
    [SerializeField] private Transform _leftAdventurerLimit;
    [SerializeField] private Transform _rightAdventurerLimit;
    [SerializeField] private Transform _leftEnemyLimit;
    [SerializeField] private Transform _rightEnemyLimit;

    [Header("Prefabs")]
    [SerializeField] private GameObject _combatSlotPrefab;

    [SerializeField, ReadOnly] private List<AdventurerObject> _adventurerObjs = new List<AdventurerObject>();
    [SerializeField, ReadOnly] private List<EnemyObject> _enemyObjs = new List<EnemyObject>();
    private List<CombatSlot> _adventurerCombatSlots = new List<CombatSlot>();
    private List<CombatSlot> _enemyCombatSlots = new List<CombatSlot>();
    [SerializeField, ReadOnly] private List<CombatSlot> _blockCombatSlots = new List<CombatSlot>();

    private const int MAX_PARTY_SIZE = 3;

    private void Start()
    {
        CardGameManager.i.OnStartPlayerTurn.AddListener(StartPlayerTurn);
    }

    public CombatSlot SpawnBlockSlot(Vector3 position)
    {
        foreach (var blockSlot in _blockCombatSlots) {
            var dist = Vector3.Distance(blockSlot.transform.position, position);
            if (dist < 0.1f) {
                print("found overlap");
                return blockSlot;
            }
        }

        GameObject newBlocKSlot = Instantiate(_combatSlotPrefab, _enemyParent);
        newBlocKSlot.transform.position = position;
        newBlocKSlot.GetComponent<CombatSlot>().IsBlockSlot = true;
        _blockCombatSlots.Add(newBlocKSlot.GetComponent<CombatSlot>());

        return newBlocKSlot.GetComponent<CombatSlot>();
    }

    public void ClearBlockSlot(CombatSlot blockSlot)
    {
        if (!blockSlot.Creature) return;
        var emptyslot = GetRandomEmptyAdventurerSlot();
        emptyslot.SetCreature(blockSlot.Creature);
    }

    public void UpdateAttackArrows(CombatSlot blockSlot)
    {
        if (blockSlot.Creature) {
            blockSlot.AttackArrow.Initialize(blockSlot.AttackArrow.Owner.transform.position, blockSlot.transform.position);
        }
        else {
            blockSlot.AttackArrow.Initialize(blockSlot.AttackArrow.Owner.transform.position, blockSlot.AttackArrow.Owner.GetTarget().transform.position);
        }
    }

    public async Task TakeEnemyActions(int TURN_WAIT_TIME)
    {
        foreach (EnemyObject enemy in _enemyObjs) {
            await enemy.Action(_adventurerObjs, _enemyObjs);
            await Task.Delay(TURN_WAIT_TIME);
        }
    }

    private void StartPlayerTurn()
    {
        foreach (var e in _enemyObjs) e.ShowIntent();
    }

    public void SpawnCombatants(Combat combat)
    {
        for (int i = 0; i < MAX_PARTY_SIZE; i++) {
            MakeNewAdventurerSlot();
            MakeNewEnemySlot();
        }

        foreach (var a in PlayerInfo.Party.Adventurers) AddAdventurerToCombat(a);
        foreach (var e in combat.enemies) AddEnemyToCombat(e);
    }

    private void AddAdventurerToCombat(AdventurerData data)
    {
        var emptySlots = _adventurerCombatSlots.Where(x => !x.Creature).ToList();
        var slot = emptySlots[0];

        AddCreatureToSlot(slot, data.AdventurerPrefab, ref _adventurerObjs);
        slot.Creature.GetComponent<AdventurerObject>().Initialize(data);
    }

    private void AddEnemyToCombat(GameObject enemyPrefab)
    {
        var emptySlots = _enemyCombatSlots.Where(x => !x.Creature).ToList();
        var slot = emptySlots[Mathf.RoundToInt((emptySlots.Count-1) / 2)];

        AddCreatureToSlot(slot, enemyPrefab, ref _enemyObjs);
    }

    private void AddCreatureToSlot<t>(CombatSlot slot, GameObject prefab, ref List<t> list)
    {
        slot.InitializeWithCreature(prefab, this);
        var creature = slot.Creature.GetComponent<t>();
        list.Add(creature);
    }

    private void MakeNewAdventurerSlot() => MakeNewSlot(_adventurerParent, ref _adventurerCombatSlots, _leftAdventurerLimit, _rightAdventurerLimit);
    private void MakeNewEnemySlot()
    {
        var slot = MakeNewSlot(_enemyParent, ref _enemyCombatSlots, _leftEnemyLimit, _rightEnemyLimit);
        slot.HideVisuals();
    }


    private CombatSlot MakeNewSlot(Transform parent, ref List<CombatSlot> slots, Transform leftLimit, Transform rightLimit)
    {
        var newCombatSlot = Instantiate(_combatSlotPrefab, parent).GetComponent<CombatSlot>();
        slots.Add(newCombatSlot);
        RearrangeList(slots, leftLimit.position, rightLimit.position);
        return newCombatSlot;
    }

    private void RearrangeList(List<CombatSlot> slots, Vector3 leftPos, Vector3 rightPos)
    {
        float step = 1f / (slots.Count + 1);
        for (int i = 0; i < slots.Count; i++) {
            float progress = (i+1) * step;
            var pos = Vector3.Lerp(leftPos, rightPos, progress);
            slots[i].transform.position = pos;
        }
    }

    public AdventurerObject GetAdventurerObject(AdventurerData adventurerData)
    {
        foreach (AdventurerObject adventurer in _adventurerObjs) {
            if (adventurer.AdventurerData == adventurerData) return adventurer;
        }
        print("didn't find adventurer for data: " + adventurerData);
        return null;
    }

    public CombatSlot GetValidAttackTarget(CombatSlot _enemyPos)
    {
        var validSlots = new List<CombatSlot>();
        for (int i = 0; i < _enemyCombatSlots.Count; i++) {
            if (_enemyCombatSlots[i] != _enemyPos) continue;

            validSlots.Add(_adventurerCombatSlots[i]);
            if (i > 0) validSlots.Add(_adventurerCombatSlots[i - 1]);
            if (i < _adventurerCombatSlots.Count - 1) validSlots.Add(_adventurerCombatSlots[i + 1]);
        }
        validSlots = validSlots.Where(x => x.Creature).ToList();

        return validSlots[Random.Range(0, validSlots.Count)];
    }

    public CombatSlot GetRandomEmptyAdventurerSlot()
    {
        List<CombatSlot> adventurerCombatSlots = _adventurerCombatSlots.Where(x => !x.Creature).ToList();
        return adventurerCombatSlots[Random.Range(0, adventurerCombatSlots.Count)];
    }

    public void RemoveCreature(Creature creature)
    {
        if (creature.GetType() == typeof(EnemyObject)) {
            _enemyObjs.Remove((EnemyObject)creature);
            if (_enemyObjs.Count == 0) {
                CardGameUIManager.i.DisplayVictoryScreen();
            }
        }
        else if (creature.GetType() == typeof(AdventurerObject)) {
            _adventurerObjs.Remove((AdventurerObject)creature);
            if (_adventurerObjs.Count == 0) {
                CardGameUIManager.i.DisplayDefeatScreen();
            }
        }
    }
}
