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
    [SerializeField] private GameObject _selectedCreatureHighlight;
    [SerializeField] private List<Adventurer> _adventurers = new List<Adventurer>();
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    public CardData CurrPlayedCard;
    protected override void Awake() {
        base.Awake();
        i = this;
        StartCoroutine(StartCombat(TestCombat, TestParty)); 
    }

    protected override void Update() {
        base.Update();
        //selecting creatures
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
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
            GameObject newEnemy = GameObject.Instantiate(combat.enemies[i], _enemyContainer);
            // evenly distribute across enemy container
            float newEnemyPosX = Mathf.Lerp(-absBound, absBound, combat.enemies.Length != 1 ? i / ((float)combat.enemies.Length - 1) : 0.5f);
            newEnemy.transform.localPosition = new Vector3(newEnemyPosX, 0, 0);
            _enemies.Add(newEnemy.GetComponent<Enemy>());
        }

        //spawn adventurers
        List<AdventurerData> adventurerData = party.adventurerData;
        absBound = ((float)adventurerData.Count - 1) / 2 * CREATURE_SPACING;
        for (int i = 0; i < adventurerData.Count; i++) {
            adventurerData[i].Adventurer = GameObject.Instantiate(adventurerData[i].Adventurer, _adventurerContainer);
            // evenly distribute across enemy container
            float newAdventurerPosX = Mathf.Lerp(-absBound, absBound, adventurerData.Count != 1 ? i / ((float)adventurerData.Count - 1) : 0.5f);
            adventurerData[i].Adventurer.transform.localPosition = new Vector3(newAdventurerPosX, 0, 0);
            _adventurers.Add(adventurerData[i].Adventurer.GetComponent<Adventurer>());
        }

        //construct deck
        foreach (AdventurerData data in party.adventurerData) {
            foreach (CardData card in data.cards) {
                CardData newCard = (CardData)ScriptableObject.CreateInstance(card.GetType());
                foreach (System.Reflection.FieldInfo fieldInfo in card.GetType().GetFields()) {
                    fieldInfo.SetValue(newCard, fieldInfo.GetValue(card));
                }
                newCard.Init(data);
                Deck.AddCard(newCard);
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
        SelectedCreatures.Add(creature);
        
        creature.selectedCreatureHighlight = GameObject.Instantiate(_selectedCreatureHighlight, creature.Canvas.gameObject.transform);
    }
    
    public void DeselectCreature(Creature creature) {
        SelectedCreatures.Remove(creature);

        Destroy(creature.selectedCreatureHighlight);
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
}

public enum CombatState {
        PlayerTurn,
        EnemyTurn,
        Idle, //in between states
    }