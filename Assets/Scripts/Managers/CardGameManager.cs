using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CardGameManager : GameManager
{
    public new static CardGameManager i;
    public Combat TestCombat;
    public Party TestParty;
    const float BUFFER_TIME = 1f;
    const float CREATURE_SPACING = 10f;
    public CombatState CurrCombatState;
    public Deck Deck;
    public Canvas UICanvas;
    public CardObject DraggedCard;
    public PlayZone HoveredPlayZone;
    [SerializeField] private Hand _hand;
    [SerializeField] private TMP_Text _instructionsText;
    [SerializeField] private TMP_Text _actionsCounter;
    [SerializeField] private TMP_Text _actionsLabel;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;
    private int _actions = 3;
    public int Actions {
        get { return _actions; }
        set {
            _actions = value;
            _actionsCounter.text = _actions.ToString();
            _actionsLabel.text = "action" + (_actions != 1 ? "s" : "") + " left";
            _actionsLabel.transform.localPosition = new Vector3(_actions / 10 * _actionsCounter.GetComponent<RectTransform>().rect.width, 0, 0);
        }
    }

    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private Transform _adventurerContainer;
    public List<Creature> SelectedCreatures = new List<Creature>();
    private List<System.Type> _currValidSelectTargets = new List<System.Type>();
    [SerializeField] private GameObject _selectedCreatureHighlight;
    [SerializeField] private List<Adventurer> _adventurers = new List<Adventurer>();
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    [SerializeField] private List<CombatSlot> _combatSlots;
    [SerializeField] private GameObject _combatSlotPrefab;
    public CardData CurrPlayedCard;
    private System.Random _r = new System.Random();
    protected override void Awake() {
        base.Awake();
        i = this;
        if (true) StartCoroutine(StartCombat(TestCombat, TestParty)); 
    }

    protected override void Update() {
        base.Update();
        //selecting creatures
        if (CurrPlayedCard && Input.GetMouseButtonDown(0)) {
            RaycastHit[] hits = Physics.RaycastAll(Camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), 50);

            foreach(RaycastHit hit in hits) {
                Creature creature = hit.collider.gameObject.GetComponent<Creature>();
                if (creature) {
                    if (CardGameManager.i.SelectedCreatures.Contains(creature)) {
                        creature.Deselect();
                    } else {
                        creature.Select();
                    }
                }
            }
        }
    }

    public void LoadOverworld()
    {
        Resume();
        StartCoroutine(FadeThenShowOverworld());
    }

    private IEnumerator FadeThenShowOverworld()
    {
        float fadeTime = UIManager.i.GetFadeTime();
        Music.FadeOutCurrent(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Camera.GetComponent<AudioListener>().enabled = false;
        var unloadingTask = SceneManager.UnloadSceneAsync(2);
        //while (!unloadingTask.isDone) yield return null;

        if (OverworldManager.i) OverworldManager.i.ShowOverworldObjects();
        else SceneManager.LoadScene(1);
    }
    
    public IEnumerator StartCombat(Combat combat, Party party) {
        //spawn enemies
        float absBound = ((float)combat.enemies.Length - 1) / 2 * CREATURE_SPACING;
        for (int i = 0; i < combat.enemies.Length; i++) {
            GameObject newCombatSlot = GameObject.Instantiate(_combatSlotPrefab, _enemyContainer);
            GameObject newEnemy = GameObject.Instantiate(combat.enemies[i], newCombatSlot.transform);
            _combatSlots.Add(newCombatSlot.GetComponent<CombatSlot>());
            newCombatSlot.GetComponent<CombatSlot>().Creature = newEnemy.GetComponent<Creature>();
            newEnemy.GetComponent<Enemy>().CombatSlot = newCombatSlot.GetComponent<CombatSlot>();
            // evenly distribute across enemy container
            float newCombatSlotPosX = Mathf.Lerp(-absBound, absBound, combat.enemies.Length != 1 ? i / ((float)combat.enemies.Length - 1) : 0.5f);
            newCombatSlot.transform.localPosition = new Vector3(newCombatSlotPosX, 0, 0);
            _enemies.Add(newEnemy.GetComponent<Enemy>());
        }

        //spawn adventurers
        List<AdventurerData> adventurerData = party.Adventurers;
        absBound = ((float)adventurerData.Count - 1) / 2 * CREATURE_SPACING;
        for (int i = 0; i < adventurerData.Count; i++) {
            GameObject newCombatSlot = GameObject.Instantiate(_combatSlotPrefab, _adventurerContainer);
            GameObject newAdventurer = GameObject.Instantiate(adventurerData[i].Adventurer, newCombatSlot.transform);
            _combatSlots.Add(newCombatSlot.GetComponent<CombatSlot>());
            newCombatSlot.GetComponent<CombatSlot>().Creature = newAdventurer.GetComponent<Creature>();
            newAdventurer.GetComponent<Adventurer>().CombatSlot = newCombatSlot.GetComponent<CombatSlot>();
            // evenly distribute across enemy container
            float newCombatSlotPosX = Mathf.Lerp(-absBound, absBound, adventurerData.Count != 1 ? i / ((float)adventurerData.Count - 1) : 0.5f);
            newCombatSlot.transform.localPosition = new Vector3(newCombatSlotPosX, 0, 0);
            _adventurers.Add(newAdventurer.GetComponent<Adventurer>());
            newAdventurer.GetComponent<Adventurer>().AdventurerData = adventurerData[i];
        }

        //construct deck

        foreach (AdventurerData data in party.Adventurers) {
            foreach (CardData card in data.Cards) {
                Deck.AddCard(card);
            }
        }

        yield return new WaitForSeconds(BUFFER_TIME / 2);

        _hand.DrawUntilFull();

        StartPlayerTurn();
    }

    public void EndCombat(bool won) {
        if (won) {
            _victoryScreen.SetActive(true);
        } else {
            _defeatScreen.SetActive(true);
        }

    }

    public void ExitCombatScreen() {
        //go to navigation, called by clicking something on end screen
        _victoryScreen.SetActive(false);
        _defeatScreen.SetActive(false);

        // add combat rewards here
        print("exiting combat");
    }

    public void StartPlayerTurn() {
        CurrCombatState = CombatState.PlayerTurn;

        _instructionsText.text = "Player Turn!";
        Actions = 3;

        _hand.DrawUntilFull();

        foreach (Enemy enemy in _enemies) {
            enemy.ShowIntent(_adventurers, _enemies);
        }
    }

    public void EndPlayerTurn() {
        CurrCombatState = CombatState.Idle;

        StartCoroutine(StartEnemyTurn());
    }

    public IEnumerator StartEnemyTurn() {
        CurrCombatState = CombatState.EnemyTurn;

        _instructionsText.text = "Enemy Turn!";

        yield return new WaitForSeconds(BUFFER_TIME);

        foreach (Enemy enemy in _enemies) {
            yield return StartCoroutine(enemy.Action(_adventurers, _enemies));
            yield return new WaitForSeconds(BUFFER_TIME);
        }

        CurrCombatState = CombatState.Idle;

        StartPlayerTurn();
    }

    public void CardStartsPlay(CardObject cardObject) {
        CurrPlayedCard = cardObject.CardData;
        _hand.MoveToDisplayAndPlay(cardObject);
    }

    public void CardPlayFunction(CardObject cardObject, CardPlayData data) {
        StartCoroutine(CardPlayFunction_Coroutine(cardObject, data));
    }

    public IEnumerator CardPlayFunction_Coroutine(CardObject cardObject, CardPlayData data) {
        switch (data.Function) {
            case Function.ATTACK: {
                List<System.Type> requiredTargets = new List<System.Type>() {typeof(Enemy)};

                IEnumerator currSelectTargets = SelectTargets(requiredTargets);
                yield return StartCoroutine(currSelectTargets);

                Creature defender = ((List<Creature>)currSelectTargets.Current).Find(x => x.GetType() == typeof(Enemy));

                yield return StartCoroutine(Utilities.LerpToAndBack(data.Owner.gameObject, defender.transform.position));
                defender.TakeDamage(data.Amount);
            }
            break;
                
            case Function.BLOCK: {
                List<System.Type> requiredTargets = new List<System.Type>() {typeof(Adventurer)};

                IEnumerator currSelectTargets = SelectTargets(requiredTargets);
                yield return StartCoroutine(currSelectTargets);

                Creature defendee = ((List<Creature>)currSelectTargets.Current).Find(x => x.GetType() == typeof(Adventurer));

                yield return StartCoroutine(Utilities.LerpToAndBack(data.Owner.gameObject, defendee.transform.position));
                defendee.AddBlock(data.Amount);
            }
            break;
        }

        //DoSpecial();

        CardEndsPlay(cardObject);
    }

    public void CardEndsPlay(CardObject cardObject) {
        CurrPlayedCard = null;
        
        DeselectAllCreatures();
        Actions--;

        _instructionsText.text = "";

        if (Actions == 0) {
            EndPlayerTurn();
        } else {
            CurrCombatState = CombatState.PlayerTurn;
        }

        Destroy(cardObject.gameObject);
    }

    public void MoveCardFromDisplay() {
        _instructionsText.text = "";
        _hand.MoveFromDisplay(CurrPlayedCard.CardObject);

        CurrPlayedCard = null;
    }

    public IEnumerator SelectTargets(List<System.Type> requiredTargets) {
        _instructionsText.text = "Select Targets";
        //wait until exactly the correct types of targets are selected
        DeselectAllCreatures();
        _currValidSelectTargets = requiredTargets;
        while(!CompareListsByType<Creature>(requiredTargets, SelectedCreatures)) yield return 0;
        
        yield return SelectedCreatures;
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
        
        creature.SelectedCreatureHighlight = GameObject.Instantiate(_selectedCreatureHighlight, creature.Canvas.gameObject.transform);
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

    public void RemoveCreature(Creature creature) {
        if (creature.GetType() == typeof(Enemy)) {
            _enemies.Remove((Enemy)creature);
            if (_enemies.Count == 0) {
                EndCombat(true);
            }
        } else if (creature.GetType() == typeof(Adventurer)) {
            _adventurers.Remove((Adventurer)creature);
            if (_adventurers.Count == 0) {
                EndCombat(false);
            }
        }
    }

    public Adventurer GetOwnerAdventurer(CardObject cardObject) {
        foreach (Adventurer dude in _adventurers) {
            if (dude.AdventurerData.Cards.Contains(cardObject.CardData)) return dude;
        }
        return null;
    }

    public Adventurer GetOwnerAdventurer(CardData cardData) {
        foreach (Adventurer dude in _adventurers) {
            if (dude.AdventurerData.Cards.Contains(cardData)) return dude;
        }
        return null;
    }

    public CombatSlot GetRandomAdventurerSlot(bool empty = false) {
        List<Adventurer> adventurers = _adventurers.OrderBy(x => _r.Next()).ToList();
        if (empty) {
            foreach(Adventurer adventurer in adventurers) {
                if (adventurer.CombatSlot && !adventurer.CombatSlot.Creature) return adventurer.CombatSlot;
            }
            return null;
        }
        return adventurers[0].CombatSlot;
    }

    public CombatSlot GetRandomEnemySlot(bool empty = false) {
        List<Enemy> enemies = _enemies.OrderBy(x => _r.Next()).ToList();
        if (empty) {
            foreach(Enemy enemy in enemies) {
                if (!enemy.CombatSlot.Creature) return enemy.CombatSlot;
            }
            return null;
        }

        return enemies[0].CombatSlot;
    }

    public CombatSlot SpawnBlockSlot(Vector3 position) {
        GameObject newCombatSlot = GameObject.Instantiate(_combatSlotPrefab, _enemyContainer);
        newCombatSlot.transform.position = position;
        newCombatSlot.GetComponent<CombatSlot>().IsBlockSlot = true;
        _combatSlots.Add(newCombatSlot.GetComponent<CombatSlot>());

        return newCombatSlot.GetComponent<CombatSlot>();
    }

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