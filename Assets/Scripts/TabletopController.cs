using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    private List<Creature> _selectedCreatures = new List<Creature>();
    private List<System.Type> _targetSelectedCreatures = new List<System.Type>();

    public void StopWiggle(AdventurerData aData) => GetAdventurerObject(aData).SetWiggle(false);

    private void Start()
    {
        CardGameManager.i.OnStartPlayerTurn.AddListener(StartPlayerTurn);
    }

    public void StartWiggle(AdventurerData aData)
    {
        StopAllWiggles();
        GetAdventurerObject(aData).SetWiggle(true);
    }

    public void StopAllWiggles()
    {
        foreach (var a in _adventurerObjs) a.SetWiggle(false);
    }

    public void StartSelectingTargets(List<System.Type> types)
    {
        _targetSelectedCreatures = types.OrderBy(x => x.Name).ToList();
        _selectedCreatures.Clear();

        var validCreatures = new List<Creature>();

        if (types.Contains(typeof(AdventurerObject))) validCreatures.AddRange(_adventurerObjs);
        if (types.Contains(typeof(EnemyObject))) validCreatures.AddRange(_enemyObjs);

        foreach (var creature in validCreatures) creature.MakeSelectable();
    }

    public void AddToSelectedTargets(Creature selected)
    {        
        if (_selectedCreatures.Contains(selected)) return;

        _selectedCreatures.Add(selected);

        var types = _selectedCreatures.Select(x => x.GetType()).OrderBy(x => x.Name).ToList();
        if (AreListsEqual(types, _targetSelectedCreatures)) {
            MakeAllCreaturesUnselectable();
            CardGameManager.i.DoCurrentCardFunction(_selectedCreatures);
        }
    }

    private List<Creature> GetAllCreaturesList()
    {
        var allCreatures = new List<Creature>(_enemyObjs);
        allCreatures.AddRange(_adventurerObjs);
        return allCreatures;
    }

    private void MakeAllCreaturesUnselectable()
    {
        var allCreatures = GetAllCreaturesList();
        foreach (var c in allCreatures) c.MakeUnselectable();
    }

    private bool AreListsEqual(List<System.Type> list1, List<System.Type> list2)
    {
        for (int i = 0; i < list1.Count; i++) {
            if (list1[i] != list2[i]) return false;
        }
        return true;
    }

    public CombatSlot SpawnBlockSlot(Vector3 position, EnemyObject enemy)
    {
        var existing = ExistingBlockSlotInPosition(position);
        if (existing) return existing;

        var slot = Instantiate(_combatSlotPrefab, position, Quaternion.identity, _enemyParent).GetComponent<CombatSlot>();
        slot.Initialize(this, enemy);
        _blockCombatSlots.Add(slot);

        return slot;
    }

    private CombatSlot ExistingBlockSlotInPosition(Vector3 pos)
    {
        _blockCombatSlots = _blockCombatSlots.Where(x => x != null).ToList();
        foreach (var blockSlot in _blockCombatSlots) {
            var dist = Vector3.Distance(blockSlot.transform.position, pos);
            if (dist < 0.1f) return blockSlot;
        }
        return null;
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

    public void RemoveAttackArrow(AttackArrow arrow) {
        if (arrow == null) return;
        _blockCombatSlots.Remove(arrow.BlockSlot);
        ClearBlockSlot(arrow.BlockSlot);
        if (arrow.BlockSlot) Destroy(arrow.BlockSlot.gameObject);
        Destroy(arrow.gameObject);
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
        var slot = emptySlots[Mathf.RoundToInt((emptySlots.Count - 1) / 2)];

        AddCreatureToSlot(slot, data.AdventurerPrefab, ref _adventurerObjs);
        slot.Creature.GetComponent<AdventurerObject>().Initialize(data);
    }

    private void AddEnemyToCombat(EnemyData enemyData)
    {
        var emptySlots = _enemyCombatSlots.Where(x => !x.Creature).ToList();
        var slot = emptySlots[Mathf.RoundToInt((emptySlots.Count-1) / 2)];

        AddCreatureToSlot(slot, enemyData.Prefab, ref _enemyObjs);
        slot.Creature.GetComponent<EnemyObject>().Initialize(enemyData);
    }

    private void AddCreatureToSlot<t>(CombatSlot slot, GameObject prefab, ref List<t> list)
    {
        slot.Initialize(prefab, this);
        var creature = slot.Creature.GetComponent<t>();
        list.Add(creature);
    }

    private void MakeNewAdventurerSlot()
    {
        var slot = MakeNewSlot(_adventurerParent, ref _adventurerCombatSlots, _leftAdventurerLimit, _rightAdventurerLimit);
        slot.gameObject.name = "adventurer slot";

    }
    private void MakeNewEnemySlot()
    {
        var slot = MakeNewSlot(_enemyParent, ref _enemyCombatSlots, _leftEnemyLimit, _rightEnemyLimit);
        slot.gameObject.name = "enemy slot";
        slot.HideVisuals();
    }


    private CombatSlot MakeNewSlot(Transform parent, ref List<CombatSlot> slots, Transform leftLimit, Transform rightLimit)
    {
        var newSlot = Instantiate(_combatSlotPrefab, parent).GetComponent<CombatSlot>();
        newSlot.Initialize(this);
        slots.Add(newSlot);
        RearrangeList(slots, leftLimit.position, rightLimit.position);
        return newSlot;
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
        if (!adventurerData) throw new System.Exception("adventurerData is null");

        foreach (AdventurerObject adventurer in _adventurerObjs) {
            if (adventurer.AdventurerData == adventurerData) return adventurer;
        }
        throw new System.Exception("didn't find AdventurerObject for AdventurerData " + adventurerData);
    }

    public AdventurerData GetAdventurerData(AdventurerObject adventurerObject) {
        if (!adventurerObject) throw new System.Exception("adventurerObject is null");

        foreach (AdventurerObject adventurer in _adventurerObjs) {
            if (adventurer == adventurerObject) return adventurer.AdventurerData;
        }
        throw new System.Exception("didn't find AdventurerData for AdventurerObject " + adventurerObject);

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

        if (validSlots.Count == 0) return null;
        return validSlots[Random.Range(0, validSlots.Count)];
    }

    public CombatSlot GetRandomEmptyAdventurerSlot()
    {
        List<CombatSlot> adventurerCombatSlots = _adventurerCombatSlots.Where(x => !x.Creature).ToList();
        return adventurerCombatSlots[Random.Range(0, adventurerCombatSlots.Count)];
    }

    public List<AdventurerObject> GetAdventurers() {
        return _adventurerObjs;
    }

    public List<EnemyObject> GetEnemies() {
        return _enemyObjs;
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
