using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CardGameManager : GameManager
{
    public new static CardGameManager i;
    public Combat testCombat;
    public Party testParty;
    protected override void Awake() { base.Awake(); i = this; StartCoroutine(StartCombat(testCombat, testParty)); }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                print(hit.collider.gameObject.name);
                Creature creature = hit.collider.gameObject.GetComponent<Creature>();
                if (creature) {
                    if (CardGameManager.i.selectedCreatures.Contains(creature)) {
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

    const float BUFFER_TIME = 1f;
    const float CREATURE_SPACING = 10f;
    public CombatState currCombatState;
    public Hand hand;
    public Deck deck;
    public Canvas UICanvas;
    public CardObject draggedCard;
    public PlayZone hoveredPlayZone;
    public TMP_Text instructionsText;
    public TMP_Text actionsCounter;
    public TMP_Text actionsLabel;
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    int Actions = 3;
    public int actions {
        get { return Actions; }
        set {
            Actions = value;
            actionsCounter.text = Actions.ToString();
            actionsLabel.text = "action" + (Actions != 1 ? "s" : "") + " left";
            actionsLabel.transform.localPosition = new Vector3(Actions / 10 * actionsCounter.GetComponent<RectTransform>().rect.width, 0, 0);
        }
    }

    public Transform enemyContainer;
    public Transform adventurerContainer;

    public List<Creature> selectedCreatures = new List<Creature>();
    public GameObject selectedCreatureHighlight;
    public List<Adventurer> adventurers = new List<Adventurer>();
    public List<Enemy> enemies = new List<Enemy>();
    public CardData currPlayedCard;
    public IEnumerator StartCombat(Combat combat, Party party) {
        print("combat starting");
        //spawn enemies
        float absBound = ((float)combat.enemies.Length - 1) / 2 * CREATURE_SPACING;
        for (int i = 0; i < combat.enemies.Length; i++) {
            GameObject newEnemy = GameObject.Instantiate(combat.enemies[i], enemyContainer);
            // evenly distribute across enemy container
            float newEnemyPosX = Mathf.Lerp(-absBound, absBound, combat.enemies.Length != 1 ? i / ((float)combat.enemies.Length - 1) : 0.5f);
            newEnemy.transform.localPosition = new Vector3(newEnemyPosX, 0, 0);
            enemies.Add(newEnemy.GetComponent<Enemy>());
        }

        //spawn adventurers
        List<AdventurerData> adventurerData = party.adventurerData;
        absBound = ((float)adventurerData.Count - 1) / 2 * CREATURE_SPACING;
        for (int i = 0; i < adventurerData.Count; i++) {
            GameObject newAdventurer = GameObject.Instantiate(adventurerData[i].prefab, adventurerContainer);
            // evenly distribute across enemy container
            float newAdventurerPosX = Mathf.Lerp(-absBound, absBound, adventurerData.Count != 1 ? i / ((float)adventurerData.Count - 1) : 0.5f);
            newAdventurer.transform.localPosition = new Vector3(newAdventurerPosX, 0, 0);
            adventurers.Add(newAdventurer.GetComponent<Adventurer>());
        }

        //construct deck
        foreach (AdventurerData data in party.adventurerData) {
            foreach (CardData card in data.cards) {
                yield return StartCoroutine(deck.AddCard(card));
            }
        }

        yield return new WaitForSeconds(BUFFER_TIME / 2);

        for (int i = 0; i < 7; i++) {
            yield return StartCoroutine(deck.DrawCard());
        }

        StartPlayerTurn();
    }

    public void EndCombat(bool won) {
        if (won) {
            victoryScreen.SetActive(true);
        } else {
            defeatScreen.SetActive(true);
        }

        foreach (Adventurer adventurer in adventurers) {
            Destroy(adventurer.gameObject);
        }
        foreach (Enemy enemy in enemies) {
            Destroy(enemy.gameObject);
        }

        foreach (CardObject card in hand.cards) {
            Destroy(card.gameObject);
        }
        hand.cards = new List<CardObject>();
        deck.cards = new List<CardData>();
    }

    public void ExitCombatScreen() {
        //go to navigation, called by clicking something on end screen
        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);

        //GameManager.i.ExitCombat();
        print("exiting combat");
    }

    public IEnumerator StartPlayerTurn() {
        currCombatState = CombatState.PlayerTurn;

        instructionsText.text = "Player Turn!";
        actions = 3;
        while (hand.cards.Count < hand.maxHandSize) {
            yield return StartCoroutine(deck.DrawCard());
        }

        //StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn() {
        //not used; select targets stuff handles. need to refactor to move behavior into this routine
        yield return null;
    }

    public void EndPlayerTurn() {
        currCombatState = CombatState.Idle;

        instructionsText.text = "";

        StartEnemyTurn();
    }

    public void StartEnemyTurn() {
        currCombatState = CombatState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn() {

        instructionsText.text = "Enemy Turn!";

        yield return new WaitForSeconds(BUFFER_TIME);
        foreach (Enemy enemy in enemies) {
            yield return StartCoroutine(enemy.Action(adventurers, enemies));
            yield return new WaitForSeconds(BUFFER_TIME);
        }

        currCombatState = CombatState.PlayerTurn;

        EndEnemyTurn();
    }

    public void EndEnemyTurn() {
        currCombatState = CombatState.Idle;

        instructionsText.text = "";

        StartPlayerTurn();
    }

    public void PlayCard() {
        currPlayedCard = null;
        
        DeselectAllCreatures();
        actions--;

        if (actions == 0) {
            EndPlayerTurn();
        } else {
            currCombatState = CombatState.PlayerTurn;
        }
    }

    public IEnumerator CancelPlayCard() {
        instructionsText.text = "";
        yield return StartCoroutine(hand.MoveFromPlayedCardDisplay(currPlayedCard.cardObject));

        currPlayedCard = null;
        currCombatState = CombatState.PlayerTurn;
    }

    public IEnumerator SelectTargets(List<System.Type> requiredTargets) {
        instructionsText.text = "Choose Targets";
        CardGameManager.i.currCombatState = CombatState.SelectTargets;
        //wait until exactly the correct types of targets are selected
        while(!CompareListsByType<Creature>(requiredTargets, selectedCreatures)) yield return 0;
        
        yield return selectedCreatures;
    }

    bool CompareListsByType<T>(List<System.Type> listA, List<T> listB) {
        foreach (System.Type type in listA.Distinct().ToList()) {
            if (listA.Count(x => x == type) != listB.Count(x => x.GetType() == type)) return false;
        }
        return true;
    }

    public void SelectCreature(Creature creature) {
        selectedCreatures.Add(creature);
        
        creature.selectedCreatureHighlight = GameObject.Instantiate(selectedCreatureHighlight, creature.canvas.gameObject.transform);
    }
    
    public void DeselectCreature(Creature creature) {
        selectedCreatures.Remove(creature);

        Destroy(creature.selectedCreatureHighlight);
    }

    public void DeselectAllCreatures() {
        while (selectedCreatures.Count > 0) {
            DeselectCreature(selectedCreatures[0]);
        }
    }

    public void RemoveCreature(Creature creature) {
        if (creature.GetType() == typeof(Enemy)) {
            enemies.Remove((Enemy)creature);
            if (enemies.Count == 0) {
                EndCombat(true);
            }
        } else if (creature.GetType() == typeof(Adventurer)) {
            adventurers.Remove((Adventurer)creature);
            if (adventurers.Count == 0) {
                EndCombat(false);
            }
        }
    }
}

public enum CombatState {
        PlayerTurn,
        EnemyTurn,
        SelectTargets,
        Idle,
        Waiting
    }