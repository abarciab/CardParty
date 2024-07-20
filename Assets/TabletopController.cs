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

    [SerializeField, ReadOnly] private List<Adventurer> _adventurerObjs = new List<Adventurer>();
    [SerializeField, ReadOnly] private List<Enemy> _enemyObjs = new List<Enemy>();
    private List<CombatSlot> _adventurerCombatSlots = new List<CombatSlot>();
    private List<CombatSlot> _enemyCombatSlots = new List<CombatSlot>();
    private List<CombatSlot> _blockCombatSlots = new List<CombatSlot>();

    private void Start()
    {
        CardGameManager.i.OnStartPlayerTurn.AddListener(StartPlayerTurn);
    }

    public CombatSlot SpawnBlockSlot(Vector3 position)
    {
        GameObject newCombatSlot = Instantiate(_combatSlotPrefab, _enemyParent);
        newCombatSlot.transform.position = position;
        newCombatSlot.GetComponent<CombatSlot>().IsBlockSlot = true;
        _blockCombatSlots.Add(newCombatSlot.GetComponent<CombatSlot>());

        return newCombatSlot.GetComponent<CombatSlot>();
    }

    public async Task TakeEnemyActions(int TURN_WAIT_TIME)
    {
        foreach (Enemy enemy in _enemyObjs) {
            await enemy.Action(_adventurerObjs, _enemyObjs);
            await Task.Delay(TURN_WAIT_TIME);
        }
    }

    private void StartPlayerTurn()
    {
        foreach (var e in _enemyObjs) e.ShowIntent(_adventurerObjs, _enemyObjs);
    }

    public void SpawnCombatants(Combat combat)
    {
        foreach (var a in PlayerInfo.Party.Adventurers) AddAdventurerToCombat(a);
        foreach (var e in combat.enemies) AddEnemyToCombat(e);
    }

    private void AddAdventurerToCombat(AdventurerData data)
    {
        var newSlot = MakeNewSlot(_adventurerParent, ref _adventurerCombatSlots, _leftAdventurerLimit, _rightAdventurerLimit);
        AddCreatureToSlot(newSlot, data.AdventurerPrefab, ref _adventurerObjs);
        newSlot.Creature.GetComponent<Adventurer>().Initialize(data);
    }

    private void AddEnemyToCombat(GameObject enemyPrefab)
    {
        var newSlot = MakeNewSlot(_enemyParent, ref _enemyCombatSlots, _leftEnemyLimit, _rightEnemyLimit);
        AddCreatureToSlot(newSlot, enemyPrefab, ref _enemyObjs);
    }

    private void AddCreatureToSlot<t>(CombatSlot slot, GameObject prefab, ref List<t> list)
    {
        slot.InitializeWithCreature(prefab, this);
        var creature = slot.Creature.GetComponent<t>();
        list.Add(creature);
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

    public Adventurer GetAdventurerObject(AdventurerData adventurerData)
    {
        foreach (Adventurer adventurer in _adventurerObjs) {
            if (adventurer.AdventurerData == adventurerData) return adventurer;
        }
        print("didn't find adventurer for data: " + adventurerData);
        return null;
    }

    public CombatSlot GetRandomAdventurerSlot(bool empty = false)
    {
        List<CombatSlot> adventurerCombatSlots = _adventurerCombatSlots.Shuffle().ToList();
        if (empty) {
            foreach (CombatSlot combatSlot in adventurerCombatSlots) {
                if (combatSlot && !combatSlot.Creature) return combatSlot;
            }
            return null;
        }
        return adventurerCombatSlots[0];
    }

    public List<Adventurer> GetAdventurers() {
        return _adventurerObjs;
    }

    public List<Enemy> GetEnemies() {
        return _enemyObjs;
    }

    public void RemoveCreature(Creature creature)
    {
        if (creature.GetType() == typeof(Enemy)) {
            _enemyObjs.Remove((Enemy)creature);
            if (_enemyObjs.Count == 0) {
                CardGameUIManager.i.DisplayVictoryScreen();
            }
        }
        else if (creature.GetType() == typeof(Adventurer)) {
            _adventurerObjs.Remove((Adventurer)creature);
            if (_adventurerObjs.Count == 0) {
                CardGameUIManager.i.DisplayDefeatScreen();
            }
        }
    }
}
